using System;
using System.Text;

namespace Лабораторная_1
{
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("=== Меню ===");
                Console.WriteLine("  1 — Скрыть информацию в документе");
                Console.WriteLine("  2 — Раскрыть информацию из документа");
                Console.WriteLine("  0 — Выход");
                Console.Write("Ваш выбор: ");

                string mode = Console.ReadLine()?.Trim() ?? "";

                switch (mode)
                {
                    case "1":
                        Hider.Run();
                        break;

                    case "2":
                        Revealer.Run();
                        break;

                    case "0":
                        Console.WriteLine("Выход из программы.");
                        return;

                    default:
                        Console.WriteLine("Неверный режим. Попробуйте снова.");
                        break;
                }

                Console.WriteLine();
                Console.WriteLine("Нажмите Enter, чтобы вернуться в меню...");
                Console.ReadLine();
                Console.Clear();
            }
        }
    }
}