using CloudinaryDotNet;
using FluentValidation;
using FluentValidation.AspNetCore;
using GenerativeAI;
using inzBackend.Entities.Identity;
using inzBackend.Helpers;
using inzBackend.Middlewares;
using inzBackend.Models;
using inzBackend.Models.UserModels;
using inzBackend.Models.Validators;
using inzBackend.Profiles;
using inzBackend.Services.AdminLearningServices.Lesson;
using inzBackend.Services.AdminLearningServices.LessonPanel;
using inzBackend.Services.AiIntegrationServices;
using inzBackend.Services.AnnouncementsServices;
using inzBackend.Services.AssignmentServices;
using inzBackend.Services.CatalogueServices;
using inzBackend.Services.CourseServices;
using inzBackend.Services.CreditServices;
using inzBackend.Services.DashboardServices;
using inzBackend.Services.EssayServices;
using inzBackend.Services.ExaminationServices;
using inzBackend.Services.GlobalVocabularyServices;
using inzBackend.Services.MatrixServices;
using inzBackend.Services.ModuleServices;
using inzBackend.Services.ProfileServices;
using inzBackend.Services.ProgramServices;
using inzBackend.Services.RankingServices;
using inzBackend.Services.ReportServices;
using inzBackend.Services.SectionActivityServices;
using inzBackend.Services.SentenceServices;
using inzBackend.Services.StudentCourseServices;
using inzBackend.Services.StudentCourseServices.ActivityPoint;
using inzBackend.Services.StudentCourseServices.Grade;
using inzBackend.Services.StudentCourseServices.LastWeek;
using inzBackend.Services.StudentCourseServices.Stats;
using inzBackend.Services.StudentLearningServices.Assignment;
using inzBackend.Services.StudentLearningServices.Flashcards;
using inzBackend.Services.StudentLearningServices.Memories;
using inzBackend.Services.StudentLearningServices.Pronunciation;
using inzBackend.Services.StudentLearningServices.Sentences;
using inzBackend.Services.StudentLearningServices.Vocabulary;
using inzBackend.Services.TheaterItemServices;
using inzBackend.Services.UserAnswerServices;
using inzBackend.Services.UserServices;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenAI;
using OpenAI.Audio;
using OpenAI.Chat;
using System.Text;
using System.Text.Json.Serialization;

