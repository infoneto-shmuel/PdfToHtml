using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using PdfRepresentation.Model.Pdf;

namespace PdfRepresentation.Model.Common
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

        [XmlElement("Cell")]
        public TextLineDetails[] Cells
        {
            get { return TextLineDetails.ToArray(); }
            set { TextLineDetails = value?.ToList() ?? new List<TextLineDetails>(); }
        }

        [XmlText]
        public string InnerText
        {
            get
            {
                return string.Join(" ", TextLineDetails.Select(d => d.InnerText)).Trim();
            }
            set
            {

            }
        }

        public bool ShouldSerializeInnerText()
        {
            return PdfModel.ShouldSerializeAll;
        }


    }
}