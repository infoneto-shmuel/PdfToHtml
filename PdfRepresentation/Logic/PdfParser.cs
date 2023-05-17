using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using System.Collections.Generic;
using System.Linq;
using PdfRepresentation.Logging;
using PdfRepresentation.Model.Pdf;

namespace PdfRepresentation.Logic
{
    public class PdfParser
    {
        public PdfDetails Parse(PdfDocument source)
        {
            var numberOfPages = source.GetNumberOfPages();
            PdfDetails details = new PdfDetails
            {
                Pages = new PageDetails[numberOfPages].ToList(),
            };
            for (int pageNumber = 1; pageNumber <= numberOfPages; pageNumber++)
            {
                Log.Info("parsing page number "+pageNumber);
                var page = source.GetPage(pageNumber);
                var pageParser = CreatePageParser(page, pageNumber);
                new PdfCanvasProcessor(pageParser,
                    new Dictionary<string, IContentOperator>())
                    .ProcessPageContent(page);
                var pageDetails = pageParser.CreatePageDetails();
                pageDetails.Parent = details;
                details.Pages[pageNumber - 1] = pageDetails;
            }
            details.Fonts=details.Pages.SelectMany(p=>p.Fonts).Distinct().ToList();
//            foreach (var f in details.Fonts)
//            {
//                
//            Console.WriteLine(f.BasicFontFamily+"\t\t"+f.FontFamily);
//            }
            return details;
        }

        protected virtual PageParser CreatePageParser(PdfPage page, int pageNumber)
            => new PageParser(page, pageNumber);
    }
}