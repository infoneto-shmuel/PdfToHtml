using System.Collections.Generic;

namespace PdfRepresantation.Model.Pdf
{
    public class PdfDetails
    {
        public List<FontDetails> Fonts { get; set; }
        public List<PageDetails> Pages { get; set; }

        public override string ToString() =>
            string.Join("\r\n---------------------------------------------------------------------------\r\n", Pages) +
            "\r\n";
    }
}