using inzBackend.Entities;
using inzBackend.Helpers;
using inzBackend.Services.UserServices;
using Microsoft.EntityFrameworkCore;
using System;

namespace inzBackend.Models
{
    public class GmitrzakEnglishAcademyDbContext : DbContext
    {
        private readonly IUserContextService _userContextService;
        public GmitrzakEnglishAcademyDbContext(DbContextOptions<GmitrzakEnglishAcademyDbContext> options, IUserContextService userContextService) : base(options) 
        {
            _userContextService = userContextService;
        }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<Program> Programs { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Matrix> Matrices { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<ProgramCourse> ProgramCourses { get; set; }
        public DbSet<CourseMatrix> CourseMatrices { get; set; }
        public DbSet<MatrixModule> MatrixModules { get; set; }
        public DbSet<UserMatrixAssignment> UserMatrixAssignments { get; set; }
        public DbSet<Flashcard> Flashcards { get; set; }
        public DbSet<FlashcardStudyLog> FlashcardStudyLogs { get; set; }
        public DbSet<Sentence> Sentences { get; set; }
        public DbSet<Memory> Memories { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<ListeningReport> ListeningReports { get; set; }
        public DbSet<PronunciationEntry> PronunciationEntries { get; set; }
        public DbSet<ActivityPoint> ActivityPoints { get; set; }
        public DbSet<Catalogue> Catalogues { get; set; }
        public DbSet<CatalogueEntry> CatalogueEntries { get; set; }
        public DbSet<Entities.Profile> Profiles { get; set; }
        public DbSet<UserModuleAssignment> UserModuleAssignments { get; set; }
        public DbSet<UserMatrixModuleCompletion> UserMatrixModuleCompletions { get; set; }
        public DbSet<TeacherNote> TeacherNotes { get; set; }
        public DbSet<Agenda> Agendas { get; set; }
        public DbSet<TheaterItem> TheaterItems { get; set; }
        public DbSet<Vocabulary> Vocabulary { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<AnnouncementRecipient> AnnouncementRecipients { get; set; }
        public DbSet<SentenceStock> SentenceStocks { get; set; }
        public DbSet<SentenceSet> SentenceSets { get; set; }
        public DbSet<SentenceSetItem> SentenceSetItems { get; set; }
        public DbSet<UserSentenceAssignment> UserSentenceAssignments { get; set; }
        public DbSet<UserSentenceAnswer> UserSentenceAnswers { get; set; }
        public DbSet<ModuleSentenceSet> ModuleSentenceSets { get; set; }
        public DbSet<UserLoginLog> UserLoginLogs { get; set; }
        public DbSet<RankingReaction> RankingReactions { get; set; }
        public DbSet<Attendance> Attendance { get; set; }
        public DbSet<ModulePresentation> ModulePresentations { get; set; }
        public DbSet<SectionActivityLog> SectionActivityLogs { get; set; }
        public DbSet<Credit> Credits { get; set; }
        public DbSet<ShopItem> ShopItems { get; set; }
        public DbSet<ShopPurchase> ShopPurchases { get; set; }

        public override int SaveChanges()
        {
            ApplyAuditInfo();
            return base.SaveChanges();
        }

        private void ApplyAuditInfo()
        {
            var entries = ChangeTracker
                .Entries<AuditableEntity>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            var currentUsername = _userContextService.GetUserName ?? "System";
            var now = PolandTime.DateTimeNow;

            foreach (var entityEntry in entries)
            {
                entityEntry.Entity.LastModifiedAt = now;
                entityEntry.Entity.LastModifiedBy = currentUsername;

                if (entityEntry.State == EntityState.Added)
                    entityEntry.Entity.CreatedBy = currentUsername;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(GmitrzakEnglishAcademyDbContext).Assembly);
        }
    }
}
