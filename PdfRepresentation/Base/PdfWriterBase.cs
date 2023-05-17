using System.IO;
using PdfRepresentation.Interfaces;
using PdfRepresentation.Model.Pdf;

namespace PdfRepresentation.Base
{
    public abstract class PdfWriterBase : IPdfWriter
    {
        public void SaveAs(PdfDetails pdf, string path)
        {
            var content = ConvertPdf(pdf);
            File.WriteAllText(path, content);
        }
        
        public abstract string ConvertPdf(PdfDetails pdf);

        public abstract string ConvertPage(PageDetails page);

    }
}