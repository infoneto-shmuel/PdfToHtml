using System.Threading.Tasks;

namespace PdfRepresantation
{
    public interface IPdfConverter
    {
        string ConvertToHtml(byte[] pdf);
        string ConvertToText(byte[] pdf);
        Task<string> ConvertToHtmlAsync(byte[] pdf);
        Task<string> ConvertToTextAsync(byte[] pdf);
    }

    
}