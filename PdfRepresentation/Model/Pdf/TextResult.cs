using System.Drawing;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace PdfRepresentation.Model.Pdf
{
    public class TextResult
    {
        public string Value { get; set; }
        public float FontSize { get; set; }
        public FontDetails Font { get; set; }
        [JsonIgnore]
        [XmlIgnore]
        public LinkResult LinkParent { get; set; }
        public Color? StrokeColore { get; set; }
        public override string ToString() => Value;
    }
}