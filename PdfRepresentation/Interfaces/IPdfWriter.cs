using PdfRepresentation.Model.Pdf;

namespace PdfRepresentation.Interfaces
{
    public interface IPdfWriter
    {
        string ConvertPdf(PdfDetails pdf);

        string ConvertPage(PageDetails page);

    }
}