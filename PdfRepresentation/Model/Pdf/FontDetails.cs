using System.Xml.Serialization;

namespace PdfRepresentation.Model.Pdf
{
    public class FontDetails
    {
        [XmlAttribute]
        public string FontFamily { get; set; }
        [XmlAttribute]
        public string BasicFontFamily { get; set; }
        [XmlAttribute]
        public bool Bold { get; set; }
        [XmlAttribute]
        public bool Italic { get; set; }
    }
}