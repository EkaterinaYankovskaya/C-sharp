using System.Xml.Serialization;

namespace TextAnalyzerApp
{
    public class Punctuation
    {
        [XmlText]
        public string Value { get; set; }

        public Punctuation() { }

        public Punctuation(string value)
        {
            Value = value;
        }

        public override string ToString() => Value;
    }
}