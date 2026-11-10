using System;
using System.IO;
using System.Collections.Generic;
using TextModel = TextAnalyzerApp.models.Text;
using TextAnalyzerApp.models;

namespace TextAnalyzerApp
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            try
            {
                string inputPath = "input.txt";

                var text = TextModel.ParseFile(inputPath);
                Console.WriteLine($"Успешно загружено и распарсено предложений из файла: {text.Sentences.Count}\n");

                while (true)
                {
                    PrintMenu();
                    Console.Write("Выберите действие (0-9): ");
                    var choice = Console.ReadLine()?.Trim();
                    Console.WriteLine();

                    if (choice is null or "0")
                        break;

                    Execute(choice, text, inputPath);
                    Console.WriteLine("\n--------------------------------------------------\n");
                }
            }
            catch (Exception exception)
            {
                PrintError(exception.Message);
            }

            Console.WriteLine("Программа завершена.");
        }

        private static void Execute(string choice, TextModel text, string inputPath)
        {
            switch (choice)
            {
                case "1":
                    Console.WriteLine("--- Предложения по возрастанию количества слов ---");
                    PrintSentences(text.GetSentencesSortedByWordCount());
                    break;

                case "2":
                    Console.WriteLine("--- Предложения по возрастанию длины ---");
                    PrintSentences(text.GetSentencesSortedByLength());
                    break;

                case "3":
                    var len = ReadPositiveInt("Введите длину слова для поиска в вопросах: ");
                    var words = text.FindWordsInInterrogativeSentences(len);
                    Console.WriteLine(words.Count == 0 ? "Слова не найдены." : $"Найдены уникальные слова: {string.Join(", ", words)}");
                    break;

                case "4":
                    var consLen = ReadPositiveInt("Введите длину слова для удаления (начинающиеся с согласной): ");
                    int removedCons = text.RemoveWordsStartingWithConsonant(consLen);
                    Console.WriteLine($"Удалено слов: {removedCons}");
                    Console.WriteLine($"Обновленный текст:\n{text}");
                    break;

                case "5":
                    if (text.Sentences.Count == 0) { Console.WriteLine("Нет предложений."); break; }
                    int sNum = ReadIntInRange($"Номер предложения (1-{text.Sentences.Count}): ", 1, text.Sentences.Count);
                    int rLen = ReadPositiveInt("Длина заменяемого слова: ");
                    Console.Write("Новая подстрока: ");
                    string repl = Console.ReadLine() ?? string.Empty;
                    int replaced = text.ReplaceWordsInSentence(sNum - 1, rLen, repl);
                    Console.WriteLine($"Заменено слов: {replaced}");
                    Console.WriteLine($"Измененное предложение:\n{text.Sentences[sNum - 1]}");
                    break;

                case "6":
                    var stopWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    if (File.Exists("stopwords_en.txt"))
                        foreach (var l in File.ReadLines("stopwords_en.txt", System.Text.Encoding.UTF8))
                            if (!string.IsNullOrWhiteSpace(l)) stopWords.Add(l.Trim());

                    if (File.Exists("stopwords_ru.txt"))
                        foreach (var l in File.ReadLines("stopwords_ru.txt", System.Text.Encoding.UTF8))
                            if (!string.IsNullOrWhiteSpace(l)) stopWords.Add(l.Trim());

                    int removedStops = text.RemoveStopWords(stopWords);
                    Console.WriteLine($"Удалено стоп-слов: {removedStops}");
                    Console.WriteLine($"Текст без стоп-слов:\n{text}");
                    break;

                case "7":
                    Console.Write("Путь для сохранения XML (по умолчанию text.xml): ");
                    string xmlPath = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(xmlPath)) xmlPath = "text.xml";
                    text.ToXml(xmlPath);
                    Console.WriteLine($"Текст успешно экспортирован в: {Path.GetFullPath(xmlPath)}");
                    break;

                case "8":
                    Console.WriteLine($"Текущий текст:\n{text}");
                    break;

                case "9": // Конкорданс текста (Новое требование)[cite: 14]
                    Console.WriteLine("--- Конкорданс текста ---");
                    var fileLines = File.ReadLines(inputPath, System.Text.Encoding.UTF8);
                    var concordance = text.BuildConcordance(fileLines);

                    foreach (var pair in concordance)
                    {
                        string wordDots = pair.Key.PadRight(30, '.');
                        string linesStr = string.Join(" ", pair.Value.Lines);
                        Console.WriteLine($"{wordDots}{pair.Value.Frequency}: {linesStr}");
                    }
                    break;

                default:
                    Console.WriteLine("Неизвестный пункт меню.");
                    break;
            }
        }

        private static void PrintMenu()
        {
            Console.WriteLine("=== МЕНЮ АНАЛИЗА ТЕКСТА ===");
            Console.WriteLine("1. Вывести предложения по возрастанию количества слов");
            Console.WriteLine("2. Вывести предложения по возрастанию длины");
            Console.WriteLine("3. Найти слова заданной длины в вопросительных предложениях");
            Console.WriteLine("4. Удалить слова заданной длины, начинающиеся с согласной");
            Console.WriteLine("5. Заменить слова в выбранном предложении");
            Console.WriteLine("6. Удалить стоп-слова (из файлов ru/en)");
            Console.WriteLine("7. Экспортировать текст в XML");
            Console.WriteLine("8. Показать текущий текст");
            Console.WriteLine("9. Построить конкорданс текста");
            Console.WriteLine("0. Выход");
        }

        private static int ReadPositiveInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out var value) && value > 0) return value;
                Console.WriteLine("Ошибка. Введите положительное целое число.");
            }
        }

        private static int ReadIntInRange(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out var value) && value >= min && value <= max) return value;
                Console.WriteLine($"Ошибка. Введите число от {min} до {max}.");
            }
        }

        private static void PrintSentences(IEnumerable<Sentence> sentences)
        {
            int i = 1;
            foreach (var s in sentences)
                Console.WriteLine($"{i++}. {s} (слов: {s.GetWordCount()})");
        }

        private static void PrintError(string message) => Console.Error.WriteLine($"Ошибка: {message}");
    }
}