using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using OpenXmlDocument = DocumentFormat.OpenXml.Wordprocessing.Document;
using QDocument = QuestPDF.Fluent.Document;
using inzBackend.Models.ModuleReportModels;
using System.IO.Compression;
using inzBackend.Exceptions;
using Microsoft.EntityFrameworkCore;
using inzBackend.Models;
using inzBackend.Services.UserAnswerServices;
using inzBackend.Enums;

namespace inzBackend.Services.ReportServices
{
    public class ModuleReportExportService : IModuleReportExportService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IUserAnswerService _userAnswerService;
        public ModuleReportExportService(GmitrzakEnglishAcademyDbContext dbContext, IUserAnswerService userAnswerService)
        {
            _dbContext = dbContext;
            _userAnswerService = userAnswerService;
        }

        public byte[] generateRangePdf(DateRangeReportDto report)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            return QDocument.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    page.Header().Column(col =>
                    {
                        col.Item().Text("Sentence Modules Report — Period Summary")
                            .FontSize(18).Bold().FontColor(Colors.Blue.Darken2);
                        col.Item().Text($"Student: {report.StudentUsername}")
                            .FontSize(13).SemiBold();
                        col.Item().Text(
                            $"Period: {report.DateFrom:d MMM yyyy} — {report.DateTo:d MMM yyyy}")
                            .FontSize(11);
                        col.Item().Text($"Generated: {report.GeneratedDate:d MMM yyyy}")
                            .FontSize(10).FontColor(Colors.Grey.Darken1);
                        col.Item().PaddingVertical(8)
                            .LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                    });

