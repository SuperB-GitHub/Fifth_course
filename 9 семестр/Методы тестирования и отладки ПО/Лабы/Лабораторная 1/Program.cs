using System;

namespace Лабораторная_1
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=================================================");
                Console.WriteLine("       ВЫБЕРИТЕ ЗАДАНИЕ ДЛЯ ПРОВЕРКИ");
                Console.WriteLine("=================================================");
                Console.WriteLine("1. Задание 1: Поиск ошибки в OddOrPos (Fault-hiding & Revealing)");
                Console.WriteLine("2. Задание 2: Normal Boundary Value Testing (Площадь)");
                Console.WriteLine("0. Выход");
                Console.WriteLine("=================================================");
                Console.Write("Ваш выбор: ");

                string choice = Console.ReadLine()!;

                switch (choice)
                {
                    case "1":
                        RunTask1();
                        break;
                    case "2":
                        RunTask2();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Неверный ввод. Нажмите любую клавишу...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        // --- ЗАДАНИЕ 1 ---
        static void RunTask1()
        {
            Console.Clear();
            Console.WriteLine("=== ЗАДАНИЕ 1: Тесты для OddOrPos ===\n");

            Console.WriteLine("--- ТЕСТЫ, СКРЫВАЮЩИЕ ОШИБКУ (Fault-hiding) ---");
            ArrayUtils.RunTest("Только положительные", [1, 2, 3, 4, 5], 5, true);
            ArrayUtils.RunTest("Только положительные", new int[] { 1, 2, 3, 4, 5 }, 5, false);
            ArrayUtils.RunTest("Только отрицательные четные", new int[] { -2, -4, -6 }, 0, true);
            ArrayUtils.RunTest("Только отрицательные четные", new int[] { -2, -4, -6 }, 0, false);
            ArrayUtils.RunTest("Пустой массив", new int[] { }, 0, true);
            ArrayUtils.RunTest("Пустой массив", new int[] { }, 0, false);

            Console.WriteLine("\n--- ТЕСТЫ, ВЫЯВЛЯЮЩИЕ ОШИБКУ (Fault-revealing) ---");
            ArrayUtils.RunTest("Отрицательное нечетное (Новый)", new int[] { -3 }, 1, true);
            ArrayUtils.RunTest("Отрицательное нечетное (Старый)", new int[] { -3 }, 1, false);
            ArrayUtils.RunTest("Смесь чисел (Новый)", new int[] { -3, -2, -1, 0, 1, 2, 3 }, 5, true);
            ArrayUtils.RunTest("Смесь чисел (Старый)", new int[] { -3, -2, -1, 0, 1, 2, 3 }, 5, false);

            Console.WriteLine("\nНажмите любую клавишу для возврата в меню...");
            Console.ReadKey();
        }

        // --- ЗАДАНИЕ 2 ---
        static void RunTask2()
        {
            Console.Clear();
            Console.WriteLine("=== ЗАДАНИЕ 2: Normal Boundary Value Testing ===\n");

            BoundaryTests.RunAllTests();

            Console.WriteLine("\nНажмите любую клавишу для возврата в меню...");
            Console.ReadKey();
        }
    }
}