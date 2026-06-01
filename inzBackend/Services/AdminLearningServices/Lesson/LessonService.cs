using inzBackend.Entities;
using inzBackend.Exceptions;
using inzBackend.Models.AdminLearningModels;
using inzBackend.Models;
using inzBackend.Services.UserServices;
using inzBackend.Enums;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using inzBackend.Models.GlobalVocabularyModels;
using inzBackend.Models.StudentLearningModels.VocabularyModels;
using inzBackend.Helpers;
using inzBackend.Models.StudentLearningModels.MemoryModels;

namespace inzBackend.Services.AdminLearningServices.Lesson
{
    public class LessonService : ILessonService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IUserContextService _userContextService;
        private readonly IMapper _mapper;

        public LessonService(GmitrzakEnglishAcademyDbContext dbContext, IUserContextService userContextService, IMapper mapper)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
            _mapper = mapper;
        }

        public SearchGlobalFlashcardResult searchGlobalFlashcard(string query, int studentUserId)
        {
            var q = query.ToLower().Trim();

            var vocabulary = _dbContext.Vocabulary
                .FirstOrDefault(x => x.Front.ToLower().Contains(q) || x.Back.ToLower().Contains(q));

            if (vocabulary is null)
            {
                return new SearchGlobalFlashcardResult
                {
                    Front = query,
                    Back = string.Empty,
                    Category = string.Empty,
                    ExistsInGlobal = false,
                    AlreadyAssignedToStudent = false
                };
            }

            var alreadyAssigned = _dbContext.Flashcards
                .Any(x => x.UserId == studentUserId && x.VocabularyId == vocabulary.Id);

            return new SearchGlobalFlashcardResult
            {
                Id = vocabulary.Id,
                Front = vocabulary.Front,
                Back = vocabulary.Back,
                Category = vocabulary.Category,
                ExistsInGlobal = true,
                AlreadyAssignedToStudent = alreadyAssigned
            };
        }

        public VocabularyDto addTranslation(AddTranslationRequest request)
        {
            var existing = _dbContext.Vocabulary
                .FirstOrDefault(x => x.Front.ToLower() == request.Front.ToLower());

            if (existing is not null)
                return new VocabularyDto
                {
                    Id = existing.Id,
                    Front = existing.Front,
                    Back = existing.Back,
                    Category = existing.Category
                };

            var vocabulary = new Vocabulary
            {
                Front = request.Front,
                Back = request.Back,
                Category = request.Category
            };

            _dbContext.Vocabulary.Add(vocabulary);
            _dbContext.SaveChanges();

            return _mapper.Map<VocabularyDto>(vocabulary);
        }

        public void assignFlashcardToStudent(AssignFlashcardToStudentRequest request)
        {
            var vocabulary = _dbContext.Vocabulary
                .FirstOrDefault(x => x.Id == request.GlobalFlashcardId);

            if (vocabulary is null)
                throw new NotFoundException("Vocabulary word not found in global database");

            var alreadyExists = _dbContext.Flashcards
                .Any(x => x.UserId == request.StudentUserId && x.VocabularyId == vocabulary.Id);

            if (alreadyExists) return;

            var today = PolandTime.Today;

            var flashcard = new Flashcard
            {
                UserId = request.StudentUserId,
                VocabularyId = vocabulary.Id,
                EaseFactor = 250,
                Interval = 0,
                IsLeech = false,
                NextReviewDate = today
            };

            _dbContext.Flashcards.Add(flashcard);
            _dbContext.SaveChanges();
        }

        public void addSentence(AddSentenceRequest request)
        {
            _dbContext.Sentences.Add(new Sentence
            {
                UserId = request.StudentUserId,
                Content = request.Content,
                Translation = request.Translation,
                Notes = request.Notes
            });
            _dbContext.SaveChanges();
        }

        public void addMemory(AddMemoryRequest request)
        {
            _dbContext.Memories.Add(new Memory
            {
                UserId = request.StudentUserId,
                Content = request.Content,
                Notes = request.Notes
            });
            _dbContext.SaveChanges();
        }

        public void addPronunciation(AddPronunciationRequest request)
        {
            var maxOrder = _dbContext.PronunciationEntries
                .Where(x => x.UserId == request.StudentUserId)
                .Select(x => (int?)x.SortOrder)
                .Max() ?? 0;

            _dbContext.PronunciationEntries.Add(new PronunciationEntry
            {
                UserId = request.StudentUserId,
                Word = request.Word,
                IsChecked = false,
                SortOrder = maxOrder + 1
            });
            _dbContext.SaveChanges();
        }

        public List<HomeworkItemDto> getHomeworkForWeek(int studentUserId)
        {
            var today = PolandTime.Today;
            var weekStart = today.AddDays(-(int)today.DayOfWeek + 1);
            var weekEnd = weekStart.AddDays(6);

            return _dbContext.UserModuleAssignments
                .Include(x => x.Module)
                .Where(x => x.UserId == studentUserId
                         && x.DueDate >= weekStart
                         && x.DueDate <= weekEnd)
                .OrderBy(x => x.DueDate)
                .Select(x => new HomeworkItemDto
                {
                    Id = x.Id,
                    ModuleName = x.Module.Name,
                    ModuleDescription = x.Module.Description,
                    DueDate = x.DueDate,
                    IsCompleted = x.IsCompleted,
                    IsOverdue = x.DueDate < today && !x.IsCompleted
                })
                .ToList();
        }

        public void checkHomework(int assignmentId)
        {
            var a = _dbContext.UserModuleAssignments.FirstOrDefault(x => x.Id == assignmentId);
            if (a is null) return;
            a.IsCompleted = true;
            _dbContext.SaveChanges();
        }

        public void uncheckHomework(int assignmentId)
        {
            var a = _dbContext.UserModuleAssignments.FirstOrDefault(x => x.Id == assignmentId);
            if (a is null) return;
            a.IsCompleted = false;
            _dbContext.SaveChanges();
        }

        public List<PronunciationTestItemDto> getPronunciationList(int studentUserId)
        {
            return _dbContext.PronunciationEntries
                .Where(x => x.UserId == studentUserId)
                .OrderBy(x => x.IsChecked)
                .ThenBy(x => x.SortOrder)
                .Select(x => new PronunciationTestItemDto
                {
                    Id = x.Id,
                    Word = x.Word,
                    IsChecked = x.IsChecked,
                    SortOrder = x.SortOrder
                })
                .ToList();
        }

        public void checkPronunciationWord(int entryId)
        {
            var entry = _dbContext.PronunciationEntries.FirstOrDefault(x => x.Id == entryId);
            if (entry is null) return;
            entry.IsChecked = true;
            _dbContext.SaveChanges();
        }

        public void uncheckPronunciationWord(int entryId)
        {
            var entry = _dbContext.PronunciationEntries.FirstOrDefault(x => x.Id == entryId);
            if (entry is null) return;
            entry.IsChecked = false;
            _dbContext.SaveChanges();
        }

        public void addGrade(AddGradeRequest request)
        {
            _dbContext.Grades.Add(new Grade
            {
                UserId = request.StudentUserId,
                GradeDate = PolandTime.Today,
                Percentage = request.Percentage,
                Category = request.Category,
                Notes = request.Notes
            });
            _dbContext.SaveChanges();
        }

        public List<GradeListDto> getGrades(int studentUserId)
        {
            return _dbContext.Grades
                .Where(x => x.UserId == studentUserId)
                .OrderByDescending(x => x.GradeDate)
                .Select(x => new GradeListDto
                {
                    Id = x.Id,
                    GradeDate = x.GradeDate,
                    Percentage = x.Percentage,
                    Category = x.Category,
                    Notes = x.Notes
                })
                .ToList();
        }

        public void removeGrade(int gradeId)
        {
            var grade = _dbContext.Grades.FirstOrDefault(x => x.Id == gradeId);
            if (grade is null) return;
            _dbContext.Grades.Remove(grade);
            _dbContext.SaveChanges();
        }

        public List<TeacherNoteDto> getNotes(int studentUserId)
        {
            var teacherId = _userContextService.GetUserId;
            return _dbContext.TeacherNotes
                .Where(x => x.TeacherUserId == teacherId && x.StudentUserId == studentUserId)
                .OrderByDescending(x => x.NoteDate)
                .Select(x => new TeacherNoteDto
                {
                    Id = x.Id,
                    Content = x.Content,
                    NoteDate = x.NoteDate
                })
                .ToList();
        }

        public void saveNote(SaveNoteRequest request)
        {
            var teacherId = _userContextService.GetUserId;
            _dbContext.TeacherNotes.Add(new TeacherNote
            {
                TeacherUserId = teacherId!.Value,
                StudentUserId = request.StudentUserId,
                Content = request.Content,
                NoteDate = PolandTime.Today
            });
            _dbContext.SaveChanges();
        }

        public void deleteNote(int noteId)
        {
            var note = _dbContext.TeacherNotes.FirstOrDefault(x => x.Id == noteId);
            if (note is null) return;
            _dbContext.TeacherNotes.Remove(note);
            _dbContext.SaveChanges();
        }

        public void addListeningReport(AddListeningReportRequest request)
        {
            if (!Enum.TryParse<MediaType>(request.MediaType, out var mediaType))
                throw new BadRequestException($"Invalid media type: {request.MediaType}");

            _dbContext.ListeningReports.Add(new ListeningReport
            {
                UserId = request.StudentUserId,
                ReportDate = PolandTime.Today,
                Title = request.Title,
                MediaType = mediaType,
                EpisodeCount = request.EpisodeCount
            });
            _dbContext.SaveChanges();
        }

        public List<ListeningReportDto> getListeningReports(int studentUserId)
        {
            var listeningReports = _dbContext.ListeningReports
                .Where(x => x.UserId == studentUserId)
                .Select(x => new ListeningReportDto
                {
                    Id = x.Id,
                    ReportDate = x.ReportDate,
                    Title = x.Title,
                    MediaType = x.MediaType.ToString(),
                    EpisodeCount = x.EpisodeCount
                })
                .ToList();

            var singleModules = _dbContext.UserModuleAssignments
                .Include(ua => ua.Module)
                    .ThenInclude(m => m.TheaterItem)
                .Where(ua => ua.UserId == studentUserId && ua.Module.Category == "Watching")
                .ToList()
                .Select(ua => new ListeningReportDto
                {
                    Id = ua.Module.Id,
                    ReportDate = DateOnly.FromDateTime(ua.CreatedAt.DateTime),

                    Title = ua.Module.TheaterItem != null
                        ? $"{ua.Module.TheaterItem.Title} | (From watching module)"
                        : ua.Module.Name,

                    MediaType = ua.Module.TheaterItem != null
                        ? ua.Module.TheaterItem.MediaType.ToString()
                        : "Video",
                    EpisodeCount = 0
                });

            var matrixModules = _dbContext.UserMatrixModuleCompletions
                .Include(mc => mc.MatrixModule)
                    .ThenInclude(mm => mm.Module)
                        .ThenInclude(m => m.TheaterItem)
                .Where(mc => mc.UserId == studentUserId && mc.MatrixModule.Module.Category == "Watching")
                .ToList()
                .Select(mc => new ListeningReportDto
                {
                    Id = mc.MatrixModule.Id,
                    ReportDate = DateOnly.FromDateTime(mc.CreatedAt.DateTime),

                    Title = mc.MatrixModule.Module.TheaterItem != null
                        ? $"{mc.MatrixModule.Module.TheaterItem.Title} | (From watching module)"
                        : mc.MatrixModule.Module.Name,

                    MediaType = mc.MatrixModule.Module.TheaterItem != null
                        ? mc.MatrixModule.Module.TheaterItem.MediaType.ToString()
                        : "Video",
                    EpisodeCount = 0
                });

            return listeningReports
                .Concat(singleModules)
                .Concat(matrixModules)
                .OrderByDescending(x => x.ReportDate)
                .ToList();
        }

        public AdminStudentStudySummaryDto getStudyLogsReport(int studentUserId)
        {
            var report = _dbContext
                .Users
                .Where(user => user.Id == studentUserId)
                .Select(user => new AdminStudentStudySummaryDto
                {
                    Username = user.Username,
                    TotalTimeSpentSeconds = user.FlashcardStudyLogs.Any()
                        ? user.FlashcardStudyLogs.Sum(log => log.TimeSpentSeconds)
                        : null,
                    TotalFlashcardsDone = user.FlashcardStudyLogs.Any()
                        ? user.FlashcardStudyLogs.Sum(log => log.EasyCount + log.HardCount + log.IncorrectCount)
                        : null
                })
                .FirstOrDefault();

            if (report is null)
                throw new NotFoundException("Student not found");

            return report;
        }

        public List<MemoryDto> getMemories(int studentUserId)
        {
            return _dbContext.Memories
                .Where(x => x.UserId == studentUserId)
                .Select(x => new MemoryDto
                {
                    Id = x.Id,
                    Content = x.Content,
                    Notes = x.Notes
                })
                .ToList();
        }
    }
}