namespace inzBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            var builder = WebApplication.CreateBuilder(args);

            // Configuration & DbContext
            var connectionString = builder.Configuration.GetConnectionString("GmitrzakEnglishAppConnectionString");

            builder.Services.AddDbContext<GmitrzakEnglishAcademyDbContext>(options =>
                options.UseNpgsql(connectionString));

            // Controllers, JSON & Validation
            builder.Services.AddSwaggerGen();

            builder.Services.Configure<JsonOptions>(options =>
            {
                options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                })
                .AddFluentValidation();

            builder.Services.AddAutoMapper(typeof(GmitrzakEnglishAppMappingProfile).Assembly);

            // Authentication (JWT)
            var authenticationSettings = new AuthenticationSettings();
            builder.Configuration.GetSection("Authentication").Bind(authenticationSettings);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Bearer";
                options.DefaultScheme = "Bearer";
                options.DefaultChallengeScheme = "Bearer";
            })
            .AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
                cfg.SaveToken = true;
                cfg.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = authenticationSettings.JwtIssuer,
                    ValidAudience = authenticationSettings.JwtIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey))
                };
            });

            builder.Services.AddSingleton(authenticationSettings);

            // External integrations: Groq (OpenAI-compatible) & Cloudinary
            builder.Services.AddScoped<ChatClient>(sp =>
            {
                var apiKey = builder.Configuration["GroqSettings:ApiKey"] ?? "";
                var modelId = builder.Configuration["GroqSettings:ModelId"] ?? "llama-3.3-70b-versatile";

                var clientOptions = new OpenAIClientOptions
                {
                    Endpoint = new Uri("https://api.groq.com/openai/v1")
                };

                var openAiClient = new OpenAIClient(
                    new System.ClientModel.ApiKeyCredential(apiKey), clientOptions);

                return openAiClient.GetChatClient(modelId);
            });

            builder.Services.AddSingleton<GenerativeModel>(sp =>
            {
                var apiKey = builder.Configuration["GeminiSettings:ApiKey"];
                return new GenerativeModel(apiKey, "models/gemini-1.5-flash");
            });

            var cloudinarySettings = builder.Configuration.GetSection("CloudinarySettings");
            var cloudinaryAccount = new Account(
                cloudinarySettings["CloudName"],
                cloudinarySettings["ApiKey"],
                cloudinarySettings["ApiSecret"]
            );
            builder.Services.AddSingleton(new Cloudinary(cloudinaryAccount));

            // Application services
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IProfileService, ProfileService>();
            builder.Services.AddScoped<IProgramService, ProgramService>();
            builder.Services.AddScoped<ICourseService, CourseService>();
            builder.Services.AddScoped<IMatrixService, MatrixService>();
            builder.Services.AddScoped<IAssignmentService, AssignmentService>();
            builder.Services.AddScoped<IModuleService, ModuleService>();
            builder.Services.AddScoped<IStudentCourseService, StudentCourseService>();
            builder.Services.AddScoped<ILastWeekService, LastWeekService>();
            builder.Services.AddScoped<IActivityPointService, ActivityPointService>();
            builder.Services.AddScoped<IGradesService, GradesService>();
            builder.Services.AddScoped<IStatsService, StatsService>();
            builder.Services.AddScoped<ISentencesService, SentencesService>();
            builder.Services.AddScoped<IMemoriesService, MemoriesService>();
            builder.Services.AddScoped<IPronunciationService, PronunciationService>();
            builder.Services.AddScoped<IFlashcardsService, FlashcardsService>();
            builder.Services.AddScoped<IVocabularyService, VocabularyService>();
            builder.Services.AddScoped<IStudentAssignmentService, StudentAssignmentService>();
            builder.Services.AddScoped<ILessonService, LessonService>();
            builder.Services.AddScoped<ILessonPanelService, LessonPanelService>();
            builder.Services.AddScoped<IDashboardService, DashboardService>();
            builder.Services.AddScoped<ICatalogueService, CatalogueService>();
            builder.Services.AddScoped<ITheaterService, TheaterService>();
            builder.Services.AddScoped<IGlobalVocabularyService, GlobalVocabularyService>();
            builder.Services.AddScoped<IAiTranslationService, AiTranslationService>();
            builder.Services.AddScoped<IAnnouncementService, AnnouncementService>();
            builder.Services.AddScoped<ISentenceService, SentenceService>();
            builder.Services.AddScoped<IUserAnswerService, UserAnswerService>();
            builder.Services.AddScoped<IAiSentenceCheckerService, AiSentenceCheckerService>();
            builder.Services.AddScoped<IModuleReportExportService, ModuleReportExportService>();
            builder.Services.AddScoped<IRankingService, RankingService>();
            builder.Services.AddScoped<ISectionActivityService, SectionActivityService>();
            builder.Services.AddScoped<IExaminationService, ExaminationService>();
            builder.Services.AddScoped<ICreditService, CreditService>();
            builder.Services.AddScoped<IEssayService, EssayService>();
            builder.Services.AddScoped<IAiSpellCheckService, AiSpellCheckService>();
            builder.Services.AddScoped<IAiPronunciationService, AiPronunciationService>();

            // Infrastructure / cross-cutting services
            builder.Services.AddScoped<IPasswordHasher<AppUser>, PasswordHasher<AppUser>>();
            builder.Services.AddScoped<IValidator<RegisterUserRequest>, RegisterUserRequestValidator>();
            builder.Services.AddScoped<ExceptionHandlingMiddleware>();
            builder.Services.AddScoped<IUserContextService, UserContextService>();

            builder.Services.AddHttpContextAccessor();

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AngularCorsPolicy", policy =>
                {
                    var frontendUrl = builder.Configuration["FrontendUrl"] ?? "http://localhost:4200";
                    policy.WithOrigins(frontendUrl)
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // Build app & middleware pipeline
            var app = builder.Build();

            app.UseCors("AngularCorsPolicy");
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseAuthentication();
            app.UseStaticFiles();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            // Apply pending EF Core migrations
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<GmitrzakEnglishAcademyDbContext>();

                if (dbContext.Database.GetPendingMigrations().Any())
                    dbContext.Database.Migrate();
            }

            app.Run();
        }
    }
}