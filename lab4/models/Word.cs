using System.Xml.Serialization;

namespace TextAnalyzerApp.models
{
    [XmlRoot("word")]
    public class Word
    {
        [XmlText]
        public string Value { get; set; }

        public Word() { }

        public Word(string value)
        {
            Value = value;
        }

        public override string ToString() => Value;
    }
}