using System.Numerics;

bool continueCalculating = true;

while (continueCalculating)
{
    Console.Clear();

    BigInteger num1 = 0;
    BigInteger num2 = 0;
    string? sign = "";

    Console.Write("Введите первое число: ");
    while (!BigInteger.TryParse(Console.ReadLine(), out num1))
    {
        Console.SetCursorPosition(0, Console.CursorTop - 1);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, Console.CursorTop - 1);
        Console.Write("Введите первое число (только целое число!): ");
    }

    Console.WriteLine($"Длина разряда: {num1.ToString().Length}\n");

    Console.Write("Введите второе число: ");
    while (!BigInteger.TryParse(Console.ReadLine(), out num2))
    {
        Console.SetCursorPosition(0, Console.CursorTop - 1);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, Console.CursorTop - 1);
        Console.Write("Введите второе число (только целое число!): ");
    }
    Console.WriteLine($"Длина разряда: {num2.ToString().Length}\n");

    Console.Write("Введите знак (+, -, *, /, %): ");
    bool validSign = false;
    while (!validSign)
    {
        sign = Console.ReadLine();

        if (sign != "+" && sign != "-" && sign != "*" && sign != "/" && sign != "%")
        {
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.Write("Введите знак (только +, -, *, /, %): ");
            continue;
        }

        if ((sign == "&" || sign == "/") && (num1 == 0 || num2 == 0))
        {
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.Write("Ошибка: деление на ноль! Введите другой знак: ");
            continue;
        }

        validSign = true;
    }

    switch (sign)
    {
        case "+":
            Console.WriteLine($"\nРезультат: {num1} {sign} {num2} = {num1 + num2}");
            break;
        case "-":
            Console.WriteLine($"\nРезультат: {num1} {sign} {num2} = {num1 - num2}");
            break;
        case "*":
            Console.WriteLine($"\nРезультат: {num1} {sign} {num2} = {num1 * num2}");
            break;
        case "/":
            Console.WriteLine($"\nРезультат: {num1} {sign} {num2} = {num1 / num2} (остаток {num1 % num2})");
            break;
        case "%":
            Console.WriteLine($"\nРезультат: {num1} {sign} {num2} = {num1 % num2}");
            break;
    }


    Console.Write("\nХотите продолжить? Нажмите Enter... ");
    string? answer = Console.ReadLine()?.ToLower();

    if (answer != "")
    {
        continueCalculating = false;
    }
}