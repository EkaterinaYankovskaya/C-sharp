using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TextAnalyzerApp
{
    public class TextParser
    {
        public static (Text text, List<string> rawSentences) ParseContent(string content)
        {
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
                    sentences.Add(new Sentence(words));
                }
            }

            return (new Text(sentences), rawSentences);
        }
    }
}
