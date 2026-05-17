using FluentValidation;
using FluentValidation.AspNetCore;
using inzBackend.Jwt;
using inzBackend.Middlewares;
using inzBackend.Models;
using inzBackend.Models.UserModels;
using inzBackend.Models.Validators;
using inzBackend.Profiles;
using inzBackend.Services.AdminFlashcardServices;
using inzBackend.Services.AdminLearningServices.Lesson;
using inzBackend.Services.AdminLearningServices.LessonPanel;
using inzBackend.Services.AssignmentServices;
using inzBackend.Services.CatalogueServices;
using inzBackend.Services.CourseServices;
using inzBackend.Services.DashboardServices;
using inzBackend.Services.MatrixServices;
using inzBackend.Services.ModuleServices;
using inzBackend.Services.ProfileServices;
using inzBackend.Services.ProgramServices;
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
using inzBackend.Services.UserServices;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

namespace inzBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSwaggerGen();
            var connectionString = builder.Configuration.GetConnectionString("GmitrzakEnglishAppConnectionString");

            builder.Services.AddDbContext<GmitrzakEnglishAcademyDbContext>(options =>
                    options.UseNpgsql(connectionString));

            builder.Services.Configure<JsonOptions>(options =>
            {
                options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });


            builder.Services.AddControllers().AddFluentValidation();
            builder.Services.AddAutoMapper(typeof(GmitrzakEnglishAppMappingProfile).Assembly);

            var authenticationSettings = new AuthenticationSettings();
            builder.Configuration.GetSection("Authentication").Bind(authenticationSettings);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Bearer";
                options.DefaultScheme = "Bearer";
                options.DefaultChallengeScheme = "Bearer";

            }).AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;
                cfg.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = authenticationSettings.JwtIssuer,
                    ValidAudience = authenticationSettings.JwtIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey))
                };
            });

            // Dependency injection

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
            builder.Services.AddScoped<IAdminFlashcardService, AdminFlashcardService>();
            builder.Services.AddScoped<ILessonService, LessonService>();
            builder.Services.AddScoped<ILessonPanelService, LessonPanelService>();
            builder.Services.AddScoped<IDashboardService, DashboardService>();
            builder.Services.AddScoped<ICatalogueService, CatalogueService>();
            builder.Services.AddScoped<IPasswordHasher<AppUser>, PasswordHasher<AppUser>>();
            builder.Services.AddScoped<IValidator<RegisterUserRequest>, RegisterUserRequestValidator>();
            builder.Services.AddScoped<ExceptionHandlingMiddleware>();
            builder.Services.AddScoped<IUserContextService, UserContextService>();
            builder.Services.AddSingleton(authenticationSettings);

            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();
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

            app.Run();
        }
    }
}
