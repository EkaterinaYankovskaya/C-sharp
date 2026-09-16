using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GeneticSearch
{
    struct GeneticData
    {
        public string protein;
        public string organism;
        public string amino_acids;
    }

    class Program
    {
        static string RLDecoding(string amino_acids)
        {
            if (string.IsNullOrEmpty(amino_acids))
                return string.Empty;

            StringBuilder decoded = new StringBuilder();
            for (int i = 0; i < amino_acids.Length; i++)
            {
                char ch = amino_acids[i];
                if (char.IsDigit(ch))
                {
                    int count = ch - '0';
                    char letter = amino_acids[i + 1];
                    decoded.Append(letter, count);
                    i++;
                }
                else
                {
                    decoded.Append(ch);
                }
            }
            return decoded.ToString();
        }

        static void ProcessSearch(int cmdNum, string pattern, List<GeneticData> database, StreamWriter writer)
        {
            string decodedPattern = RLDecoding(pattern);

            writer.WriteLine($"{(cmdNum):D3}   search   {decodedPattern}");
            writer.WriteLine("organism                 protein");

            bool found = false;
            foreach (var item in database)
            {
                if (item.amino_acids.Contains(decodedPattern))
                {
                    writer.WriteLine($"{item.organism,-24} {item.protein}");
                    found = true;
                }
            }

            if (!found)
            {
                writer.WriteLine("NOT FOUND");
            }
        }

        static void ProcessDiff(int cmdNum, string prot1, string prot2, List<GeneticData> database, StreamWriter writer)
        {
            writer.WriteLine($"{(cmdNum):D3}   diff   {prot1}   {prot2}");
            writer.WriteLine("amino-acids difference:");

            GeneticData? p1 = null;
            GeneticData? p2 = null;

            foreach (var item in database)
            {
                if (item.protein == prot1 && p1 == null) p1 = item;
                if (item.protein == prot2 && p2 == null) p2 = item;
            }

            if (p1 == null || p2 == null)
            {
                StringBuilder missing = new StringBuilder("MISSING:");
                if (p1 == null) missing.Append(" ").Append(prot1);
                if (p2 == null) missing.Append(" ").Append(prot2);
                writer.WriteLine(missing.ToString());
                return;
            }

            string seq1 = p1.Value.amino_acids;
            string seq2 = p2.Value.amino_acids;

            int diffCount = 0;
            int minLen = Math.Min(seq1.Length, seq2.Length);

            for (int i = 0; i < minLen; i++)
            {
                if (seq1[i] != seq2[i])
                    diffCount++;
            }

            diffCount += Math.Abs(seq1.Length - seq2.Length);

            writer.WriteLine(diffCount);
        }

        static void ProcessMode(int cmdNum, string protName, List<GeneticData> database, StreamWriter writer)
        {
            writer.WriteLine($"{(cmdNum):D3}   mode   {protName}");
            writer.WriteLine("amino-acid occurs:");

            GeneticData? target = null;
            foreach (var item in database)
            {
                if (item.protein == protName)
                {
                    target = item;
                    break;
                }
            }

            if (target == null)
            {
                writer.WriteLine($"MISSING: {protName}");
                return;
            }

            Dictionary<char, int> freq = new Dictionary<char, int>();
            foreach (char c in target.Value.amino_acids)
            {
                if (!freq.ContainsKey(c))
                    freq[c] = 0;
                freq[c]++;
            }

            char maxChar = 'A';
            int maxCount = -1;

            foreach (var kvp in freq.OrderBy(k => k.Key))
            {
                if (kvp.Value > maxCount)
                {
                    maxCount = kvp.Value;
                    maxChar = kvp.Key;
                }
            }

            writer.WriteLine($"{maxChar}          {maxCount}");
        }

        static void Main(string[] args)
        {
            Console.Write("Введите номер или полное имя файла sequences (напр. 0, 1 или sequences.0.txt): ");
            string seqInput = Console.ReadLine()?.Trim() ?? "";
            string seqFile = seqInput.EndsWith(".txt", StringComparison.OrdinalIgnoreCase)
                ? seqInput
                : $"sequences.{seqInput}.txt";

            Console.Write("Введите номер или полное имя файла commands (напр. 0, 1 или commands.0.txt): ");
            string cmdInput = Console.ReadLine()?.Trim() ?? "";
            string cmdFile = cmdInput.EndsWith(".txt", StringComparison.OrdinalIgnoreCase)
                ? cmdInput
                : $"commands.{cmdInput}.txt";

            if (!File.Exists(seqFile))
            {
                Console.WriteLine($"\nОшибка: Файл последовательностей '{seqFile}' не найден!");
                return;
            }

            if (!File.Exists(cmdFile))
            {
                Console.WriteLine($"\nОшибка: Файл команд '{cmdFile}' не найден!");
                return;
            }

            string outFile = $"genedata.seq_{seqInput}.cmd_{cmdInput}.out.txt";

            List<GeneticData> database = new List<GeneticData>();
            string[] seqLines = File.ReadAllLines(seqFile);

            foreach (string line in seqLines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                string[] parts = line.Split('\t');
                if (parts.Length >= 3)
                {
                    GeneticData data;
                    data.protein = parts[0];
                    data.organism = parts[1];
                    data.amino_acids = RLDecoding(parts[2]);
                    database.Add(data);
                }
            }

            using (StreamReader reader = new StreamReader(cmdFile))
            using (StreamWriter writer = new StreamWriter(outFile, false, Encoding.UTF8))
            {
                writer.WriteLine("Ekaterina");
                writer.WriteLine("Genetic Searching");

                int cmdCounter = 1;
                string separator = new string('-', 74);

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] parts = line.Split('\t');
                    string commandName = parts[0];

                    writer.WriteLine(separator);

                    if (commandName == "search" && parts.Length >= 2)
                    {
                        ProcessSearch(cmdCounter, parts[1], database, writer);
                    }
                    else if (commandName == "diff" && parts.Length >= 3)
                    {
                        ProcessDiff(cmdCounter, parts[1], parts[2], database, writer);
                    }
                    else if (commandName == "mode" && parts.Length >= 2)
                    {
                        ProcessMode(cmdCounter, parts[1], database, writer);
                    }

                    cmdCounter++;
                }
            }

            Console.WriteLine($"\nОбработка успешно завершена!");
            Console.WriteLine($"Файл последовательностей: {seqFile}");
            Console.WriteLine($"Файл команд:               {cmdFile}");
            Console.WriteLine($"Результат сохранен в:      {outFile}");
        }
    }
}