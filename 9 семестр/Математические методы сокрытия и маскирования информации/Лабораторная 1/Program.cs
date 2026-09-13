using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Text;

namespace DocxInspector
{
    class Program
    {
        // --- Эталонные значения ---
        private const string RefColor = "000000";
        private const string RefBackground = "Прозрачный";
        private const double RefFontSize = 12.0;
        private const int RefScale = 100;
        private const int RefSpacing = 0;

        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("Укажите путь к файлу .docx:");
            string filePath = Console.ReadLine()![1..^1];

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Файл не найден: {filePath}");
                return;
            }

            try
            {
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, false))
                {
                    var mainPart = wordDoc.MainDocumentPart;
                    if (mainPart?.Document?.Body == null)
                    {
                        Console.WriteLine("Документ пуст или имеет неверную структуру.");
                        return;
                    }

                    var flagsBuilder = new StringBuilder();
                    var allReasons = new HashSet<string>();   // уникальные причины по всему документу

                    foreach (var paragraph in mainPart.Document.Body.Descendants<Paragraph>())
                    {
                        foreach (var run in paragraph.Descendants<Run>())
                        {
                            string text = run.InnerText;
                            if (string.IsNullOrEmpty(text)) continue;

                            RunProperties props = run.RunProperties!;

                            string colorStr = GetRunColor(props, run);
                            string bgStr = GetRunBackground(props);
                            double fontSize = GetEffectiveFontSize(run);
                            int scale = GetRunScaleValue(props, run);
                            int spacing = GetRunSpacingValue(props, run);

                            var localReasons = new List<string>();

                            if (!string.Equals(colorStr, RefColor, StringComparison.OrdinalIgnoreCase))
                                localReasons.Add($"цвет = {colorStr}");

                            if (!string.Equals(bgStr, RefBackground, StringComparison.OrdinalIgnoreCase))
                                localReasons.Add($"фон = {bgStr}");

                            if (Math.Abs(fontSize - RefFontSize) > 0.001)
                                localReasons.Add($"размер = {fontSize} pt");

                            if (scale != RefScale)
                                localReasons.Add($"масштаб = {scale}%");

                            if (spacing != RefSpacing)
                                localReasons.Add($"интервал = {spacing} twips");

                            char flag = localReasons.Count > 0 ? '1' : '0';
                            flagsBuilder.Append(flag, text.Length);

                            foreach (var r in localReasons)
                                allReasons.Add(r);
                        }
                    }

                    Console.WriteLine($"=== Битовая скрытая строка === (всего битов: {flagsBuilder.Length})");
                    Console.WriteLine(flagsBuilder.ToString());
                    Console.WriteLine();

                    // Одно итоговое предложение
                    if (allReasons.Count == 0)
                    {
                        Console.WriteLine("Текст спрятан из-за: (нет отклонений)");
                    }
                    else
                    {
                        Console.WriteLine("Текст спрятан из-за: " + string.Join(", ", allReasons));
                    }

                    string bits = flagsBuilder.ToString();

                    Console.WriteLine();
                    Console.WriteLine("=== Код Бодо (МТК-2) ===");
                    Console.WriteLine(Encodings.DecodeMtk2(bits));

                    Console.WriteLine();
                    Console.WriteLine("=== КОИ-8R ===");
                    Console.WriteLine(Encodings.DecodeKoi8r(bits));

                    Console.WriteLine();
                    Console.WriteLine("=== CP866 ===");
                    Console.WriteLine(Encodings.DecodeCp866(bits));

                    Console.WriteLine();
                    Console.WriteLine("=== Windows-1251 ===");
                    Console.WriteLine(Encodings.DecodeWin1251(bits));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении документа: {ex.Message}");
            }
        }

        // --- Вспомогательные методы (строковые, для отчёта) ---

        private static string GetRunColor(RunProperties props, Run run)
        {
            var colorElement = props?.Color ?? GetStyleProperty<Color>(run);
            return colorElement?.Val?.Value ?? "Авто (черный по умолчанию)";
        }

        private static string GetRunBackground(RunProperties props)
        {
            var shading = props?.Shading;
            if (shading?.Fill != null && !string.IsNullOrEmpty(shading.Fill.Value))
            {
                string fill = shading.Fill.Value;
                // "auto" / "FFFFFF" / белый = фактически прозрачный
                if (!fill.Equals("auto", StringComparison.OrdinalIgnoreCase) &&
                    !fill.Equals("FFFFFF", StringComparison.OrdinalIgnoreCase))
                {
                    return $"Заливка: #{fill}";
                }
            }

            var highlight = props?.Highlight;
            if (highlight?.Val != null)
            {
                var val = highlight.Val.Value;
                // white / none = визуально прозрачный
                if (val != HighlightColorValues.White && val != HighlightColorValues.None)
                    return $"Маркер: {val}";
            }

            return "Прозрачный";
        }


        // --- Числовые методы (для сравнения) ---

        private static int GetRunScaleValue(RunProperties props, Run run)
        {
            var el = props?.CharacterScale ?? GetStyleProperty<CharacterScale>(run);
            return (int)(el?.Val?.Value ?? 100);
        }

        private static int GetRunSpacingValue(RunProperties props, Run run)
        {
            var el = props?.Spacing ?? GetStyleProperty<Spacing>(run);
            return el?.Val?.Value ?? 0;
        }

        private static double GetEffectiveFontSize(Run run)
        {
            double direct = 12.0;
            var directEl = run.RunProperties?.FontSize;
            if (directEl?.Val?.Value != null &&
                double.TryParse(directEl.Val.Value, out double h1))
            {
                direct = h1 / 2.0;
            }

            double fromStyle = double.MaxValue;
            var styleEl = GetStyleProperty<FontSize>(run);
            if (styleEl?.Val?.Value != null &&
                double.TryParse(styleEl.Val.Value, out double h2))
            {
                fromStyle = h2 / 2.0;
            }

            return Math.Min(direct, fromStyle);
        }

        // --- Поиск свойства в стиле ---

        private static T GetStyleProperty<T>(Run run) where T : OpenXmlElement
        {
            var styleId = run.RunProperties?.RunStyle?.Val?.Value;
            if (string.IsNullOrEmpty(styleId)) return null!;

            var mainPart = run.Ancestors<Document>().FirstOrDefault()?.MainDocumentPart;
            var stylePart = mainPart?.StyleDefinitionsPart;
            if (stylePart?.Styles == null) return null!;

            var style = stylePart.Styles.Elements<Style>()
                .FirstOrDefault(s => s.StyleId == styleId);

            if (style == null) return null!;

            var styleProps = style.StyleRunProperties;
            if (styleProps == null) return null!;

            return styleProps.Elements<T>().FirstOrDefault()!;
        }
    }
}