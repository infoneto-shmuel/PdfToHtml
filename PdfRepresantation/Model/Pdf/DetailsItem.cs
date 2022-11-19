using System.Xml.Serialization;

namespace PdfRepresantation.Model.Pdf
{
    public class DetailsItem
    {
        [XmlAttribute]
        public float Left { get; set; }
        [XmlAttribute]
        public float Right { get; set; }
        [XmlAttribute]
        public float Bottom { get; set; }
        [XmlAttribute]
        public float Top { get; set; }
        [XmlAttribute]
        public float Width { get; set; }
        [XmlAttribute]
        public float Height { get; set; }
    }
}