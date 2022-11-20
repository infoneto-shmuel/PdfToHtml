using PdfRepresantation.Model.Pdf;

namespace PdfRepresantation.Interfaces
{
    public interface IPdfWriterEx: IPdfWriter
    {
        string ConvertPdf(PdfDetails pdf, bool shouldSerializeAll);

        string ConvertPage(PageDetails page, bool shouldSerializeAll);

    }
}