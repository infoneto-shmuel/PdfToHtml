using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using PdfRepresantation.Model.Pdf;

namespace PdfRepresantation.Model
{
    public class RowModel
    {
        [JsonIgnore]
        [XmlIgnore]
        internal List<TextLineDetails> TextLineDetails { get; set; } = new List<TextLineDetails>();

        public override string ToString()
        {
            return TextLineDetails.Count.ToString();
        }

        [XmlElement("Cells")]
        public TextLineDetails[] Cells
        {
            get { return TextLineDetails.ToArray(); }
            set { TextLineDetails = value?.ToList() ?? new List<TextLineDetails>(); }
        }
    }
}