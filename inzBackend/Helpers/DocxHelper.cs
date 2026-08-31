using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace inzBackend.Helpers
{
    public static class DocxHelper
    {
        public static MainDocumentPart InitDocument(WordprocessingDocument doc)
        {
            var mainPart = doc.AddMainDocumentPart();
            mainPart.Document = new Document();

            AddDefaultStylesPart(mainPart);
            AddDocumentSettingsPart(mainPart);

            return mainPart;
        }

        public static SectionProperties CreateSectionProperties()
        {
            var sectionProps = new SectionProperties();

            var pageSize = new PageSize
            {
                Width = 11906U,
                Height = 16838U,
                Orient = PageOrientationValues.Portrait
            };

            var pageMargin = new PageMargin
            {
                Top = 1440,
                Right = 1440,
                Bottom = 1440,
                Left = 1440,
                Header = 720U,
                Footer = 720U,
                Gutter = 0U
            };

            sectionProps.Append(pageSize);
            sectionProps.Append(pageMargin);
            return sectionProps;
        }

        public static Paragraph CreateParagraph(
            string text,
            bool bold = false,
            bool italic = false,
            int fontSize = 24,
            string? color = null,
            int spaceAfter = 100)
        {
            var para = new Paragraph();

            var pPr = new ParagraphProperties();
            pPr.Append(new Justification { Val = JustificationValues.Left });
            pPr.Append(new SpacingBetweenLines { After = spaceAfter.ToString(), Line = "240", LineRule = LineSpacingRuleValues.Auto });
            para.Append(pPr);

            var run = new Run();
            var rPr = new RunProperties();

            rPr.Append(new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman", ComplexScript = "Times New Roman" });
            if (bold) rPr.Append(new Bold());
            if (italic) rPr.Append(new Italic());
            rPr.Append(new FontSize { Val = fontSize.ToString() });
            if (!string.IsNullOrEmpty(color)) rPr.Append(new Color { Val = color });

            run.Append(rPr);
            run.Append(new Text(text ?? string.Empty) { Space = SpaceProcessingModeValues.Preserve });

            para.Append(run);
            return para;
        }

        public static Paragraph CreateSectionHeader(string text, string color)
        {
            var para = new Paragraph();
            var pPr = new ParagraphProperties();
            pPr.Append(new Justification { Val = JustificationValues.Left });

            var pBdr = new ParagraphBorders();
            pBdr.Append(new BottomBorder
            {
                Val = BorderValues.Single,
                Size = 6,
                Space = 2,
                Color = color
            });
            pPr.Append(pBdr);
            pPr.Append(new SpacingBetweenLines { Before = "240", After = "120" });
            para.Append(pPr);

            var run = new Run();
            var rPr = new RunProperties();
            rPr.Append(new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman", ComplexScript = "Times New Roman" });
            rPr.Append(new Bold());
            rPr.Append(new FontSize { Val = "26" });
            rPr.Append(new Color { Val = color });

            run.Append(rPr);
            run.Append(new Text(text ?? string.Empty) { Space = SpaceProcessingModeValues.Preserve });

            para.Append(run);
            return para;
        }

        public static Paragraph CreateDividerParagraph(string color = "CCCCCC")
        {
            var para = new Paragraph();
            var pPr = new ParagraphProperties();
            pPr.Append(new Justification { Val = JustificationValues.Left });

            var pBdr = new ParagraphBorders();
            pBdr.Append(new BottomBorder
            {
                Val = BorderValues.Single,
                Size = 4,
                Space = 1,
                Color = color
            });
            pPr.Append(pBdr);
            pPr.Append(new SpacingBetweenLines { Before = "120", After = "240" });
            para.Append(pPr);

            return para;
        }

        public static Paragraph CreateEmptyLine()
        {
            var para = new Paragraph();
            var pPr = new ParagraphProperties();
            pPr.Append(new Justification { Val = JustificationValues.Left });
            pPr.Append(new SpacingBetweenLines { After = "120" });
            para.Append(pPr);
            return para;
        }

        private static void AddDefaultStylesPart(MainDocumentPart mainPart)
        {
            var styleDefinitionsPart = mainPart.AddNewPart<StyleDefinitionsPart>();
            var styles = new Styles();

            var normalStyle = new Style
            {
                Type = StyleValues.Paragraph,
                StyleId = "Normal",
                Default = true
            };
            normalStyle.Append(new StyleName { Val = "Normal" });
            normalStyle.Append(new PrimaryStyle());

            var pPr = new StyleParagraphProperties();
            pPr.Append(new Justification { Val = JustificationValues.Left });
            pPr.Append(new SpacingBetweenLines { After = "120", Line = "240", LineRule = LineSpacingRuleValues.Auto });
            normalStyle.Append(pPr);

            var rPr = new StyleRunProperties();
            rPr.Append(new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman", ComplexScript = "Times New Roman" });
            rPr.Append(new FontSize { Val = "24" });
            rPr.Append(new Languages { Val = "pl-PL" });
            normalStyle.Append(rPr);

            styles.Append(normalStyle);
            styleDefinitionsPart.Styles = styles;
            styleDefinitionsPart.Styles.Save();
        }

        private static void AddDocumentSettingsPart(MainDocumentPart mainPart)
        {
            var settingsPart = mainPart.AddNewPart<DocumentSettingsPart>();
            var settings = new Settings();

            var compat = new Compatibility();
            compat.Append(new CompatibilitySetting
            {
                Name = CompatSettingNameValues.CompatibilityMode,
                Uri = "http://schemas.microsoft.com/office/word",
                Val = "15"
            });
            settings.Append(compat);

            settingsPart.Settings = settings;
            settingsPart.Settings.Save();
        }
    }
}
