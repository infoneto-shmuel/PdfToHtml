using System.Threading.Tasks;

namespace PdfRepresantation.Interfaces.Shared
{
    public interface IPdfConverter
    {
        string ConvertToText(byte[] pdf);
        Task<string> ConvertToTextAsync(byte[] pdf);
    }


}