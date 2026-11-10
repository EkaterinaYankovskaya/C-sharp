using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace TextAnalyzerApp.models
{
    public class Sentence
    {
        [XmlIgnore]
        public string RawText { get; set; } = string.Empty;

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
            return !string.IsNullOrEmpty(originalText) && originalText.EndsWith("?");
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
            if (Words == null || Words.Count == 0)
                return RawText;

            // Собираем оставшиеся слова через пробел
            string wordsStr = string.Join(" ", Words.Select(w => w.Value));

            // Берем знак препинания из конца оригинального RawText (если он там есть)
            if (!string.IsNullOrEmpty(RawText))
            {
                char lastChar = RawText[RawText.Length - 1];
                if (lastChar == '?' || lastChar == '!' || lastChar == '.')
                {
                    return wordsStr + lastChar;
                }
            }

            return wordsStr;
        }
    }
}