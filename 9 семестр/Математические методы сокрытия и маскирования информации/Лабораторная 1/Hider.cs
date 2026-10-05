using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Text;

namespace Лабораторная_1
{
    public static class Hider
    {
        private const string RefColor = "000000";
        private const double RefFontSize = 12.0;
        private const int RefScale = 100;
        private const int RefSpacing = 0;

        private const string DeviantColor = "FF0000";
        private const string DeviantShading = "DDDDDD";
        private const int DeviantFontSizeHalfPoints = 28;
        private const int DeviantScale = 120;
        private const int DeviantSpacing = 20;

        public static void Run()
        {
            Console.WriteLine("Укажите путь к исходному .docx (шаблону):");
            string templatePath = StripQuotes(Console.ReadLine());

            if (!File.Exists(templatePath))
            {
                Console.WriteLine($"Файл не найден: {templatePath}");
                return;
            }

            Console.WriteLine("Введите сообщение для сокрытия:");
            string message = Console.ReadLine() ?? "";

            Console.WriteLine("Выберите кодировку для преобразования в биты:");
            Console.WriteLine("  1 — МТК-2 (5 бит)");
            Console.WriteLine("  2 — КОИ-8R (8 бит)");
            Console.WriteLine("  3 — CP866 (8 бит)");
            Console.WriteLine("  4 — Windows-1251 (8 бит)");
            Console.Write("Ваш выбор: ");
            string encChoice = Console.ReadLine()?.Trim() ?? "1";

            Console.WriteLine("Выберите формат сокрытия (одно свойство):");
            Console.WriteLine("  1 — Цвет текста");
            Console.WriteLine("  2 — Фон (заливка)");
            Console.WriteLine("  3 — Размер шрифта");
            Console.WriteLine("  4 — Масштаб символов");
            Console.WriteLine("  5 — Интервал между символами");
            Console.Write("Ваш выбор: ");
            string hideChoice = Console.ReadLine()?.Trim() ?? "1";

            string bits = EncodeToBits(message, encChoice);

            if (bits.Length == 0)
            {
                Console.WriteLine("Не удалось получить битовую строку.");
                return;
            }

            Console.WriteLine($"Бит для сокрытия: {bits.Length}");

            Console.WriteLine("Укажите путь для сохранения результата .docx:");
            string outputPath = StripQuotes(Console.ReadLine());

            try
            {
                File.Copy(templatePath, outputPath, overwrite: true);

                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(outputPath, true))
                {
                    var mainPart = wordDoc.MainDocumentPart;
                    if (mainPart?.Document?.Body == null)
                    {
                        Console.WriteLine("Документ пуст или имеет неверную структуру.");
                        return;
                    }

                    SplitRunsByCharacters(mainPart.Document.Body);

                    var runs = mainPart.Document.Body
                        .Descendants<Paragraph>()
                        .SelectMany(p => p.Descendants<Run>())
                        .Where(r => !string.IsNullOrEmpty(r.InnerText))
                        .ToList();

                    int totalChars = runs.Count;
                    if (totalChars < bits.Length)
                    {
                        Console.WriteLine(
                            $"В документе {totalChars} символов, а нужно {bits.Length}. " +
                            "Недостаточно места для сокрытия.");
                        return;
                    }

                    for (int i = 0; i < bits.Length; i++)
                    {
                        var run = runs[i];
                        EnsureRunProperties(run);
                        ApplyBitToRun(run, bits[i], hideChoice);
                    }

                    mainPart.Document.Save();
                }

                Console.WriteLine($"Готово. Результат сохранён: {outputPath}");
                Console.WriteLine($"Скрыто битов: {bits.Length}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сокрытии: {ex.Message}");
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

        private static void EnsureRunProperties(Run run)
        {
            if (run.RunProperties == null)
                run.RunProperties = new RunProperties();
        }

        private static void ApplyBitToRun(Run run, char bit, string hideChoice)
        {
            var props = run.RunProperties!;

            bool deviate = bit == '1';

            switch (hideChoice)
            {
                case "1": // Цвет
                    props.RemoveAllChildren<Color>();
                    props.Append(new Color
                    {
                        Val = deviate ? DeviantColor : RefColor
                    });
                    break;

                case "2": // Фон (заливка)
                    props.RemoveAllChildren<Shading>();
                    props.Append(new Shading
                    {
                        Fill = deviate ? DeviantShading : "FFFFFF"
                    });
                    break;

                case "3": // Размер шрифта
                    props.RemoveAllChildren<FontSize>();
                    props.Append(new FontSize
                    {
                        Val = deviate
                            ? DeviantFontSizeHalfPoints.ToString()
                            : "24"
                    });
                    break;

                case "4": // Масштаб
                    props.RemoveAllChildren<CharacterScale>();
                    props.Append(new CharacterScale
                    {
                        Val = deviate ? DeviantScale : RefScale
                    });
                    break;

                case "5": // Интервал
                    props.RemoveAllChildren<Spacing>();
                    props.Append(new Spacing
                    {
                        Val = deviate ? DeviantSpacing : RefSpacing
                    });
                    break;

                default:
                    // По умолчанию — цвет
                    props.RemoveAllChildren<Color>();
                    props.Append(new Color
                    {
                        Val = deviate ? DeviantColor : RefColor
                    });
                    break;
            }
        }

        private static void SplitRunsByCharacters(Body body)
        {
            // Сначала собираем все Run в список (чтобы не менять коллекцию во время обхода)
            var allRuns = body.Descendants<Run>().ToList();

            foreach (var run in allRuns)
            {
                string text = run.InnerText;
                if (string.IsNullOrEmpty(text) || text.Length == 1)
                    continue; // уже один символ — ничего не делаем

                // Родитель — обычно Paragraph
                var parent = run.Parent;
                if (parent == null) continue;

                // Копируем RunProperties исходного run'а для каждого нового
                // (каждый раз новый экземпляр, иначе ApplyBitToRun испортит общий)
                var originalProps = run.RunProperties;

                // Опорный узел: вставляем новые Run перед исходным
                var insertBefore = run;

                for (int i = 0; i < text.Length; i++)
                {
                    var newRun = new Run();

                    // Копия свойств
                    if (originalProps != null)
                        newRun.RunProperties = (RunProperties)originalProps.CloneNode(true);

                    // Один символ
                    newRun.AppendChild(new Text(text[i].ToString())
                    {
                        Space = SpaceProcessingModeValues.Preserve
                    });

                    parent.InsertBefore(newRun, insertBefore);
                }

                // Удаляем исходный многосимвольный Run
                run.Remove();
            }
        }


        private static string EncodeToBits(string message, string encChoice)
        {
            return encChoice switch
            {
                "1" => EncodeMtk2(message),
                "2" => EncodeKoi8r(message),
                "3" => EncodeCp866(message),
                "4" => EncodeWin1251(message),
                _ => EncodeMtk2(message)
            };
        }

        private static string EncodeMtk2(string message)
        {
            var sb = new StringBuilder();

            int currentRegister = 0;

            foreach (char raw in message)
            {
                char c = raw;

                if (c == ' ')
                {
                    sb.Append(ToBits(0b00100, 5));
                    continue;
                }

                int targetRegister;
                int code;

                if (TryGetMtk2Code(c, out code, out targetRegister))
                {
                    if (targetRegister != currentRegister)
                    {
                        sb.Append(ToBits(GetRegisterSwitchCode(targetRegister), 5));
                        currentRegister = targetRegister;
                    }

                    sb.Append(ToBits(code, 5));
                }
            }

            return sb.ToString();
        }

        private static string EncodeKoi8r(string message)
        {
            var sb = new StringBuilder();
            var reverse = BuildReverse(Encodings.Koi8rTable);
            foreach (char c in message)
            {
                if (reverse.TryGetValue(c, out int code))
                    sb.Append(ToBits(code, 8));
            }
            return sb.ToString();
        }

        private static string EncodeCp866(string message)
        {
            var sb = new StringBuilder();
            var reverse = BuildReverse(Encodings.Cp866Table);
            foreach (char c in message)
            {
                if (reverse.TryGetValue(c, out int code))
                    sb.Append(ToBits(code, 8));
            }
            return sb.ToString();
        }

        private static string EncodeWin1251(string message)
        {
            var sb = new StringBuilder();
            var reverse = BuildReverse(Encodings.Win1251Table);
            foreach (char c in message)
            {
                if (reverse.TryGetValue(c, out int code))
                    sb.Append(ToBits(code, 8));
            }
            return sb.ToString();
        }

        private static bool TryGetMtk2Code(char c, out int code, out int register)
        {
            // 1. Русский регистр (Mtk2Russian)
            foreach (var kv in Encodings.Mtk2Russian)
            {
                if (kv.Value == c)
                {
                    code = kv.Key;
                    register = 0;
                    return true;
                }
            }

            // 2. Латинский регистр (Mtk2Latin)
            foreach (var kv in Encodings.Mtk2Latin)
            {
                if (kv.Value == c)
                {
                    code = kv.Key;
                    register = 1;
                    return true;
                }
            }

            // 3. Цифры/знаки (Mtk2Digits)
            foreach (var kv in Encodings.Mtk2Digits)
            {
                if (kv.Value == c)
                {
                    code = kv.Key;
                    register = 2;
                    return true;
                }
            }

            code = 0;
            register = -1;
            return false;
        }

        private static int GetRegisterSwitchCode(int register)
        {
            return register switch
            {
                0 => 0b00000, // русский
                1 => 0b11111, // латиница
                2 => 0b11011, // цифры
                _ => 0b00000
            };
        }

        private static Dictionary<char, int> BuildReverse(char[] table)
        {
            var d = new Dictionary<char, int>();
            for (int i = 0; i < table.Length; i++)
            {
                if (!d.ContainsKey(table[i]))
                    d[table[i]] = i;
            }
            return d;
        }

        private static string ToBits(int value, int width)
        {
            var sb = new StringBuilder(width);
            for (int i = width - 1; i >= 0; i--)
                sb.Append(((value >> i) & 1) == 1 ? '1' : '0');
            return sb.ToString();
        }
    }
}