using System;

namespace Лабораторная_1
{
    public static class BoundaryTests
    {
        public static double CalculateArea(double side1, double side2)
        {
            if (side1 < 0 || side1 > 100 || side2 < 0 || side2 > 100)
            {
                throw new ArgumentOutOfRangeException("Стороны должны быть в диапазоне [0, 100]");
            }
            return side1 * side2;
        }

        public static void RunAllTests()
        {
            Console.WriteLine("--- Тестирование side1 (side2 = 50) ---");
            CheckResult("Test 1: side1 Min (0)", 0, 50, 0);
            CheckResult("Test 2: side1 Min+ (1)", 1, 50, 50);
            CheckResult("Test 3: side1 Nom (50)", 50, 50, 2500);
            CheckResult("Test 4: side1 Max- (99)", 99, 50, 4950);
            CheckResult("Test 5: side1 Max (100)", 100, 50, 5000);

            Console.WriteLine("\n--- Тестирование side2 (side1 = 50) ---");
            CheckResult("Test 6: side2 Min (0)", 50, 0, 0);
            CheckResult("Test 7: side2 Min+ (1)", 50, 1, 50);
            CheckResult("Test 8: side2 Nom (50)", 50, 50, 2500);
            CheckResult("Test 9: side2 Max- (99)", 50, 99, 4950);
            CheckResult("Test 10: side2 Max (100)", 50, 100, 5000);
        }

        private static void CheckResult(string testName, double s1, double s2, double expected)
        {
            try
            {
                double actual = CalculateArea(s1, s2);
                string status = (Math.Abs(actual - expected) < 0.001) ? "ПРОЙДЕН" : "ПРОВАЛЕН";
                
                Console.WriteLine($"[{status}] {testName,-25} | Area({s1,3}, {s2,3}) = {actual,5} (Ожидалось: {expected,5})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ОШИБКА] {testName}: {ex.Message}");
            }
        }
    }
}