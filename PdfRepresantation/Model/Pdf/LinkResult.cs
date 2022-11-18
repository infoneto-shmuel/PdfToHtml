using System.Collections.Generic;

namespace PdfRepresantation.Model.Pdf
{
    public class LinkResult : TextResult
    {
        public List<TextResult> Children { get; set; } = new List<TextResult>();
        public string Link { get; set; }
        public override string ToString() => string.Join("", Children) + "=>" + Link;
    }
}