using System;
using System.Net.Http;
using System.Text;
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

    public class PdfRepresantationClient : IPdfConverter
    {
        private const string DefaultUrl = "http://localhost:51717/";
        private readonly string BaseUrl;

        public PdfRepresantationClient(string baseUrl = DefaultUrl)
        {
            BaseUrl = baseUrl;
        }


        public string ConvertToHtml(byte[] pdf) => ReadAsync(pdf, true).GetAwaiter().GetResult();
        public string ConvertToText(byte[] pdf) => ReadAsync(pdf, false).GetAwaiter().GetResult();
        public Task<string> ConvertToHtmlAsync(byte[] pdf) => ReadAsync(pdf, true);
        public Task<string> ConvertToTextAsync(byte[] pdf) => ReadAsync(pdf, false);

        class ServerException : Exception
        {
            private readonly string Text;
            public override string StackTrace { get; }
            public ServerException(string message, string stack, string text) : base(message)
            {
                Text = text;
                StackTrace = stack;
            }

            public override string ToString() => Text;
        }

        async Task<string> ReadAsync(byte[] pdf, bool html)
        {
            var data = Convert.ToBase64String(pdf);
            string json = "{\"ContentBase64\":\"" + data + "\"}";
            var url = BaseUrl + (html ? "ConvertToHtml" : "ConvertToText");


            string stringResult;
            using (var client = new HttpClient())
            {
                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                try
                {
                    var response = await client.PostAsync(url, stringContent);
                    stringResult = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        var parts = stringResult.Split('\u0001');
                        if(parts.Length!=3)
                            throw new Exception(stringResult);
                        throw new ServerException(parts[0],parts[1],parts[2]);
                    }
                }
                catch (AggregateException e)
                {
                    throw e.InnerException ?? e;
                }
            }

            return stringResult;
        }
    }
}