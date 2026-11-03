using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TextAnalyzerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            var stopWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (File.Exists("stopwords_en.txt"))
            {
                foreach (var line in File.ReadAllLines("stopwords_en.txt", Encoding.UTF8))
                {
                    if (!string.IsNullOrWhiteSpace(line)) stopWords.Add(line.Trim());
                }
            }
            if (File.Exists("stopwords_ru.txt"))
            {
                foreach (var line in File.ReadAllLines("stopwords_ru.txt", Encoding.UTF8))
                {
                    if (!string.IsNullOrWhiteSpace(line)) stopWords.Add(line.Trim());
                }
            }

            string sampleText = "Mary had a little lamb. Aesop and? Is this a test sentence?";
            var (text, rawSentences) = TextParser.ParseContent(sampleText);

            Console.WriteLine("--- 1. По возрастанию количества слов ---");
            foreach (var s in text.GetSentencesSortedByWordCount())
            {
                Console.WriteLine($"[{s.GetWordCount()} слов]: {s}");
            }

            Console.WriteLine("\n--- 2. По возрастанию длины предложения ---");
            foreach (var s in text.GetSentencesSortedByLength())
            {
                Console.WriteLine($"[{s.ToString().Length} символов]: {s}");
            }

            Console.WriteLine("\n--- 3. Слова длины 4 в вопросительных предложениях ---");
            var foundWords = text.FindWordsInInterrogativeSentences(4, rawSentences);
            foreach (var w in foundWords)
            {
                Console.WriteLine(w);
            }

            text.RemoveWordsStartingWithConsonant(3);
            text.ReplaceWordsInSentence(0, 4, "REPLACED");
            text.RemoveStopWords(stopWords);

            text.ToXml("output_text.xml");
            Console.WriteLine("\n--- Текст успешно экспортирован в output_text.xml ---");
        }
    }
}
