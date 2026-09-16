using System;
using System.Collections.Generic;
using System.IO;
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
        static string RLDecoding(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";

            StringBuilder decoded = new StringBuilder();
            int count = 0;

            foreach (char c in input)
            {
                if (char.IsDigit(c))
                {
                    count = count * 10 + (c - '0');
                }
                else
                {
                    int repeat = (count == 0) ? 1 : count;
                    decoded.Append(c, repeat);
                    count = 0;
                }
            }
            return decoded.ToString();
        }

        static void Main(string[] args)
        {
            Console.Write("Введите номер или имя файла sequences (напр. 0, 1 или sequences.0.txt): ");
            string seqInput = Console.ReadLine()?.Trim() ?? "";
            string seqFile = seqInput.EndsWith(".txt", StringComparison.OrdinalIgnoreCase)
                ? seqInput
                : $"sequences.{seqInput}.txt";

            Console.Write("Введите номер или имя файла commands (напр. 0, 1 или commands.0.txt): ");
            string cmdInput = Console.ReadLine()?.Trim() ?? "";
            string cmdFile = cmdInput.EndsWith(".txt", StringComparison.OrdinalIgnoreCase)
                ? cmdInput
                : $"commands.{cmdInput}.txt";

            if (!File.Exists(seqFile))
            {
                Console.WriteLine($"\nОшибка: Файл '{seqFile}' не найден!");
                return;
            }

            if (!File.Exists(cmdFile))
            {
                Console.WriteLine($"\nОшибка: Файл '{cmdFile}' не найден!");
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
                    GeneticData data = new GeneticData();
                    data.protein = parts[0];
                    data.organism = parts[1];
                    data.amino_acids = RLDecoding(parts[2]);
                    database.Add(data);
                }
            }

            string[] cmdLines = File.ReadAllLines(cmdFile);

            using (StreamWriter writer = new StreamWriter(outFile, false, Encoding.UTF8))
            {
                writer.WriteLine("Ekaterina");
                writer.WriteLine("Genetic Searching");

                int cmdCounter = 1;
                string separator = new string('-', 74);

                foreach (string line in cmdLines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] parts = line.Split('\t');
                    string commandName = parts[0];

                    writer.WriteLine(separator);

                    if (commandName == "search" && parts.Length >= 2)
                    {
                        string decodedPattern = RLDecoding(parts[1]);

                        writer.WriteLine($"{cmdCounter:D3}   search   {decodedPattern}");
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
                    else if (commandName == "diff" && parts.Length >= 3)
                    {
                        string prot1 = parts[1];
                        string prot2 = parts[2];

                        writer.WriteLine($"{cmdCounter:D3}   diff   {prot1}   {prot2}");
                        writer.WriteLine("amino-acids difference:");

                        GeneticData p1 = new GeneticData();
                        GeneticData p2 = new GeneticData();
                        bool found1 = false;
                        bool found2 = false;

                        foreach (var item in database)
                        {
                            if (item.protein == prot1 && !found1) { p1 = item; found1 = true; }
                            if (item.protein == prot2 && !found2) { p2 = item; found2 = true; }
                        }

                        if (!found1 || !found2)
                        {
                            string missing = "MISSING:";
                            if (!found1) missing += " " + prot1;
                            if (!found2) missing += " " + prot2;
                            writer.WriteLine(missing);
                        }
                        else
                        {
                            string seq1 = p1.amino_acids;
                            string seq2 = p2.amino_acids;

                            int minLen = Math.Min(seq1.Length, seq2.Length);
                            int diffCount = 0;

                            for (int i = 0; i < minLen; i++)
                            {
                                if (seq1[i] != seq2[i])
                                {
                                    diffCount++;
                                }
                            }

                            diffCount += Math.Abs(seq1.Length - seq2.Length);
                            writer.WriteLine(diffCount);
                        }
                    }
                    else if (commandName == "mode" && parts.Length >= 2)
                    {
                        string protName = parts[1];

                        writer.WriteLine($"{cmdCounter:D3}   mode   {protName}");
                        writer.WriteLine("amino-acid occurs:");

                        GeneticData target = new GeneticData();
                        bool found = false;

                        foreach (var item in database)
                        {
                            if (item.protein == protName)
                            {
                                target = item;
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            writer.WriteLine($"MISSING: {protName}");
                        }
                        else
                        {
                            Dictionary<char, int> freq = new Dictionary<char, int>();
                            foreach (char c in target.amino_acids)
                            {
                                if (!freq.ContainsKey(c)) freq[c] = 0;
                                freq[c]++;
                            }

                            char maxChar = ' ';
                            int maxCount = -1;

                            foreach (var pair in freq)
                            {
                                if (pair.Value > maxCount || (pair.Value == maxCount && pair.Key < maxChar))
                                {
                                    maxCount = pair.Value;
                                    maxChar = pair.Key;
                                }
                            }

                            writer.WriteLine($"{maxChar}          {maxCount}");
                        }
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