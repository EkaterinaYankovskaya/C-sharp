using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace TextAnalyzerApp
{
    public class Sentence
    {
        [XmlElement("word")]
        public List<Word> Words { get; set; } = new List<Word>();

        public Sentence() { }

        public Sentence(List<Word> words)
        {
            Words = words;
        }

        public int GetWordCount() => Words.Count;

        public bool IsInterrogative(string originalText)
        {
            return originalText.Trim().EndsWith("?");
        }

        public void RemoveWordsStartingWithConsonant(int length)
        {
            string consonants = "бвгджзклмнпрстфхцчшщbcdfghjklmnpqrstvwxyzБВГДЖЗКЛМНПРСТФХЦЧШЩBCDFGHJKLMNPQRSTVWXYZ";
            Words.RemoveAll(w => w.Value.Length == length && consonants.Contains(w.Value[0]));
        }

        public void ReplaceWordsOfLength(int length, string replacement)
        {
            for (int i = 0; i < Words.Count; i++)
            {
                if (Words[i].Value.Length == length)
                {
                    Words[i] = new Word(replacement);
                }
            }
        }

        public void RemoveStopWords(HashSet<string> stopWords)
        {
            Words.RemoveAll(w => stopWords.Contains(w.Value.ToLower()));
        }

        public override string ToString()
        {
            return string.Join(" ", Words.Select(w => w.Value));
        }
    }
}