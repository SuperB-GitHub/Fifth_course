class Program
{
    static void Main()
    {

    }

    static string Vigenere(string text, string keyword, bool isEncrypt)
    {
        string alph = "АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
        int n = alph.Length;

        string result = "";
        int indexKey = 0;

        for (int i = 0; i < text.Length; i++)
        {
            char charText = text[i];

            if (char.IsLetter(charText))
            {
                char charKey = keyword[Mod(indexKey, keyword.Length)];
                char answer = isEncrypt ? alph[Mod(alph.IndexOf(charText) + alph.IndexOf(charKey), n)] : alph[Mod(alph.IndexOf(charText) - alph.IndexOf(charKey), n)];
                Console.WriteLine($"i = {i}, ОТ: {charText} = {alph.IndexOf(charText)}, {charKey} = {alph.IndexOf(charKey)}, ШТ: {answer}");
                result += answer;
                indexKey++;
            }
            else
            {
                Console.WriteLine($"i = {i}, {charText}");
                result += charText;
            }
        }

        return result;
    }

    static int Mod(int a, int m)
    {
        return (a % m + m) % m;
    }
}