using System;
using System.IO;
using System.Text;
using Lab2Hashing;

Console.OutputEncoding = Encoding.UTF8;

while (true)
{
    Console.WriteLine();
    Console.WriteLine("=====================================");
    Console.WriteLine("   SHA-256 — Лабораторная работа №3");
    Console.WriteLine("=====================================");
    Console.WriteLine("  1. Создать hash для файла");
    Console.WriteLine("  2. Проверить hash файла");
    Console.WriteLine("  0. Выход");
    Console.WriteLine("-------------------------------------");
    Console.Write("Выберите пункт: ");

    string? choice = Console.ReadLine()?.Trim();

    switch (choice)
    {
        case "1": MenuCreateHash(); break;
        case "2": MenuCheckHash();  break;
        case "0": return;
        default:
            Console.WriteLine("Неверный выбор. Попробуйте снова.");
            break;
    }
}

// ---------------------------------------------------------------
static void MenuCreateHash()
{
    Console.WriteLine();
    Console.WriteLine("--- Создание hash ---");

    string inputPath = ReadExistingFile("Введите путь к входному файлу: ");
    if (inputPath.Length == 0) return;

    Console.Write("Введите путь к файлу для сохранения hash: ");
    string hashPath = Console.ReadLine()?.Trim().Trim('"') ?? "";
    if (hashPath.Length == 0)
    {
        Console.WriteLine("Путь не задан. Отмена.");
        return;
    }

    try
    {
        byte[] hash = Sha256.ComputeFileHash(inputPath);
        string hex = Sha256.ToHex(hash);

        // Если расширение не задано — допишем .sha256
        if (!Path.HasExtension(hashPath))
            hashPath += ".sha256";

        string? dir = Path.GetDirectoryName(Path.GetFullPath(hashPath));
        if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);

        File.WriteAllText(hashPath, hex, Encoding.UTF8);

        Console.WriteLine();
        Console.WriteLine($"Файл:        {Path.GetFullPath(inputPath)}");
        Console.WriteLine($"Размер:      {new FileInfo(inputPath).Length} байт");
        Console.WriteLine($"SHA-256:     {hex}");
        Console.WriteLine($"Сохранено в: {Path.GetFullPath(hashPath)}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка: {ex.Message}");
    }
}

// ---------------------------------------------------------------
static void MenuCheckHash()
{
    Console.WriteLine();
    Console.WriteLine("--- Проверка hash ---");

    string inputPath = ReadExistingFile("Введите путь к проверяемому файлу: ");
    if (inputPath.Length == 0) return;

    string hashPath = ReadExistingFile("Введите путь к файлу с сохранённым hash: ");
    if (hashPath.Length == 0) return;

    try
    {
        byte[] actual = Sha256.ComputeFileHash(inputPath);
        string actualHex = Sha256.ToHex(actual);

        string expectedHex = File.ReadAllText(hashPath).Trim().ToLowerInvariant();

        Console.WriteLine();
        Console.WriteLine($"Файл:               {Path.GetFullPath(inputPath)}");
        Console.WriteLine($"Ожидаемый hash:     {expectedHex}");
        Console.WriteLine($"Фактический hash:   {actualHex}");
        Console.WriteLine();

        if (string.Equals(expectedHex, actualHex, StringComparison.OrdinalIgnoreCase))
            Console.WriteLine(">>> РЕЗУЛЬТАТ: ХЕШИ СОВПАДАЮТ ✔");
        else
            Console.WriteLine(">>> РЕЗУЛЬТАТ: ХЕШИ НЕ СОВПАДАЮТ ✘");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка: {ex.Message}");
    }
}

// ---------------------------------------------------------------
/// <summary>Читает путь, проверяет существование файла; возвращает "" при отмене.</summary>
static string ReadExistingFile(string prompt)
{
    Console.Write(prompt);
    string path = Console.ReadLine()?.Trim().Trim('"') ?? "";

    if (path.Length == 0)
    {
        Console.WriteLine("Путь не задан. Отмена.");
        return "";
    }

    if (!File.Exists(path))
    {
        Console.WriteLine($"Файл не найден: {path}");
        return "";
    }
    return path;
}