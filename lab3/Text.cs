using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TextAnalyzerApp
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

        public List<Sentence> GetSentencesSortedByWordCount()
        {
            return Sentences.OrderBy(s => s.GetWordCount()).ToList();
        }

        public List<Sentence> GetSentencesSortedByLength()
        {
            return Sentences.OrderBy(s => s.ToString().Length).ToList();
        }

        public HashSet<string> FindWordsInInterrogativeSentences(int length, List<string> originalRawSentences)
        {
            var resultSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < Sentences.Count; i++)
            {
                string raw = i < originalRawSentences.Count ? originalRawSentences[i] : "";
                if (Sentences[i].IsInterrogative(raw))
                {
                    foreach (var word in Sentences[i].Words)
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

        public void RemoveWordsStartingWithConsonant(int length)
        {
            foreach (var sentence in Sentences)
            {
                sentence.RemoveWordsStartingWithConsonant(length);
            }
        }

        public void ReplaceWordsInSentence(int sentenceIndex, int wordLength, string replacement)
        {
            if (sentenceIndex >= 0 && sentenceIndex < Sentences.Count)
            {
                Sentences[sentenceIndex].ReplaceWordsOfLength(wordLength, replacement);
            }
        }

        public void RemoveStopWords(HashSet<string> stopWords)
        {
            foreach (var sentence in Sentences)
            {
                sentence.RemoveStopWords(stopWords);
            }
        }

        public void ToXml(string filePath)
        {
            var serializer = new XmlSerializer(typeof(Text));
            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                serializer.Serialize(writer, this);
            }
        }
    }
}
