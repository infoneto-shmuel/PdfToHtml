using System.Collections.Generic;
using System.Xml.Serialization;

namespace PdfRepresantation.Model.Pdf
{
    public class TextLineDetails : DetailsItem
    {
        public List<TextResult> Texts { get; set; } = new List<TextResult>();

        public string InnerText => string.Join("", Texts);
        public override string ToString() => InnerText + "\r\n";
    }
}