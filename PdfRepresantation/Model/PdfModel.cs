using System.Collections.Generic;
using System.Xml.Serialization;
using PdfRepresantation.Model.Pdf;

namespace PdfRepresantation.Model
{
    public class PdfModel
    {
        [XmlElement("Fonts")]
        public List<FontDetails> Fonts { get; set; }
        [XmlArray("Pages")]
        public List<PageModel> Pages { get; set; }
    }
}