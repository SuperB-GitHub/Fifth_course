using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Text;

namespace Лабораторная_1
{
    public static class Revealer
    {
        public static void Run()
        {
            Console.WriteLine("Укажите путь к файлу .docx:");
            string filePath = StripQuotes(Console.ReadLine());

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Файл не найден: {filePath}");
                return;
            }

            try
            {
                using (WordprocessingDocument wordDoc =
                       WordprocessingDocument.Open(filePath, false))
                {
                    var mainPart = wordDoc.MainDocumentPart;
                    if (mainPart?.Document?.Body == null)
                    {
                        Console.WriteLine("Документ пуст или имеет неверную структуру.");
                        return;
                    }

                    var runs = mainPart.Document.Body
                        .Descendants<Paragraph>()
                        .SelectMany(p => p.Descendants<Run>())
                        .Where(r => !string.IsNullOrEmpty(r.InnerText))
                        .ToList();

                    if (runs.Count == 0)
                    {
                        Console.WriteLine("В документе нет текста.");
                        return;
                    }

                    // Эталон берём с последнего символа документа
                    var refStyle = RunStyleHelper.BuildRefStyle(runs[^1]);

                    var flagsBuilder = new StringBuilder();
                    var allReasons = new HashSet<string>();

                    foreach (var run in runs)
                    {
                        string text = run.InnerText;
                        RunProperties props = run.RunProperties!;

                        string colorStr = RunStyleHelper.GetRunColor(props, run);
                        string bgStr = RunStyleHelper.GetRunBackground(props);
                        double fontSize = RunStyleHelper.GetEffectiveFontSize(run);
                        int scale = RunStyleHelper.GetRunScaleValue(props, run);
                        int spacing = RunStyleHelper.GetRunSpacingValue(props, run);

                        var localReasons = new List<string>();

                        if (!string.Equals(colorStr, refStyle.Color, StringComparison.OrdinalIgnoreCase))
                            localReasons.Add($"цвет = {colorStr}");

                        if (!string.Equals(bgStr, refStyle.Background, StringComparison.OrdinalIgnoreCase))
                            localReasons.Add($"фон = {bgStr}");

                        if (Math.Abs(fontSize - refStyle.FontSize) > 0.001)
                            localReasons.Add($"размер = {fontSize} pt");

                        if (scale != refStyle.Scale)
                            localReasons.Add($"масштаб = {scale}%");

                        if (spacing != refStyle.Spacing)
                            localReasons.Add($"интервал = {spacing} twips");

                        char flag = localReasons.Count > 0 ? '1' : '0';
                        flagsBuilder.Append(flag, text.Length);

                        foreach (var r in localReasons)
                            allReasons.Add(r);
                    }

                    Console.WriteLine($"=== Битовая скрытая строка === (всего битов: {flagsBuilder.Length})");
                    Console.WriteLine(flagsBuilder.ToString());
                    Console.WriteLine();

                    if (allReasons.Count == 0)
                        Console.WriteLine("Текст спрятан из-за: (нет отклонений)");
                    else
                        Console.WriteLine("Текст спрятан из-за: " + string.Join(", ", allReasons));

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

        private static string StripQuotes(string? s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            s = s.Trim();
            if (s.Length >= 2 && s[0] == '"' && s[^1] == '"')
                s = s[1..^1];
            return s;
        }
    }
}