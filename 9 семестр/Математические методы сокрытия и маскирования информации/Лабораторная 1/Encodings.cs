using System;
using System.Collections.Generic;
using System.Text;

namespace DocxInspector
{
    public static class Encodings
    {
        public static readonly Dictionary<int, char> Mtk2Latin = new()
        {
            { 0b00011, 'A' }, { 0b11001, 'B' }, { 0b01110, 'C' },
            { 0b01001, 'D' }, { 0b00001, 'E' }, { 0b01101, 'F' },
            { 0b11010, 'G' }, { 0b10100, 'H' }, { 0b00110, 'I' },
            { 0b01011, 'J' }, { 0b01111, 'K' }, { 0b10010, 'L' },
            { 0b11100, 'M' }, { 0b01100, 'N' }, { 0b11000, 'O' },
            { 0b10110, 'P' }, { 0b10111, 'Q' }, { 0b01010, 'R' },
            { 0b00101, 'S' }, { 0b10000, 'T' }, { 0b00111, 'U' },
            { 0b11110, 'V' }, { 0b10011, 'W' }, { 0b11101, 'X' },
            { 0b10101, 'Y' }, { 0b10001, 'Z' },
        };

        public static readonly Dictionary<int, char> Mtk2Russian = new()
        {
            { 0b00011, 'А' }, { 0b11001, 'Б' }, { 0b01110, 'Ц' },
            { 0b01001, 'Д' }, { 0b00001, 'Е' }, { 0b01101, 'Ф' },
            { 0b11010, 'Г' }, { 0b10100, 'Х' }, { 0b00110, 'И' },
            { 0b01011, 'Й' }, { 0b01111, 'К' }, { 0b10010, 'Л' },
            { 0b11100, 'М' }, { 0b01100, 'Н' }, { 0b11000, 'О' },
            { 0b10110, 'П' }, { 0b10111, 'Я' }, { 0b01010, 'Р' },
            { 0b00101, 'С' }, { 0b10000, 'Т' }, { 0b00111, 'У' },
            { 0b11110, 'Ж' }, { 0b10011, 'В' }, { 0b11101, 'Ь' },
            { 0b10101, 'Ы' }, { 0b10001, 'З' },
        };

        public static readonly Dictionary<int, char> Mtk2Digits = new()
        {
            { 0b00011, '-' }, { 0b11001, '?' }, { 0b01110, ':' },
            { 0b01001, 'К' }, { 0b00001, '3' }, { 0b01101, 'Э' },
            { 0b11010, 'Ш' }, { 0b10100, 'Щ' }, { 0b00110, '8' },
            { 0b01011, 'Ю' }, { 0b01111, '(' }, { 0b10010, ')' },
            { 0b11100, '.' }, { 0b01100, ',' }, { 0b11000, '9' },
            { 0b10110, '0' }, { 0b10111, '1' }, { 0b01010, '4' },
            { 0b00101, '\'' }, { 0b10000, '5' }, { 0b00111, '7' },
            { 0b11110, '=' }, { 0b10011, '2' }, { 0b11101, '/' },
            { 0b10101, '6' }, { 0b10001, '+' },
        };

        private enum Mtk2Register
        {
            Latin,
            Russian,
            Digits
        }

        private const int Mtk2CodeLatin = 0b11111;
        private const int Mtk2CodeDigits = 0b11011;
        private const int Mtk2CodeRussian = 0b00000;
        private const int Mtk2CodeCR = 0b01000;
        private const int Mtk2CodeLF = 0b00010;
        private const int Mtk2CodeSpace = 0b00100;

        private static readonly char[] Koi8rTable = BuildKoi8r();

        private static char[] BuildKoi8r()
        {
            var t = new char[256];
            for (int i = 0; i < 128; i++) t[i] = (char)i;
            string lower = "юабцдефгхийклмнопярстужвьызшэщчъ";
            string upper = "ЮАБЦДЕФГХИЙКЛМНОПЯРСТУЖВЬЫЗШЭЩЧЪ";
            for (int i = 0; i < 32; i++) t[0xC0 + i] = lower[i];
            for (int i = 0; i < 32; i++) t[0xE0 + i] = upper[i];
            for (int i = 0x80; i < 0xC0; i++) if (t[i] == '\0') t[i] = '·';
            return t;
        }

        private static readonly char[] Cp866Table = BuildCp866();

