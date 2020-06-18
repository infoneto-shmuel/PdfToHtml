using System.Collections.Generic;

namespace PdfRepresantation
{
    public class PdfDetails
    {
        public IList<PdfFontDetails> Fonts { get; set; }
        public IList<PdfPageDetails> Pages { get; set; }

        public override string ToString() =>
            string.Join("\r\n---------------------------------------------------------------------------\r\n", Pages) +
            "\r\n";
    }
}