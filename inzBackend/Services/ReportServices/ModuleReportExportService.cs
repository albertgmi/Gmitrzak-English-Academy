using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using OpenXmlDocument = DocumentFormat.OpenXml.Wordprocessing.Document;
using QDocument = QuestPDF.Fluent.Document;
using inzBackend.Models.ModuleReportModels;

namespace inzBackend.Services.ReportServices
{
    public class ModuleReportExportService : IModuleReportExportService
    {
        public byte[] GeneratePdf(ModuleReportDto report)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            return QDocument.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);

                    page.DefaultTextStyle(x =>
                        x.FontSize(11)
                         .FontFamily("Arial"));

                    page.Header().Column(col =>
                    {
                        col.Item()
                            .Text("Sentence Module Report")
                            .FontSize(20)
                            .Bold()
                            .FontColor(Colors.Blue.Darken2);

                        col.Item()
                            .Text($"Student: {report.StudentUsername}")
                            .FontSize(13)
                            .SemiBold();

                        col.Item()
                            .Text($"Module: {report.ModuleName}")
                            .FontSize(12);

                        col.Item()
                            .Text($"Generated: {report.GeneratedDate:d MMM yyyy}")
                            .FontSize(10)
                            .FontColor(Colors.Grey.Darken1);

                        col.Item()
                            .PaddingVertical(8)
                            .LineHorizontal(1)
                            .LineColor(Colors.Grey.Lighten1);
                    });

                    page.Content().Column(col =>
                    {
                        col.Item()
                            .PaddingBottom(12)
                            .Row(row =>
                            {
                                row.Spacing(8);

                                BuildSummaryBox(
                                    row,
                                    report.CorrectCount.ToString(),
                                    "Correct",
                                    Colors.Green.Lighten2,
                                    Colors.Green.Lighten4,
                                    Colors.Green.Darken2);

                                BuildSummaryBox(
                                    row,
                                    report.PartialCount.ToString(),
                                    "Partial",
                                    Colors.Yellow.Lighten2,
                                    Colors.Yellow.Lighten4,
                                    Colors.Yellow.Darken3);

                                BuildSummaryBox(
                                    row,
                                    report.IncorrectCount.ToString(),
                                    "Incorrect",
                                    Colors.Red.Lighten2,
                                    Colors.Red.Lighten4,
                                    Colors.Red.Darken2);
                            });

                        foreach (var item in report.Items)
                        {
                            var borderColor =
                                item.FinalResult == "Correct"
                                    ? Colors.Green.Lighten2
                                    : item.FinalResult == "Partial"
                                        ? Colors.Yellow.Lighten2
                                        : Colors.Red.Lighten2;

                            var bgColor =
                                item.FinalResult == "Correct"
                                    ? Colors.Green.Lighten5
                                    : item.FinalResult == "Partial"
                                        ? Colors.Yellow.Lighten5
                                        : Colors.Red.Lighten5;

                            col.Item()
                                .PaddingBottom(8)
                                .Border(1)
                                .BorderColor(borderColor)
                                .Background(bgColor)
                                .Padding(10)
                                .Column(c =>
                                {
                                    c.Item().Row(r =>
                                    {
                                        r.RelativeItem()
                                            .Text($"#{item.Order} {item.Polish}")
                                            .Bold()
                                            .FontSize(12);

                                        r.ConstantItem(80)
                                            .AlignRight()
                                            .Text(item.FinalResult)
                                            .Bold();
                                    });

                                    c.Item()
                                        .PaddingTop(4)
                                        .Text($"Expected: {item.ExpectedTranslation}")
                                        .Italic();

                                    c.Item()
                                        .PaddingTop(2)
                                        .Text($"Student: {item.StudentAnswer}");

                                    c.Item()
                                        .PaddingTop(4)
                                        .Text($"AI: {item.AiExplanation}")
                                        .FontSize(9)
                                        .Italic();

                                    if (!string.IsNullOrWhiteSpace(item.TeacherOverride))
                                    {
                                        c.Item()
                                            .PaddingTop(4)
                                            .Background(Colors.Blue.Lighten4)
                                            .Padding(6)
                                            .Column(tc =>
                                            {
                                                tc.Item()
                                                    .Text($"Teacher override: {item.TeacherOverride}")
                                                    .Bold();

                                                if (!string.IsNullOrWhiteSpace(item.TeacherExplanation))
                                                {
                                                    tc.Item()
                                                        .Text(item.TeacherExplanation)
                                                        .Italic();
                                                }
                                            });
                                    }
                                });
                        }
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                            x.Span(" of ");
                            x.TotalPages();
                        });
                });
            }).GeneratePdf();
        }
        public byte[] GenerateDocx(ModuleReportDto report)
        {
            using var stream = new MemoryStream();

            using (var document = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document, true))
            {
                var mainPart = document.AddMainDocumentPart();
                mainPart.Document = new OpenXmlDocument();
                var body = new Body();

                body.Append(CreateHeading("Sentence Module Report", 32));
                body.Append(CreateParagraph($"Student: {report.StudentUsername}", true));
                body.Append(CreateParagraph($"Module: {report.ModuleName}", true));
                body.Append(CreateParagraph($"Generated: {report.GeneratedDate:d MMM yyyy}"));
                body.Append(CreateParagraph(" "));

                int totalIncorrect = report.IncorrectCount + report.PartialCount;
                body.Append(CreateParagraph($"Correct: {report.CorrectCount} | Incorrect: {totalIncorrect}", true));
                body.Append(CreateParagraph(" "));

                foreach (var item in report.Items)
                {
                    string displayResult = item.FinalResult == "Correct" ? "Correct" : "Incorrect";
                    body.Append(CreateCustomParagraph(displayResult, bold: false, italic: true, fontSize: 18));

                    body.Append(CreateCustomParagraph($"#{item.Order} {item.Polish}", bold: true, italic: false, fontSize: 28));

                    if (!string.IsNullOrWhiteSpace(item.StudentAnswer))
                        body.Append(CreateParagraph($"Students translation: {item.StudentAnswer}"));

                    if (!string.IsNullOrWhiteSpace(item.ExpectedTranslation))
                        body.Append(CreateParagraph($"Expected translation: {item.ExpectedTranslation}"));

                    body.Append(CreateParagraph(" "));
                }

                mainPart.Document.Append(body);
                mainPart.Document.Save();
            }

            return stream.ToArray();
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