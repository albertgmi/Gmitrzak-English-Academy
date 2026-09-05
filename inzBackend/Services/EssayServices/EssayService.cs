using ClosedXML.Excel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using inzBackend.Entities;
using inzBackend.Exceptions;
using inzBackend.Helpers;
using inzBackend.Models;
using inzBackend.Models.EssayModels;
using inzBackend.Services.AdminLearningServices.LessonPanel;
using inzBackend.Services.UserServices;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.IO.Compression;
using OpenXmlDocument = DocumentFormat.OpenXml.Wordprocessing.Document;
using inzBackend.Entities.Curriculum;
using inzBackend.Entities.Assignments;
using inzBackend.Services.ReportServices;

namespace inzBackend.Services.EssayServices
{
    public class EssayService : IEssayService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IUserContextService _userContextService;
        private readonly ILessonPanelService _lessonPanelService;

        public EssayService(
            GmitrzakEnglishAcademyDbContext dbContext,
            IUserContextService userContextService,
            ILessonPanelService lessonPanelService)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
            _lessonPanelService = lessonPanelService;
        }

        public EssayModuleDto GetEssayModule(int moduleId)
        {
            var userId = _userContextService.GetUserId!.Value;

            var module = _dbContext.Modules
                .FirstOrDefault(x => x.Id == moduleId && x.Category == "Essay")
                ?? throw new NotFoundException("Essay module not found");

            var activeAssignment = _dbContext.UserModuleAssignments
                .FirstOrDefault(x => x.UserId == userId && x.ModuleId == moduleId && !x.IsCompleted);

            UserEssay? existing = null;

            if (activeAssignment != null)
            {
                existing = _dbContext.UserEssays
                    .FirstOrDefault(x => x.UserId == userId
                                      && x.ModuleId == moduleId
                                      && x.UserModuleAssignmentId == activeAssignment.Id);
            }
            else
            {
                existing = _dbContext.UserEssays
                    .Where(x => x.UserId == userId && x.ModuleId == moduleId)
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefault();
            }

            return new EssayModuleDto
            {
                ModuleId = module.Id,
                ModuleName = module.Name,
                EssayPrompt = module.EssayPrompt ?? string.Empty,
                ExistingEssay = existing is null ? null : MapToDto(existing, module)
            };
        }

        public UserEssayDto SubmitEssay(SubmitEssayRequest request)
        {
            var userId = _userContextService.GetUserId!.Value;
            var today = PolandTime.Today;

            var module = _dbContext.Modules
                .FirstOrDefault(x => x.Id == request.ModuleId)
                ?? throw new NotFoundException("Module not found");

            var activeAssignment = _dbContext.UserModuleAssignments
                .FirstOrDefault(x => x.UserId == userId && x.ModuleId == request.ModuleId && !x.IsCompleted);

            UserEssay newOrExistingEssay;

            if (activeAssignment != null)
            {
                var alreadySubmitted = _dbContext.UserEssays
                    .Any(x => x.UserId == userId
                           && x.ModuleId == request.ModuleId
                           && x.UserModuleAssignmentId == activeAssignment.Id
                           && x.IsSubmitted);

                if (alreadySubmitted)
                    throw new BadRequestException("Essay already submitted for this assignment.");

                newOrExistingEssay = new UserEssay
                {
                    UserId = userId,
                    ModuleId = request.ModuleId,
                    UserModuleAssignmentId = activeAssignment.Id,
                    Content = request.Content,
                    IsSubmitted = true,
                    SubmittedDate = today
                };
                _dbContext.UserEssays.Add(newOrExistingEssay);
            }
            else
            {
                newOrExistingEssay = new UserEssay
                {
                    UserId = userId,
                    ModuleId = request.ModuleId,
                    Content = request.Content,
                    IsSubmitted = true,
                    SubmittedDate = today
                };
                _dbContext.UserEssays.Add(newOrExistingEssay);
            }

            _dbContext.SaveChanges();

            CompleteModuleForUser(userId, request.ModuleId);

            _lessonPanelService.AddActivityPoints(
                userId, 15, $"Essay submitted: {module.Name}");

            return MapToDto(newOrExistingEssay, module);
        }

        public List<UserEssayDto> GetAllEssaysForAdmin()
        {
            return _dbContext.UserEssays
                .Include(x => x.User).ThenInclude(u => u.Profile)
                .Include(x => x.Module)
                .Where(x => x.IsSubmitted)
                .OrderByDescending(x => x.SubmittedDate)
                .Select(x => new UserEssayDto
                {
                    Id = x.Id,
                    ModuleId = x.ModuleId,
                    ModuleName = x.Module.Name,
                    EssayPrompt = x.Module.EssayPrompt ?? string.Empty,
                    Content = x.Content,
                    AdminContent = x.AdminContent,
                    IsSubmitted = x.IsSubmitted,
                    IsReviewed = x.IsReviewed,
                    SubmittedDate = x.SubmittedDate,
                    ReviewedDate = x.ReviewedDate,
                    Username = x.User.Username,
                    AvatarUrl = x.User.Profile != null ? x.User.Profile.AvatarUrl : null
                })
                .ToList();
        }

        public List<UserEssayDto> GetMyEssays()
        {
            var userId = _userContextService.GetUserId!.Value;
            return GetEssaysForStudent(userId);
        }

        public List<UserEssayDto> GetEssaysForStudent(int studentId)
        {
            return _dbContext.UserEssays
                .Include(x => x.User).ThenInclude(u => u.Profile)
                .Include(x => x.Module)
                .Where(x => x.UserId == studentId && x.IsSubmitted)
                .OrderByDescending(x => x.SubmittedDate)
                .Select(x => new UserEssayDto
                {
                    Id = x.Id,
                    ModuleId = x.ModuleId,
                    ModuleName = x.Module.Name,
                    EssayPrompt = x.Module.EssayPrompt ?? string.Empty,
                    Content = x.Content,
                    AdminContent = x.AdminContent,
                    IsSubmitted = x.IsSubmitted,
                    IsReviewed = x.IsReviewed,
                    SubmittedDate = x.SubmittedDate,
                    ReviewedDate = x.ReviewedDate,
                    Username = x.User.Username,
                    AvatarUrl = x.User.Profile != null ? x.User.Profile.AvatarUrl : null
                })
                .ToList();
        }

        public UserEssayDto ReviewEssay(int essayId, ReviewEssayRequest request)
        {
            var essay = _dbContext.UserEssays
                .Include(x => x.User)
                .Include(x => x.Module)
                .FirstOrDefault(x => x.Id == essayId)
                ?? throw new NotFoundException("Essay not found");

            essay.AdminContent = request.AdminContent;
            essay.IsReviewed = true;
            essay.ReviewedDate = PolandTime.Today;

            _dbContext.SaveChanges();

            return MapToDto(essay, essay.Module);
        }

        public byte[] ExportEssayToDocx(int essayId)
        {
            var essay = _dbContext.UserEssays
                .Include(x => x.User)
                .Include(x => x.Module)
                .FirstOrDefault(x => x.Id == essayId)
                ?? throw new NotFoundException("Essay not found");

            using var stream = new MemoryStream();

            using (var doc = WordprocessingDocument.Create(
                stream, WordprocessingDocumentType.Document, true))
            {
                var mainPart = DocxHelper.InitDocument(doc);
                var body = new Body();

                body.Append(DocxHelper.CreateParagraph("Essay Review", bold: true, fontSize: 32, spaceAfter: 160));
                body.Append(DocxHelper.CreateParagraph($"Student: {essay.User.Username}", bold: true, spaceAfter: 60));
                body.Append(DocxHelper.CreateParagraph(
                    $"Submitted: {essay.SubmittedDate?.ToString("d MMM yyyy") ?? "—"}", spaceAfter: 200));

                body.Append(DocxHelper.CreateSectionHeader("Essay Prompt", "2E74B5"));
                body.Append(DocxHelper.CreateParagraph(essay.Module.EssayPrompt ?? string.Empty, italic: true, spaceAfter: 200));

                if (!string.IsNullOrWhiteSpace(essay.AdminContent))
                {
                    body.Append(DocxHelper.CreateSectionHeader("CORRECTIONS", "375623"));

                    var adminText = StripHtml(essay.AdminContent);
                    foreach (var line in SplitIntoLines(adminText))
                        body.Append(DocxHelper.CreateParagraph(line, color: "375623", spaceAfter: 120));
                }
                else
                {
                    body.Append(DocxHelper.CreateSectionHeader("CORRECTIONS", "7F7F7F"));
                    body.Append(DocxHelper.CreateParagraph("No corrections yet.", italic: true, color: "7F7F7F", spaceAfter: 120));
                }

                body.Append(DocxHelper.CreateSectionProperties());

                mainPart.Document.Append(body);
                mainPart.Document.Save();
            }

            return stream.ToArray();
        }

        public byte[] ExportAllReviewedEssaysToZip()
        {
            var reviewedEssays = _dbContext.UserEssays
                .Include(x => x.User)
                .Include(x => x.Module)
                .Where(x => x.IsSubmitted && x.IsReviewed)
                .OrderByDescending(x => x.ReviewedDate)
                .ToList();

            using var zipStream = new MemoryStream();

            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
            {
                foreach (var essay in reviewedEssays)
                {
                    var username = string.Join("_", (essay.User?.Username ?? "student").Split(Path.GetInvalidFileNameChars()));
                    var moduleName = string.Join("_", (essay.Module?.Name ?? "module").Split(Path.GetInvalidFileNameChars()));
                    var fileName = $"essay_{essay.Id}_{username}_{moduleName}.docx";

                    var zipEntry = archive.CreateEntry(fileName, CompressionLevel.Optimal);
                    var docxBytes = ExportEssayToDocx(essay.Id);

                    using var entryStream = zipEntry.Open();
                    entryStream.Write(docxBytes, 0, docxBytes.Length);
                }
            }

            return zipStream.ToArray();
        }

        private static IEnumerable<string> SplitIntoLines(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return [""];

            return text
                .Split('\n')
                .Select(l => l.Trim())
                .Where(l => l.Length > 0)
                .DefaultIfEmpty("—");
        }

        private void CompleteModuleForUser(int userId, int moduleId)
        {
            var direct = _dbContext.UserModuleAssignments
                .FirstOrDefault(x => x.UserId == userId && x.ModuleId == moduleId);

            if (direct is not null && !direct.IsCompleted)
            {
                direct.IsCompleted = true;
                _dbContext.SaveChanges();
            }

            var userMatrixIds = _dbContext.UserMatrixAssignments
                .Where(x => x.UserId == userId)
                .Select(x => x.MatrixId)
                .ToList();

            var matrixModules = _dbContext.MatrixModules
                .Where(x => x.ModuleId == moduleId && userMatrixIds.Contains(x.MatrixId))
                .Select(x => x.Id)
                .ToList();

            foreach (var mmId in matrixModules)
            {
                var alreadyDone = _dbContext.UserMatrixModuleCompletions
                    .Any(x => x.UserId == userId && x.MatrixModuleId == mmId);

                if (!alreadyDone)
                {
                    _dbContext.UserMatrixModuleCompletions.Add(new UserMatrixModuleCompletion
                    {
                        UserId = userId,
                        MatrixModuleId = mmId,
                        CompletedDate = PolandTime.Today
                    });
                }
            }

            _dbContext.SaveChanges();
        }

        private static UserEssayDto MapToDto(UserEssay x, Module module) => new()
        {
            Id = x.Id,
            ModuleId = x.ModuleId,
            ModuleName = module.Name,
            EssayPrompt = module.EssayPrompt ?? string.Empty,
            Content = x.Content,
            AdminContent = x.AdminContent,
            IsSubmitted = x.IsSubmitted,
            IsReviewed = x.IsReviewed,
            SubmittedDate = x.SubmittedDate,
            ReviewedDate = x.ReviewedDate,
            Username = string.Empty
        };

        public List<EssayCommentDto> GetCommentsForEssay(int essayId)
        {
            return _dbContext.EssayComments
                .Include(c => c.Author).ThenInclude(a => a.Profile)
                .Where(c => c.UserEssayId == essayId)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new EssayCommentDto
                {
                    Id = c.Id,
                    NoteId = $"note_{c.Id}",
                    UserEssayId = c.UserEssayId,
                    AuthorId = c.AuthorId,
                    Author = c.Author.Username,
                    AvatarUrl = c.Author.Profile != null ? c.Author.Profile.AvatarUrl : null,
                    SelectedText = c.SelectedText,
                    NoteContent = c.NoteContent,
                    Category = c.Category,
                    IsArchived = c.IsArchived,
                    Timestamp = c.CreatedAt
                })
                .ToList();
        }

        public EssayCommentDto AddComment(int essayId, CreateEssayCommentRequest request)
        {
            var userId = _userContextService.GetUserId!.Value;

            var essay = _dbContext.UserEssays.FirstOrDefault(e => e.Id == essayId)
                ?? throw new NotFoundException("Essay not found");

            var comment = new EssayComment
            {
                UserEssayId = essayId,
                AuthorId = userId,
                SelectedText = request.SelectedText,
                NoteContent = request.NoteContent,
                Category = request.Category,
                IsArchived = false,
                CreatedAt = PolandTime.DateTimeNow
            };

            _dbContext.EssayComments.Add(comment);
            _dbContext.SaveChanges();

            var author = _dbContext.Users.Include(u => u.Profile).FirstOrDefault(u => u.Id == userId);

            return new EssayCommentDto
            {
                Id = comment.Id,
                NoteId = $"note_{comment.Id}",
                UserEssayId = comment.UserEssayId,
                AuthorId = comment.AuthorId,
                Author = author?.Username ?? "User",
                AvatarUrl = author?.Profile?.AvatarUrl,
                SelectedText = comment.SelectedText,
                NoteContent = comment.NoteContent,
                Category = comment.Category,
                IsArchived = comment.IsArchived,
                Timestamp = comment.CreatedAt
            };
        }

        public void ArchiveComment(int commentId)
        {
            var comment = _dbContext.EssayComments.FirstOrDefault(c => c.Id == commentId)
                ?? throw new NotFoundException("Comment not found");

            comment.IsArchived = true;
            _dbContext.SaveChanges();
        }

        private static string StripHtml(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return string.Empty;
            var textWithoutTags = Regex.Replace(html, "<.*?>", string.Empty);
            return System.Net.WebUtility.HtmlDecode(textWithoutTags).Trim();
        }
    }
}