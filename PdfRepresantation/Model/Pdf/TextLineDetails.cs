using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace PdfRepresantation.Model.Pdf
{
    public class TextLineDetails : DetailsItem
    {
        public List<TextResult> Texts { get; set; } = new List<TextResult>();

        public bool ShouldSerializeTexts()
        {
            return PdfModel.ShouldSerializeAll;
        }

        [XmlText]
        public string InnerText
        {
            get { return string.Join(" ", Texts.Select(t => t.Value)).Trim(); }
            set { }
        }

        public override string ToString() => InnerText + "\r\n";
    }
}