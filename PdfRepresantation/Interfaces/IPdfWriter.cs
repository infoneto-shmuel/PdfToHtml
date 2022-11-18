using PdfRepresantation.Model.Pdf;

namespace PdfRepresantation.Interfaces
{
    public interface IPdfWriter
    {
        string ConvertPdf(PdfDetails pdf);

        string ConvertPage(PageDetails page);

    }
}