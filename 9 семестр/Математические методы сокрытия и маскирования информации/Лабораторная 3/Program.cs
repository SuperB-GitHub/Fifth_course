using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Text;

namespace Лабораторная_3
{
    static class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            string filePath = RequestFilePath()!;
            if (filePath == null) return;

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("=== Меню ===");
                Console.WriteLine("  1 — Метод по количеству слов (<=3 - 1, >3 - 0)");
                Console.WriteLine("  2 — Метод по первой букве (гласная - 0, согласная - 1)");
                Console.WriteLine("  3 — Выбор нового файла");
                Console.WriteLine("  0 — Выход");
                Console.Write("Ваш выбор: ");
                string choice = Console.ReadLine()?.Trim() ?? "";

                switch (choice)
                {
                    case "0":
                        Console.WriteLine("Выход.");
                        return;

                    case "1":
                    case "2":
                        ProcessDocument(filePath, choice);
                        break;

                    case "3":
                        string newPath = RequestFilePath()!;
                        if (newPath != null)
                            filePath = newPath;
                        break;

                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                        break;
                }
            }
        }

        /// <summary>
        /// Запрашивает путь к .docx и проверяет его.
        /// Возвращает null, если путь пуст или файл не найден.
        /// </summary>
        static string? RequestFilePath()
        {
            Console.WriteLine();
            Console.WriteLine("Укажите путь к файлу .docx:");
            string filePath = StripQuotes(Console.ReadLine());

            if (string.IsNullOrEmpty(filePath))
            {
                Console.WriteLine("Путь не введён.");
                return null;
            }

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Файл не найден: {filePath}");
                return null;
            }

            Console.WriteLine($"Выбран файл: {filePath}");
            return filePath;
        }

        static void ProcessDocument(string filePath, string method)
        {
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

                    var paragraphs = mainPart.Document.Body
                        .Descendants<Paragraph>()
                        .Where(p => !string.IsNullOrWhiteSpace(p.InnerText))
                        .ToList();

                    if (paragraphs.Count == 0)
                    {
                        Console.WriteLine("В документе нет непустых абзацев.");
                        return;
                    }

                    var bits = new StringBuilder();

                    foreach (var p in paragraphs)
                    {
                        string text = p.InnerText.Trim();
                        if (text.Length == 0) continue;

                        int bit;
                        string reason;

                        if (method == "1")
                        {
                            int words = CountWords(text);
                            bit = words <= 3 ? 1 : 0;
                            reason = $"{words}";
                        }
                        else
                        {
                            char first = FirstLetter(text);
                            bool isVowel = IsVowel(first);
                            bit = isVowel ? 0 : 1;
                            reason = $"{first}";
                        }

                        string answer = bit == 1 ? "ДА " : "НЕТ";
                        string line = $"{answer} - [{reason}] - {bit} - {text}";
                        bits.Append(bit);

                        Console.WriteLine(line);
                    }

                    Console.WriteLine();
                    Console.WriteLine("Битовая последовательность:");
                    Console.WriteLine(bits.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении документа: {ex.Message}");
            }
        }

        private static int CountWords(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return 0;
            return text.Split(
                [' ', '\t', '\u00A0'],
                StringSplitOptions.RemoveEmptyEntries).Length;
        }

        private static char FirstLetter(string text)
        {
            foreach (char c in text)
            {
                if (char.IsLetter(c))
                    return char.ToLowerInvariant(c);
            }
            return '\0';
        }

        private static bool IsVowel(char c)
        {
            const string vowels = "аеёиоуыэюяaeiouy";
            return vowels.IndexOf(c) >= 0;
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