                    page.Content().Column(col =>
                    {
                        col.Item().PaddingBottom(12).Row(row =>
                        {
                            row.Spacing(8);
                            BuildSummaryBox(row, report.TotalSentences.ToString(),
                                "Total", Colors.Blue.Lighten2, Colors.Blue.Lighten4, Colors.Blue.Darken2);
                            BuildSummaryBox(row, report.TotalCorrect.ToString(),
                                "Correct", Colors.Green.Lighten2, Colors.Green.Lighten4, Colors.Green.Darken2);
                            BuildSummaryBox(row, report.TotalPartial.ToString(),
                                "Partial", Colors.Yellow.Lighten2, Colors.Yellow.Lighten4, Colors.Yellow.Darken3);
                            BuildSummaryBox(row, report.TotalIncorrect.ToString(),
                                "Incorrect", Colors.Red.Lighten2, Colors.Red.Lighten4, Colors.Red.Darken2);
                        });

                        foreach (var module in report.Modules)
                        {
                            col.Item().PaddingBottom(4).Text(module.ModuleName)
                                .FontSize(14).Bold().FontColor(Colors.Blue.Darken1);

                            col.Item().PaddingBottom(8).Row(row =>
                            {
                                row.Spacing(6);
                                BuildSummaryBox(row, module.CorrectCount.ToString(),
                                    "Correct", Colors.Green.Lighten2, Colors.Green.Lighten4, Colors.Green.Darken2);
                                BuildSummaryBox(row, module.PartialCount.ToString(),
                                    "Partial", Colors.Yellow.Lighten2, Colors.Yellow.Lighten4, Colors.Yellow.Darken3);
                                BuildSummaryBox(row, module.IncorrectCount.ToString(),
                                    "Incorrect", Colors.Red.Lighten2, Colors.Red.Lighten4, Colors.Red.Darken2);
                            });

                            foreach (var item in module.Items)
                            {
                                var borderColor = item.FinalResult == "Correct" ? Colors.Green.Lighten2
                                    : item.FinalResult == "Partial" ? Colors.Yellow.Lighten2
                                    : Colors.Red.Lighten2;
                                var bgColor = item.FinalResult == "Correct" ? Colors.Green.Lighten5
                                    : item.FinalResult == "Partial" ? Colors.Yellow.Lighten5
                                    : Colors.Red.Lighten5;

                                col.Item().PaddingBottom(6)
                                    .Border(1).BorderColor(borderColor)
                                    .Background(bgColor).Padding(8).Column(c =>
                                    {
                                        c.Item().Row(r =>
                                        {
                                            r.RelativeItem().Text($"#{item.Order} {item.Polish}")
                                                .Bold().FontSize(11);
                                            r.ConstantItem(70).AlignRight()
                                                .Text(item.FinalResult).Bold();
                                        });
                                        c.Item().PaddingTop(3)
                                            .Text($"Expected: {item.ExpectedTranslation}").Italic();
                                        c.Item().PaddingTop(2)
                                            .Text($"Student: {item.StudentAnswer}");
                                        c.Item().PaddingTop(3)
                                            .Text($"AI: {item.AiExplanation}")
                                            .FontSize(9).Italic();

                                        if (!string.IsNullOrWhiteSpace(item.TeacherOverride))
                                        {
                                            c.Item().PaddingTop(4)
                                                .Background(Colors.Blue.Lighten4).Padding(5).Column(tc =>
                                                {
                                                    tc.Item().Text($"Teacher: {item.TeacherOverride}").Bold();
                                                    if (!string.IsNullOrWhiteSpace(item.TeacherExplanation))
                                                        tc.Item().Text(item.TeacherExplanation).Italic();
                                                });
                                        }
                                    });
                            }

                            col.Item().PaddingVertical(8)
                                .LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten1);
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Page "); x.CurrentPageNumber();
                        x.Span(" of "); x.TotalPages();
                    });
                });
            }).GeneratePdf();
        }

        public byte[] generateRangeDocx(DateRangeReportDto report)
        {
            using var stream = new MemoryStream();

            using (var document = WordprocessingDocument.Create(
                stream, WordprocessingDocumentType.Document, true))
            {
                var mainPart = document.AddMainDocumentPart();
                mainPart.Document = new OpenXmlDocument();
                var body = new Body();

                body.Append(CreateHeading("Sentence Modules Report — Period Summary", 32));
                body.Append(CreateParagraph($"Student: {report.StudentUsername}", true));
                body.Append(CreateParagraph(
                    $"Period: {report.DateFrom:d MMM yyyy} — {report.DateTo:d MMM yyyy}", true));
                body.Append(CreateParagraph($"Generated: {report.GeneratedDate:d MMM yyyy}"));
                body.Append(CreateParagraph(" "));
                body.Append(CreateParagraph(
                    $"Total: {report.TotalSentences} | Correct: {report.TotalCorrect} " +
                    $"| Partial: {report.TotalPartial} | Incorrect: {report.TotalIncorrect}", true));
                body.Append(CreateParagraph(" "));

                foreach (var module in report.Modules)
                {
                    body.Append(CreateHeading($"Module: {module.ModuleName}", 26));
                    body.Append(CreateParagraph(
                        $"Correct: {module.CorrectCount} | Partial: {module.PartialCount} " +
                        $"| Incorrect: {module.IncorrectCount}"));
                    body.Append(CreateParagraph(" "));

                    foreach (var item in module.Items)
                    {
                        var result = item.FinalResult == "Correct" ? "Correct" : "Incorrect";
                        body.Append(CreateCustomParagraph(result, bold: false, italic: true, fontSize: 18));
                        body.Append(CreateCustomParagraph(
                            $"#{item.Order} {item.Polish}", bold: true, italic: false, fontSize: 24));
                        if (!string.IsNullOrWhiteSpace(item.StudentAnswer))
                            body.Append(CreateParagraph($"Student: {item.StudentAnswer}"));
                        if (!string.IsNullOrWhiteSpace(item.ExpectedTranslation))
                            body.Append(CreateParagraph($"Expected: {item.ExpectedTranslation}"));
                        if (!string.IsNullOrWhiteSpace(item.TeacherOverride))
                            body.Append(CreateParagraph(
                                $"Teachers explanation: {item.TeacherOverride}" +
                                (string.IsNullOrWhiteSpace(item.TeacherExplanation)
                                    ? "" : $" — {item.TeacherExplanation}"), true));
                        body.Append(CreateParagraph(" "));
                    }

                    body.Append(CreateParagraph("─────────────────────────────────────────────────────────────"));
                    body.Append(CreateParagraph(" "));
                }

                mainPart.Document.Append(body);
                mainPart.Document.Save();
            }

            return stream.ToArray();
        }

        public byte[] generateActiveStudentsZipReport(DateOnly dateFrom, DateOnly dateTo)
        {
            var activeStudentIds = _dbContext
                .Users
                .Where(u => u.IsActive == true && u.Role == UserRole.User)
                .Select(u => u.Id)
                .ToList();

            var reports = new List<DateRangeReportDto>();

            foreach (var studentId in activeStudentIds)
            {
                try
                {
                    var report = _userAnswerService.generateDateRangeReport(studentId, dateFrom, dateTo);

                    if (report.Modules != null && report.Modules.Any())
                    {
                        reports.Add(report);
                    }
                }
                catch (NotFoundException)
                {
                    continue;
                }
            }

            return generateActiveStudentsDocxZip(reports);
        }

        private byte[] generateActiveStudentsDocxZip(IEnumerable<DateRangeReportDto> reports)
        {
            using var zipStream = new MemoryStream();

            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
            {
                foreach (var report in reports)
                {
                    var sanitizedUsername = string.Join("_", report.StudentUsername.Split(Path.GetInvalidFileNameChars()));
                    var fileName = $"Raport_{sanitizedUsername}_{report.DateFrom:yyyyMMdd}-{report.DateTo:yyyyMMdd}.docx";

                    var zipEntry = archive.CreateEntry(fileName, CompressionLevel.Optimal);

                    var docxBytes = generateRangeDocx(report);

                    using var entryStream = zipEntry.Open();
                    entryStream.Write(docxBytes, 0, docxBytes.Length);
                }
            }

            return zipStream.ToArray();
        }

        private static void BuildSummaryBox(RowDescriptor row, string value, string label, string borderColor, string bgColor, string textColor)
        {
            row.RelativeItem()
                .Border(1)
                .BorderColor(borderColor)
                .Background(bgColor)
                .Padding(8)
                .Column(c =>
                {
                    c.Item()
                        .AlignCenter()
                        .Text(value)
                        .FontSize(22)
                        .Bold()
                        .FontColor(textColor);

                    c.Item()
                        .AlignCenter()
                        .Text(label)
                        .FontColor(textColor);
                });
        }

        private static Paragraph CreateParagraph(string text, bool bold = false)
        {
            var run = new Run();
            if (bold)
                run.Append(new RunProperties(new Bold()));
            run.Append(new Text(text));
            return new Paragraph(run);
        }

        private static Paragraph CreateHeading(string text, int size)
        {
            return new Paragraph(
                new Run(
                    new RunProperties(
                        new Bold(),
                        new FontSize { Val = size.ToString() }
                    ),
                    new Text(text)));
        }

        private static Paragraph CreateCustomParagraph(string text, bool bold = false, bool italic = false, int? fontSize = null)
        {
            var run = new Run();
            var runProperties = new RunProperties();

            if (bold)
                runProperties.Append(new Bold());

            if (italic)
                runProperties.Append(new Italic());

            if (fontSize.HasValue)
                runProperties.Append(new FontSize { Val = fontSize.Value.ToString() });

            if (bold || italic || fontSize.HasValue)
                run.Append(runProperties);

            run.Append(new Text(text));
            return new Paragraph(run);
        }
    }
}