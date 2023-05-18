using System.Collections.Generic;
using System.Xml.Serialization;
using Newtonsoft.Json;
using PdfRepresentation.Model.Pdf;

namespace PdfRepresentation.Model.Common
{
    public class PdfModel
    {
        [XmlIgnore]
        [JsonIgnore]
        [XmlElement("Fonts")]
        public List<FontDetails> Fonts { get; set; }
        [XmlArray("Pages")]
        public List<PageModel> Pages { get; set; }

        public static bool ShouldSerializeAll { get; set; } = false;
    }
}