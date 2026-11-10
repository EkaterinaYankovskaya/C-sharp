using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace TextAnalyzerApp.models
{
    [XmlRoot("text")]
    public class Text
    {
        [XmlElement("sentence")]
        public List<Sentence> Sentences { get; set; } = new List<Sentence>();

        public Text() { }

        public Text(List<Sentence> sentences)
        {
            Sentences = sentences;
        }

        public static Text ParseContent(string content)
        {
            // Регулярное выражение разбивает текст, сохраняя знаки препинания в конце каждого предложения
            var rawSentences = Regex.Split(content, @"(?<=[.!?])\s+").Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            var sentences = new List<Sentence>();

            foreach (var raw in rawSentences)
            {
                var wordMatches = Regex.Matches(raw, @"[a-zA-Zа-яА-ЯёЁ\-]+");
                var words = new List<Word>();
                foreach (Match m in wordMatches)
                {
                    words.Add(new Word(m.Value));
                }

                if (words.Count > 0)
                {
                    var sentence = new Sentence(words)
                    {
                        RawText = raw.Trim()
                    };
                    sentences.Add(sentence);
                }
            }

            return new Text(sentences);
        }

        // Метод для чтения и парсинга текста из файла
        public static Text ParseFile(string filePath)
        {
            string content = File.ReadAllText(filePath, Encoding.UTF8);
            return ParseContent(content);
        }

        public List<Sentence> GetSentencesSortedByWordCount()
        {
            return Sentences.OrderBy(s => s.GetWordCount()).ToList();
        }

        public List<Sentence> GetSentencesSortedByLength()
        {
            return Sentences.OrderBy(s => s.ToString().Length).ToList();
        }

        public HashSet<string> FindWordsInInterrogativeSentences(int length)
        {
            var resultSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var sentence in Sentences)
            {
                if (sentence.IsInterrogative(sentence.RawText))
                {
                    foreach (var word in sentence.Words)
                    {
                        if (word.Value.Length == length)
                        {
                            resultSet.Add(word.Value);
                        }
                    }
                }
            }
            return resultSet;
        }

        public int RemoveWordsStartingWithConsonant(int length)
        {
            int totalRemoved = 0;
            foreach (var sentence in Sentences)
            {
                int before = sentence.Words.Count;
                sentence.RemoveWordsStartingWithConsonant(length);
                totalRemoved += before - sentence.Words.Count;
            }
            return totalRemoved;
        }

        public int ReplaceWordsInSentence(int sentenceIndex, int wordLength, string replacement)
        {
            if (sentenceIndex >= 0 && sentenceIndex < Sentences.Count)
            {
                var sentence = Sentences[sentenceIndex];
                int count = 0;
                for (int i = 0; i < sentence.Words.Count; i++)
                {
                    if (sentence.Words[i].Value.Length == wordLength)
                    {
                        sentence.Words[i] = new Word(replacement);
                        count++;
                    }
                }
                return count;
            }
            return 0;
        }

        public int RemoveStopWords(HashSet<string> stopWords)
        {
            int totalRemoved = 0;
            foreach (var sentence in Sentences)
            {
                int before = sentence.Words.Count;
                sentence.RemoveStopWords(stopWords);
                totalRemoved += before - sentence.Words.Count;
            }
            return totalRemoved;
        }

        public void ToXml(string filePath)
        {
            var serializer = new XmlSerializer(typeof(Text));
            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                serializer.Serialize(writer, this);
            }
        }

        public override string ToString() => string.Join(" ", Sentences.Select(s => s.ToString()));
    }
}