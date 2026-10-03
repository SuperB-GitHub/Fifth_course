using System;

namespace Лабораторная_1
{
    public static class ArrayUtils
    {
        // --- Новый вариант ---
        public static int OddOrPosFixed(int[] x)
        {
            int count = 0;
            for (int i = 0; i < x.Length; i++)
            {
                // Правильная проверка на нечетность
                if (x[i] % 2 != 0 || x[i] > 0)
                {
                    count++;
                }
            }
            return count;
        }

        // --- Старый вариант ---
        public static int OddOrPosBroken(int[] x)
        {
            int count = 0;
            for (int i = 0; i < x.Length; i++)
            {
                // Ошибка: для отрицательных нечетных чисел остаток равен -1, а не 1
                if (x[i] % 2 == 1 || x[i] > 0)
                {
                    count++;
                }
            }
            return count;
        }

        public static void RunTest(string testName, int[] input, int expected, bool useFixedVersion)
        {
            int actual;

            if (useFixedVersion)
                actual = OddOrPosFixed(input);
            else
                actual = OddOrPosBroken(input);

            string status = (actual == expected) ? "ПРОЙДЕН" : "ПРОВАЛЕН";

            Console.WriteLine($"[{status}] {testName}");
            Console.WriteLine($"   Вход: [{string.Join(", ", input)}]");
            Console.WriteLine($"   Ожидалось: {expected}, Получено: {actual}");
            Console.WriteLine();
        }
    }
}