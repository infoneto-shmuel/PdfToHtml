using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace PdfRepresantation.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls("http://localhost:51717/")
                .UseStartup<Startup>();
    }
}
