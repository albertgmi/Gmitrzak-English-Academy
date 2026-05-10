using inzBackend.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace inzBackend.Models
{
    public class GmitrzakEnglishAcademyDbContext : DbContext
    {
        public GmitrzakEnglishAcademyDbContext(DbContextOptions<GmitrzakEnglishAcademyDbContext> options) : base(options) { }
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
        public DbSet<StreamEntry> StreamEntries { get; set; }
        public DbSet<Entities.Profile> Profiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(GmitrzakEnglishAcademyDbContext).Assembly);
        }
    }
}
