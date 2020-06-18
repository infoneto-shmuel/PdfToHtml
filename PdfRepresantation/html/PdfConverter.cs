using System.Threading.Tasks;

namespace PdfRepresantation
{
    public class PdfConverter : IPdfConverter
    {
        PdfHtmlWriter writer=new PdfHtmlWriter();
        public string ConvertToHtml(byte[] pdf)
        {
            var details=PdfDetailsFactory.Create(pdf);
            return writer.ConvertPdf(details);
        }

        public string ConvertToText(byte[] pdf)
        {
            var details=PdfDetailsFactory.Create(pdf);
            return details.ToString();
        }

        public Task<string> ConvertToHtmlAsync(byte[] pdf)
        {
            return Task.FromResult(ConvertToHtml(pdf));
        }

        public Task<string> ConvertToTextAsync(byte[] pdf)
        {
            return Task.FromResult(ConvertToText(pdf));
        }
    }
}