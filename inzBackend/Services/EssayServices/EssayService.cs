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
using OpenXmlDocument = DocumentFormat.OpenXml.Wordprocessing.Document;
using inzBackend.Entities.Curriculum;
using inzBackend.Entities.Assignments;

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

            var existing = _dbContext.UserEssays
                .FirstOrDefault(x => x.UserId == userId && x.ModuleId == moduleId);

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

            var alreadySubmitted = _dbContext.UserEssays
                .Any(x => x.UserId == userId
                       && x.ModuleId == request.ModuleId
                       && x.IsSubmitted);

            if (alreadySubmitted)
                throw new BadRequestException("Essay already submitted.");

            var module = _dbContext.Modules
                .FirstOrDefault(x => x.Id == request.ModuleId)
                ?? throw new NotFoundException("Module not found");

            var existing = _dbContext.UserEssays
                .FirstOrDefault(x => x.UserId == userId && x.ModuleId == request.ModuleId);

            if (existing is not null)
            {
                existing.Content = request.Content;
                existing.IsSubmitted = true;
                existing.SubmittedDate = today;
            }
            else
            {
                existing = new UserEssay
                {
                    UserId = userId,
                    ModuleId = request.ModuleId,
                    Content = request.Content,
                    IsSubmitted = true,
                    SubmittedDate = today
                };
                _dbContext.UserEssays.Add(existing);
            }

            _dbContext.SaveChanges();

            CompleteModuleForUser(userId, request.ModuleId);

            _lessonPanelService.AddActivityPoints(
                userId, 15, $"Essay submitted: {module.Name}");

            return MapToDto(existing, module);
        }

        public List<UserEssayDto> GetAllEssaysForAdmin()
        {
            return _dbContext.UserEssays
                .Include(x => x.User)
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
                    Username = x.User.Username
                })
                .ToList();
        }

        public List<UserEssayDto> GetEssaysForStudent(int studentId)
        {
            return _dbContext.UserEssays
                .Include(x => x.User)
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
                    Username = x.User.Username
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
                var mainPart = doc.AddMainDocumentPart();
                mainPart.Document = new OpenXmlDocument();
                var body = new Body();

                body.Append(CreateParagraph("Essay Review", bold: true, fontSize: 32));
                body.Append(CreateParagraph($"Student: {essay.User.Username}", bold: true));
                body.Append(CreateParagraph($"Module: {essay.Module.Name}"));
                body.Append(CreateParagraph(
                    $"Submitted: {essay.SubmittedDate?.ToString("d MMM yyyy") ?? "—"}"));
                body.Append(CreateEmptyLine());

                body.Append(CreateParagraph("Essay Prompt", bold: true, fontSize: 24,
                    color: "2E74B5"));
                body.Append(CreateParagraph(essay.Module.EssayPrompt ?? string.Empty,
                    italic: true));
                body.Append(CreateEmptyLine());

                body.Append(CreateSectionHeader("STUDENT'S ESSAY", "2E74B5"));

                var studentText = StripHtml(essay.Content);
                foreach (var line in SplitIntoLines(studentText))
                    body.Append(CreateParagraph(line));

                body.Append(CreateEmptyLine());

                if (!string.IsNullOrWhiteSpace(essay.AdminContent))
                {
                    body.Append(CreateSectionHeader("TEACHER'S CORRECTIONS", "375623"));

                    var adminText = StripHtml(essay.AdminContent);
                    foreach (var line in SplitIntoLines(adminText))
                        body.Append(CreateParagraph(line, color: "375623"));
                }
                else
                {
                    body.Append(CreateSectionHeader("TEACHER'S CORRECTIONS", "7F7F7F"));
                    body.Append(CreateParagraph("No corrections yet.", italic: true,
                        color: "7F7F7F"));
                }

                mainPart.Document.Append(body);
                mainPart.Document.Save();
            }

            return stream.ToArray();
        }
        private static Paragraph CreateParagraph(
            string text,
            bool bold = false,
            bool italic = false,
            int fontSize = 22,
            string? color = null)
        {
            var run = new Run();
            var props = new RunProperties();

            if (bold) props.Append(new Bold());
            if (italic) props.Append(new Italic());
            if (fontSize != 22) props.Append(new FontSize { Val = fontSize.ToString() });
            if (color is not null) props.Append(new Color { Val = color });

            if (props.HasChildren) run.Append(props);
            run.Append(new Text(text) { Space = SpaceProcessingModeValues.Preserve });

            return new Paragraph(run);
        }

        private static Paragraph CreateSectionHeader(string text, string color)
        {
            var run = new Run();
            var props = new RunProperties();
            props.Append(new Bold());
            props.Append(new FontSize { Val = "26" });
            props.Append(new Color { Val = color });
            run.Append(props);
            run.Append(new Text(text));

            var para = new Paragraph(run);
            var pPr = new ParagraphProperties();
            var pBdr = new ParagraphBorders();

            pBdr.Append(new BottomBorder
            {
                Val = BorderValues.Single,
                Size = 6,
                Space = 1,
                Color = color
            });
            pPr.Append(pBdr);
            pPr.Append(new SpacingBetweenLines
            { Before = "200", After = "120" });
            para.InsertAt(pPr, 0);

            return para;
        }

        private static Paragraph CreateEmptyLine() => new Paragraph(new Run(new Text(string.Empty)));

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
                return;
            }

            var userMatrixIds = _dbContext.UserMatrixAssignments
                .Where(x => x.UserId == userId)
                .Select(x => x.MatrixId)
                .ToList();

            var mm = _dbContext.MatrixModules
                .FirstOrDefault(x => x.ModuleId == moduleId
                                  && userMatrixIds.Contains(x.MatrixId));

            if (mm is null) return;

            var alreadyDone = _dbContext.UserMatrixModuleCompletions
                .Any(x => x.UserId == userId && x.MatrixModuleId == mm.Id);

            if (!alreadyDone)
            {
                _dbContext.UserMatrixModuleCompletions.Add(new UserMatrixModuleCompletion
                {
                    UserId = userId,
                    MatrixModuleId = mm.Id,
                    CompletedDate = PolandTime.Today
                });
                _dbContext.SaveChanges();
            }
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

        private static string StripHtml(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return string.Empty;
            return Regex.Replace(html, "<.*?>", string.Empty)
                        .Replace("&nbsp;", " ")
                        .Replace("&amp;", "&")
                        .Replace("&lt;", "<")
                        .Replace("&gt;", ">")
                        .Trim();
        }
    }
}