        private static char[] BuildCp866()
        {
            var t = new char[256];

            for (int i = 0; i < 128; i++)
                t[i] = (char)i;

            string upper = "АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
            string lower = "абвгдежзийклмнопрстуфхцчшщъыьэюя";

            for (int i = 0; i < 32; i++)
            {
                t[0x80 + i] = upper[i];
                t[0xA0 + i] = lower[i];
            }

            t[0xB0] = '░'; t[0xB1] = '▒'; t[0xB2] = '▓'; t[0xB3] = '│';
            t[0xB4] = '┤'; t[0xB5] = '╡'; t[0xB6] = '╢'; t[0xB7] = '╖';
            t[0xB8] = '╕'; t[0xB9] = '╣'; t[0xBA] = '║'; t[0xBB] = '╗';
            t[0xBC] = '╝'; t[0xBD] = '╜'; t[0xBE] = '╛'; t[0xBF] = '┐';

            t[0xC0] = '└'; t[0xC1] = '┴'; t[0xC2] = '┬'; t[0xC3] = '├';
            t[0xC4] = '─'; t[0xC5] = '┼'; t[0xC6] = '╞'; t[0xC7] = '╟';
            t[0xC8] = '╚'; t[0xC9] = '╔'; t[0xCA] = '╩'; t[0xCB] = '╦';
            t[0xCC] = '╠'; t[0xCD] = '═'; t[0xCE] = '╬'; t[0xCF] = '╧';
            t[0xD0] = '╨'; t[0xD1] = '╤'; t[0xD2] = '╥'; t[0xD3] = '╙';
            t[0xD4] = '╘'; t[0xD5] = '╒'; t[0xD6] = '╓'; t[0xD7] = '╫';
            t[0xD8] = '╪'; t[0xD9] = '┘'; t[0xDA] = '┌'; t[0xDB] = '█';
            t[0xDC] = '▄'; t[0xDD] = '▌'; t[0xDE] = '▐'; t[0xDF] = '▀';

            t[0xE0] = 'р'; t[0xE1] = 'с'; t[0xE2] = 'т'; t[0xE3] = 'у';
            t[0xE4] = 'ф'; t[0xE5] = 'х'; t[0xE6] = 'ц'; t[0xE7] = 'ч';
            t[0xE8] = 'ш'; t[0xE9] = 'щ'; t[0xEA] = 'ъ'; t[0xEB] = 'ы';
            t[0xEC] = 'ь'; t[0xED] = 'э'; t[0xEE] = 'ю'; t[0xEF] = 'я';

            t[0xF0] = '≡'; t[0xF1] = '±'; t[0xF2] = '≥'; t[0xF3] = '≤';
            t[0xF4] = '⌠'; t[0xF5] = '⌡'; t[0xF6] = '÷'; t[0xF7] = '≈';
            t[0xF8] = '°'; t[0xF9] = '∙'; t[0xFA] = '·'; t[0xFB] = '√';
            t[0xFC] = 'ⁿ'; t[0xFD] = '²'; t[0xFE] = '■'; t[0xFF] = ' ';

            return t;
        }

        private static readonly char[] Win1251Table = BuildWin1251();

        private static char[] BuildWin1251()
        {
            var t = new char[256];
            for (int i = 0; i < 128; i++) t[i] = (char)i;
            string upper = "АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
            string lower = "абвгдежзийклмнопрстуфхцчшщъыьэюя";
            for (int i = 0; i < 32; i++) t[0xC0 + i] = upper[i];
            for (int i = 0; i < 32; i++) t[0xE0 + i] = lower[i];
            t[0xA0] = ' '; t[0xA8] = 'Ё'; t[0xB8] = 'ё';
            for (int i = 0; i < 256; i++) if (t[i] == '\0') t[i] = '·';
            return t;
        }

        public static List<int> SliceBits(string bits, int groupSize)
        {
            var result = new List<int>();
            for (int i = 0; i < bits.Length; i += groupSize)
            {
                int value = 0;
                for (int j = 0; j < groupSize; j++)
                {
                    value <<= 1;
                    if (i + j < bits.Length && bits[i + j] == '1')
                        value |= 1;
                }
                result.Add(value);
            }
            return result;
        }

        public static string DecodeMtk2(string bits)
        {
            var groups = SliceBits(bits, 5);
            var sb = new StringBuilder();
            Mtk2Register current = Mtk2Register.Russian;

            foreach (int code in groups)
            {
                switch (code)
                {
                    case Mtk2CodeLatin:
                        current = Mtk2Register.Latin;
                        continue;
                    case Mtk2CodeRussian:
                        current = Mtk2Register.Russian;
                        continue;
                    case Mtk2CodeDigits:
                        current = Mtk2Register.Digits;
                        continue;
                }

                if (code == Mtk2CodeCR) { sb.Append('\r'); continue; }
                if (code == Mtk2CodeLF) { sb.Append('\n'); continue; }
                if (code == Mtk2CodeSpace) { sb.Append(' '); continue; }

                Dictionary<int, char> table = current switch
                {
                    Mtk2Register.Latin => Mtk2Latin,
                    Mtk2Register.Russian => Mtk2Russian,
                    Mtk2Register.Digits => Mtk2Digits,
                    _ => Mtk2Latin
                };

                sb.Append(table.TryGetValue(code, out char c) ? c : '?');
            }

            return sb.ToString();
        }

        public static string DecodeKoi8r(string bits)
        {
            var groups = SliceBits(bits, 8);
            var sb = new StringBuilder();
            foreach (int code in groups)
                sb.Append(Koi8rTable[code & 0xFF]);
            return sb.ToString();
        }

        public static string DecodeCp866(string bits)
        {
            var groups = SliceBits(bits, 8);
            var sb = new StringBuilder();
            foreach (int code in groups)
                sb.Append(Cp866Table[code & 0xFF]);
            return sb.ToString();
        }

        public static string DecodeWin1251(string bits)
        {
            var groups = SliceBits(bits, 8);
            var sb = new StringBuilder();
            foreach (int code in groups)
                sb.Append(Win1251Table[code & 0xFF]);
            return sb.ToString();
        }
    }
}