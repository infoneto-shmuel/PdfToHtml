using iText.Kernel.Pdf;

namespace PdfRepresantation
{
    class PageContext
    {
        public bool PageRTL { get; set; }
        public float PageHeight { get; set; }
        public float PageWidth { get; set; }
        public PdfPage Page { get; set; }
        public int PageNumber { get; set; }

        public LinkManager LinkManager { get; set; }
    }
}