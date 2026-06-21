using AutoMapper;
using inzBackend.Entities.Administration;
using inzBackend.Entities.Assignments;
using inzBackend.Entities.Gamification;
using inzBackend.Entities.LearningMaterials;
using inzBackend.Entities.SpacedRepetition;
using inzBackend.Enums;
using inzBackend.Exceptions;
using inzBackend.Helpers;
using inzBackend.Models;
using inzBackend.Models.AdminLearningModels;
using inzBackend.Models.AiSpellCheckingModels;
using inzBackend.Models.StudentLearningModels.MemoryModels;
using inzBackend.Models.StudentLearningModels.VocabularyModels;
using inzBackend.Services.UserServices;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Services.AdminLearningServices.Lesson
{
    public class LessonService : ILessonService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IUserContextService _userContextService;
        private readonly IMapper _mapper;
        private const int SESSION_SIZE = 20;
        private const int CORRECT_EXPIRY_DAYS = 30;

        public LessonService(GmitrzakEnglishAcademyDbContext dbContext, IUserContextService userContextService, IMapper mapper)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
            _mapper = mapper;
        }

        public SearchGlobalFlashcardResult SearchGlobalFlashcard(string query, int studentUserId)
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

        public VocabularyDto AddTranslation(AddTranslationRequest request)
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

        public void AssignFlashcardToStudent(AssignFlashcardToStudentRequest request)
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

        public void AddSentence(AddSentenceRequest request)
        {
            var normalizedContent = request.Content.Trim().ToLower();

            var alreadyExists = _dbContext.Sentences
                .Any(x => x.UserId == request.StudentUserId
                       && x.Content.ToLower() == normalizedContent);

            if (alreadyExists)
                return;

            _dbContext.Sentences.Add(new Sentence
            {
                UserId = request.StudentUserId,
                Content = request.Content,
                Translation = request.Translation,
                Notes = request.Notes
            });
            _dbContext.SaveChanges();
        }

        public void AddMemory(AddMemoryRequest request)
        {
            var generatedContent = GeneratePrompts(request.OptionA, request.OptionB, request.Category);

            _dbContext.Memories.Add(new Memory
            {
                UserId = request.StudentUserId,
                OptionA = request.OptionA,
                OptionB = request.OptionB,
                Content = generatedContent,
                Notes = request.Notes,
                Category = request.Category
            });
            _dbContext.SaveChanges();
        }

        public void AddPronunciation(AddPronunciationRequest request)
        {
            var alreadyExists = _dbContext.PronunciationEntries
                .Any(x => x.UserId == request.StudentUserId
                       && x.Word.ToLower() == request.Word.ToLower().Trim());

            if (alreadyExists) return;

            var maxOrder = _dbContext.PronunciationEntries
                .Where(x => x.UserId == request.StudentUserId)
                .Select(x => (int?)x.SortOrder)
                .Max() ?? 0;

            _dbContext.PronunciationEntries.Add(new PronunciationEntry
            {
                UserId = request.StudentUserId,
                Word = request.Word.Trim(),
                Status = PronunciationStatus.Pending,
                SortOrder = maxOrder + 1,
                IsInCurrentSession = false
            });
            _dbContext.SaveChanges();

            RefreshSession(request.StudentUserId);
        }

        public List<HomeworkItemDto> GetHomeworkForWeek(int studentUserId)
        {
            var today = PolandTime.Today;
            var weekStart = WeekHelper.GetWeekMonday(today);
            var weekEnd = weekStart.AddDays(6);

            var result = new List<HomeworkItemDto>();

            var directAssignments = _dbContext.UserModuleAssignments
                .Include(x => x.Module)
                .Where(x => x.UserId == studentUserId
                         && x.DueDate <= weekEnd
                         && (x.DueDate >= weekStart || !x.IsCompleted))
                .ToList();

            var directDtos = directAssignments.Select(x => new HomeworkItemDto
            {
                Id = x.Id,
                ModuleName = x.Module.Name,
                ModuleDescription = x.Module.Description,
                DueDate = x.DueDate,
                IsCompleted = x.IsCompleted,
                IsOverdue = x.DueDate < today && !x.IsCompleted
            });
            result.AddRange(directDtos);

            var completedMatrixModuleIds = _dbContext.UserMatrixModuleCompletions
                .Where(x => x.UserId == studentUserId)
                .Select(x => x.MatrixModuleId)
                .ToHashSet();

            var dueDateOverrides = _dbContext.UserMatrixModuleDueDateOverrides
                .Where(x => x.UserId == studentUserId)
                .ToDictionary(x => x.MatrixModuleId, x => x.NewDeadline);

            var matrixAssignments = _dbContext.UserMatrixAssignments
                .Include(x => x.Matrix)
                    .ThenInclude(m => m.MatrixModules)
                        .ThenInclude(mm => mm.Module)
                .Where(x => x.UserId == studentUserId)
                .ToList();

            foreach (var ma in matrixAssignments)
            {
                foreach (var mm in ma.Matrix.MatrixModules)
                {
                    var unlockDate = MatrixModuleDateHelper.ComputeDeadline(
                        ma.StartDate, mm.WeekNumber, mm.DayOfWeek, ma.Matrix.RefreshIntervalDays);

                    if (unlockDate > weekEnd) continue;

                    var isCompleted = completedMatrixModuleIds.Contains(mm.Id);

                    if (unlockDate < weekStart && isCompleted) continue;

                    var deadline = dueDateOverrides.TryGetValue(mm.Id, out var ov) ? ov : unlockDate;

                    result.Add(new HomeworkItemDto
                    {
                        Id = -mm.Id,
                        ModuleName = $"{mm.Module.Name} (Matrix: {ma.Matrix.Name})",
                        ModuleDescription = mm.Module.Description ?? string.Empty,
                        DueDate = deadline,
                        IsCompleted = isCompleted,
                        IsOverdue = deadline < today && !isCompleted,
                        IsFromMatrix = true
                    });
                }
            }

            return result.OrderByDescending(x => x.IsOverdue).ThenBy(x => x.DueDate).ToList();
        }

        public void CheckHomework(int assignmentId)
        {
            if (assignmentId < 0)
            {
                int realMatrixModuleId = Math.Abs(assignmentId);

                var matrixAssignment = _dbContext.UserMatrixAssignments
                    .FirstOrDefault(x => x.Matrix.MatrixModules.Any(mm => mm.Id == realMatrixModuleId));

                if (matrixAssignment is null) return;

                var exists = _dbContext.UserMatrixModuleCompletions
                    .Any(x => x.UserId == matrixAssignment.UserId && x.MatrixModuleId == realMatrixModuleId);

                if (!exists)
                {
                    _dbContext.UserMatrixModuleCompletions.Add(new UserMatrixModuleCompletion
                    {
                        UserId = matrixAssignment.UserId,
                        MatrixModuleId = realMatrixModuleId
                    });
                    _dbContext.SaveChanges();
                }
            }
            else
            {
                var a = _dbContext.UserModuleAssignments.FirstOrDefault(x => x.Id == assignmentId);
                if (a is null) return;
                a.IsCompleted = true;
                _dbContext.SaveChanges();
            }
        }

        public void UncheckHomework(int assignmentId)
        {
            if (assignmentId < 0)
            {
                int realMatrixModuleId = Math.Abs(assignmentId);

                var matrixAssignment = _dbContext.UserMatrixAssignments
                    .FirstOrDefault(x => x.Matrix.MatrixModules.Any(mm => mm.Id == realMatrixModuleId));

                if (matrixAssignment is null) return;

                var completion = _dbContext.UserMatrixModuleCompletions
                    .FirstOrDefault(x => x.UserId == matrixAssignment.UserId && x.MatrixModuleId == realMatrixModuleId);

                if (completion != null)
                {
                    _dbContext.UserMatrixModuleCompletions.Remove(completion);
                    _dbContext.SaveChanges();
                }
            }
            else
            {
                var a = _dbContext.UserModuleAssignments.FirstOrDefault(x => x.Id == assignmentId);
                if (a is null) return;
                a.IsCompleted = false;
                _dbContext.SaveChanges();
            }
        }

        public List<PronunciationTestItemDto> GetPronunciationList(int studentUserId)
        {
            RefreshSession(studentUserId);

            return _dbContext.PronunciationEntries
                .Where(x => x.UserId == studentUserId
                         && x.IsInCurrentSession)
                .OrderBy(x => x.Status == PronunciationStatus.Incorrect ? 0
                            : x.Status == PronunciationStatus.Pending ? 1 : 2)
                .ThenBy(x => x.SortOrder)
                .Select(x => new PronunciationTestItemDto
                {
                    Id = x.Id,
                    Word = x.Word,
                    Status = x.Status.ToString(),
                    SortOrder = x.SortOrder
                })
                .ToList();
        }

        public List<PronunciationTestItemDto> GetCorrectEntries(int studentUserId)
        {
            var today = PolandTime.Today;

            return _dbContext.PronunciationEntries
                .Where(x => x.UserId == studentUserId
                         && x.Status == PronunciationStatus.Correct)
                .OrderByDescending(x => x.MarkedCorrectAt)
                .Select(x => new PronunciationTestItemDto
                {
                    Id = x.Id,
                    Word = x.Word,
                    Status = x.Status.ToString(),
                    SortOrder = x.SortOrder,
                    MarkedCorrectAt = x.MarkedCorrectAt,
                    DaysUntilRefresh = x.MarkedCorrectAt.HasValue
                        ? Math.Max(0, CORRECT_EXPIRY_DAYS -
                            (today.DayNumber - x.MarkedCorrectAt.Value.DayNumber))
                        : null
                })
                .ToList();
        }

        public void CheckPronunciationWord(int entryId)
        {
            var entry = _dbContext.PronunciationEntries
                .FirstOrDefault(x => x.Id == entryId);
            if (entry is null) return;

            entry.Status = PronunciationStatus.Correct;
            entry.MarkedCorrectAt = PolandTime.Today;
            entry.IsInCurrentSession = false;

            _dbContext.SaveChanges();

            RefreshSession(entry.UserId);
        }

        public void UncheckPronunciationWord(int entryId)
        {
            var entry = _dbContext.PronunciationEntries
                .FirstOrDefault(x => x.Id == entryId);
            if (entry is null) return;

            entry.Status = PronunciationStatus.Pending;
            entry.MarkedCorrectAt = null;
            entry.IsInCurrentSession = true;

            _dbContext.SaveChanges();
        }

        public void MarkPronunciationResult(MarkPronunciationRequest request)
        {
            var entry = _dbContext.PronunciationEntries
                .FirstOrDefault(x => x.Id == request.EntryId)
                ?? throw new NotFoundException("Entry not found");

            if (request.Result.ToLower() == "correct")
            {
                entry.Status = PronunciationStatus.Correct;
                entry.MarkedCorrectAt = PolandTime.Today;
                entry.IsInCurrentSession = false;
            }
            else
            {
                entry.Status = PronunciationStatus.Incorrect;
                entry.MarkedCorrectAt = null;
                entry.IsInCurrentSession = true;
            }

            _dbContext.SaveChanges();
            RefreshSession(entry.UserId);
        }

        private void RefreshSession(int userId)
        {
            var today = PolandTime.Today;

            var expiredCorrect = _dbContext.PronunciationEntries
                .Where(x => x.UserId == userId
                         && x.IsInCurrentSession
                         && x.Status == PronunciationStatus.Correct
                         && x.MarkedCorrectAt.HasValue
                         && x.MarkedCorrectAt.Value.AddDays(CORRECT_EXPIRY_DAYS) < today)
                .ToList();

            foreach (var e in expiredCorrect)
                e.IsInCurrentSession = false;

            var currentSessionCount = _dbContext.PronunciationEntries
                .Count(x => x.UserId == userId
                         && x.IsInCurrentSession
                         && !(x.Status == PronunciationStatus.Correct
                              && x.MarkedCorrectAt.HasValue
                              && x.MarkedCorrectAt.Value.AddDays(CORRECT_EXPIRY_DAYS) < today));

            var needed = SESSION_SIZE - currentSessionCount;
            if (needed <= 0)
            {
                _dbContext.SaveChanges();
                return;
            }

            var inSessionIds = _dbContext.PronunciationEntries
                .Where(x => x.UserId == userId && x.IsInCurrentSession)
                .Select(x => x.Id)
                .ToList();

            var toAdd = _dbContext.PronunciationEntries
                .Where(x => x.UserId == userId
                         && !x.IsInCurrentSession
                         && !inSessionIds.Contains(x.Id)
                         && x.Status != PronunciationStatus.Correct)
                .OrderBy(x => x.Status == PronunciationStatus.Incorrect ? 0 : 1)
                .ThenBy(x => x.SortOrder)
                .Take(needed)
                .ToList();

            foreach (var e in toAdd)
                e.IsInCurrentSession = true;

            _dbContext.SaveChanges();
        }

        public void AddGrade(AddGradeRequest request)
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

        public List<GradeListDto> GetGrades(int studentUserId)
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

        public void RemoveGrade(int gradeId)
        {
            var grade = _dbContext.Grades.FirstOrDefault(x => x.Id == gradeId);
            if (grade is null) return;
            _dbContext.Grades.Remove(grade);
            _dbContext.SaveChanges();
        }

        public List<TeacherNoteDto> GetNotes(int studentUserId)
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

        public void SaveNote(SaveNoteRequest request)
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

        public void DeleteNote(int noteId)
        {
            var note = _dbContext.TeacherNotes.FirstOrDefault(x => x.Id == noteId);
            if (note is null) return;
            _dbContext.TeacherNotes.Remove(note);
            _dbContext.SaveChanges();
        }

        public void AddListeningReport(AddListeningReportRequest request)
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

        public List<ListeningReportDto> GetListeningReports(int studentUserId)
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

        public AdminStudentStudySummaryDto GetStudyLogsReport(int studentUserId)
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

        public List<MemoryDto> GetMemories(int studentUserId)
        {
            return _dbContext.Memories
                .Where(x => x.UserId == studentUserId)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new MemoryDto
                {
                    Id = x.Id,
                    OptionA = x.OptionA,
                    OptionB = x.OptionB,
                    Content = x.Content,
                    Notes = x.Notes,
                    Category = x.Category
                })
                .ToList();
        }

        private static readonly List<PromptTemplateItem> PromptTemplates = new()
        {
            new() { Key = "difference",   Template = "What's the difference between {A} and {B}? Provide examples." },
            new() { Key = "comma_before",  Template = "Do you put a comma before {A}? Are there any exceptions? Back it up with examples." },
            new() { Key = "position",      Template = "Where do you put the word {A} in a sentence? Is there only one option? Give examples." },
            new() { Key = "past",          Template = "What are the past forms of the word {A}? Is it regular or irregular? Use all in sentences." },
            new() { Key = "change",        Template = "How do you change a verb after the word {A}?" },
            new() { Key = "synonym",       Template = "What are the synonyms of {A}? Show the difference between them in context." },
            new() { Key = "antonym",       Template = "What are the antonyms of {A}? Provide example sentences." },
            new() { Key = "preposition",   Template = "What prepositions are used with {A}? Give examples for each." },
            new() { Key = "collocations",  Template = "What are the most common collocations with {A}? Use them in sentences." },
            new() { Key = "formal",        Template = "Is {A} formal or informal? What's the formal/informal alternative?" },
            new() { Key = "grammar",       Template = "Explain the grammar rules for using {A}. What are the most common mistakes?" }
        };

        private string GeneratePrompts(string optionA, string? optionB, string? category)
        {
            var templates = string.IsNullOrWhiteSpace(category)
                ? PromptTemplates
                : PromptTemplates.Where(t => t.Key == category).ToList();

            var lines = new List<string>();

            foreach (var promptTemplateItem in templates)
            {
                if (promptTemplateItem.Template.Contains("{B}") && string.IsNullOrWhiteSpace(optionB))
                    continue;

                var prompt = promptTemplateItem.Template.Trim()
                    .Replace("{A}", optionA.Trim())
                    .Replace("{B}", optionB.Trim() ?? string.Empty);

                lines.Add($"{prompt}");
            }

            return string.Join("\n", lines);
        }
    }
}