using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zad4Playfair
{
    class Program
    {
        private static string keywordFileName = "keyword.txt";
        private static string explicitTextFileName = "plik.txt";

        private static Dictionary<char, int> alphabet = new Dictionary<char, int>
        {
            {'a', 0 },
            {'b', 1 },
            {'c', 2 },
            {'d', 3 },
            {'e', 4 },
            {'f', 5 },
            {'g', 6 },
            {'h', 7 },
            {'i', 8 },
            {'k', 9 },
            {'l', 10 },
            {'m', 11 },
            {'n', 12 },
            {'o', 13 },
            {'p', 14 },
            {'q', 15 },
            {'r', 16 },
            {'s', 17 },
            {'t', 18 },
            {'u', 19 },
            {'v', 20 },
            {'w', 21 },
            {'x', 22 },
            {'y', 23 },
            {'z', 24 }
        };

        private static int tableCols = 5;
        private static int tableRows = 5;
        private static char[,] table;
        private static List<string> explicitTextPairs;

        public static void Main(string[] args)
        {
            string keyword = GetTextFromFile(keywordFileName);
            string explicitText = GetTextFromFile(explicitTextFileName);
            List<string> encodedPairs;
            List<string> decodedPairs;

            keyword = RemoveRepetitions(keyword);
            explicitText = SeparateRepetitions(explicitText);
            explicitText = MakeCharsAmountEven(explicitText);
            explicitTextPairs = extractPairsFrom(explicitText);
            table = CreateTableFrom(keyword);
            encodedPairs = Encode(explicitTextPairs);
            decodedPairs = Decode(encodedPairs);

            encodedPairs.ForEach(x => Console.Write(x + " "));
            Console.WriteLine();
            decodedPairs.ForEach(x => Console.Write(x + " "));

            Console.ReadLine();
        }

        private static List<string> Encode(List<string> pairs)
        {
            List<string> encodedPairs = new List<string>();

            foreach (var pair in pairs)
            {
                string encodedPair = String.Empty;
                Coords firstSign = GetSignCoords(pair[0]);
                Coords secondSign = GetSignCoords(pair[1]);
                int newIndex;

                if (firstSign.X.Equals(secondSign.X))
                {
                    encodedPair += table[(firstSign.Y + 1) % table.GetLength(0), firstSign.X];
                    encodedPair += table[(secondSign.Y + 1) % table.GetLength(0), secondSign.X];
                }
                else if (firstSign.Y.Equals(secondSign.Y))
                {
                    encodedPair += table[firstSign.Y, (firstSign.X + 1) % table.GetLength(1)];
                    encodedPair += table[secondSign.Y, (secondSign.X + 1) % table.GetLength(1)];
                }
                else
                {
                    encodedPair += table[firstSign.Y, secondSign.X];
                    encodedPair += table[secondSign.Y, firstSign.X];
                }

                encodedPairs.Add(encodedPair);
            }

            return encodedPairs;
        }

        private static List<string> Decode(List<string> pairs)
        {
            List<string> encodedPairs = new List<string>();

            foreach (var pair in pairs)
            {
                string encodedPair = String.Empty;
                Coords firstSign = GetSignCoords(pair[0]);
                Coords secondSign = GetSignCoords(pair[1]);
                int newIndex;

                if (firstSign.X.Equals(secondSign.X))
                {
                    newIndex = firstSign.Y.Equals(0) ? table.GetLength(0) - 1 : firstSign.Y - 1;

                    encodedPair += table[newIndex, firstSign.X];

                    newIndex = secondSign.Y.Equals(0) ? table.GetLength(0) - 1 : secondSign.Y - 1;

                    encodedPair += table[newIndex, secondSign.X];
                }
                else if (firstSign.Y.Equals(secondSign.Y))
                {
                    newIndex = firstSign.X.Equals(0) ? table.GetLength(1) - 1 : firstSign.X - 1;

                    encodedPair += table[firstSign.Y, newIndex];

                    newIndex = secondSign.X.Equals(0) ? table.GetLength(1) - 1 : secondSign.X - 1;

                    encodedPair += table[secondSign.Y, newIndex];
                }
                else
                {
                    encodedPair += table[firstSign.Y, secondSign.X];
                    encodedPair += table[secondSign.Y, firstSign.X];
                }

                encodedPairs.Add(encodedPair);
            }

            return encodedPairs;
        }

        private static Coords GetSignCoords(char sign)
        {
            if (sign.Equals('j'))
            {
                sign = 'i';
            }

            for (int i = 0; i < table.GetLength(0); i++)
            {
                for (int j = 0; j < table.GetLength(1); j++)
                {
                    if (table[i,j].Equals(sign))
                    {
                        return new Coords() { X = j, Y = i };
                    }
                }
            }

            throw new Exception("Undefined sign requirement");
        }

        private static string GetTextFromFile(string filename)
        {
            if (File.Exists(filename))
            {
                return File.ReadAllText(filename).ToLower();
            }

            return String.Empty;
        }
    
        private static string RemoveRepetitions(string text)
        {
            return new string(text.ToCharArray().Distinct().ToArray());
        }

        private static string SeparateRepetitions(string text)
        {
            char previousSign = text[0];

            for (int i = 1; i < text.Length; i++)
            {
                if (previousSign.Equals(text[i]))
                {
                    string alphabetSign = alphabet.FirstOrDefault(x => x.Key != previousSign).Key.ToString();
                    text = text.Insert(i, alphabetSign);
                    i++;
                }

                previousSign = text[i];
            }

            return text;
        }

        private static string MakeCharsAmountEven(string text)
        {
            int textLength = text.Length;

            if (textLength % 2 != 0)
            {
                char lastSign = text[textLength - 1];

                text += alphabet.FirstOrDefault(x => x.Key != lastSign).Key;
            }

            return text;
        }

        private static List<string> extractPairsFrom(string text)
        {
            List<string> pairs = new List<string>();

            for (int i = 0; i < text.Length; i += 2)
            {
                pairs.Add(text.Substring(i, 2));
            }

            return pairs;
        }

        private static char[,] CreateTableFrom(string keyword)
        {
            int keywordIndex = 0;
            int alphabetIndex = 0;
            char[,] table = new char[5, 5];
            char alphabetSign;
            List<char> charsUsed = new List<char>();
            
            for (int i = 0; i < tableRows; i++)
            {
                for (int j = 0; j < tableCols; j++)
                {
                    if (keywordIndex < keyword.Length)
                    {
                        char keywordSign = keyword[keywordIndex];

                        table[i, j] = keywordSign;
                        charsUsed.Add(keywordSign);
                    }
                    else
                    {
                        do
                        {
                            alphabetSign = GetAlphabetKeyByValue(alphabetIndex);

                            alphabetIndex++;
                        }
                        while (charsUsed.Contains(alphabetSign));

                        table[i, j] = alphabetSign;
                    }

                    keywordIndex++;
                }
                
            }

            return table;
        }

        private static char GetAlphabetKeyByValue(int value)
            => alphabet.FirstOrDefault(x => x.Value == value).Key;

        private static void PrintTable()
        {
            for (int i = 0; i < table.GetLength(0); i++)
            {
                for (int j = 0; j < table.GetLength(1); j++)
                {
                    Console.Write(table[i, j] + " ");
                }

                Console.WriteLine();
            }
        }
    }
}
