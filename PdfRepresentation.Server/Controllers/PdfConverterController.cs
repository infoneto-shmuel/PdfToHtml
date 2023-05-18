using System;
using Microsoft.AspNetCore.Mvc;
using PdfRepresentation.Html;
using PdfRepresentation.Logic;

namespace PdfRepresentation.Server.Controllers
{
    [Route("")]
    [ApiController]
    public class PdfConverterController : ControllerBase
    {
        private static readonly PdfHtmlWriter htmlWriter = new PdfHtmlWriter();

        public class FileRequest
        {
            public string ContentBase64 { get; set; }
        }

        [HttpPost("ConvertToHtml")]
        public ActionResult<string> ConvertToHtml([FromBody] FileRequest pdf)
        {
            try
            {
                var buffer = Convert.FromBase64String(pdf.ContentBase64);
                var details = PdfDetailsFactory.Create(buffer);
                var result = htmlWriter.ConvertPdf(details, true);
                return result;
            }
            catch (Exception e)
            {
                base.Response.StatusCode = 500;
                return e.Message+'\u0001'+e.StackTrace+'\u0001'+e;
            }
        }

        [HttpGet("Test")]
        public ActionResult<string> Test()
        {
            return "success";
        }

        [HttpPost("ConvertToText")]
        public ActionResult<string> ConvertToText([FromBody] FileRequest pdf)
        {
            try
            {
                var buffer = Convert.FromBase64String(pdf.ContentBase64);
                var details = PdfDetailsFactory.Create(buffer);
                return details.ToString();
            }
            catch (Exception e)
            {
                base.Response.StatusCode = 500;
                return e.ToString();
            }
        }

        [HttpPost("ConvertToHtml/Base64")]
        public ActionResult<string> ConvertToHtml([FromBody] string base64pdf)
        {
            return ConvertToHtml(new FileRequest {ContentBase64 = base64pdf});
        }

        [HttpPost("ConvertToText/Base64")]
        public ActionResult<string> ConvertToText([FromBody] string base64pdf)
        {
            return ConvertToText(new FileRequest {ContentBase64 = base64pdf});
        }
    }
}