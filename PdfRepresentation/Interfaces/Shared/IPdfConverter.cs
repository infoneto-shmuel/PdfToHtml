using System.Threading.Tasks;

namespace PdfRepresentation.Interfaces.Shared
{
    public interface IPdfConverter
    {
        string ConvertToText(byte[] pdf);
        Task<string> ConvertToTextAsync(byte[] pdf);
    }


}