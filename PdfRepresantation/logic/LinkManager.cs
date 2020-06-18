using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace PdfRepresantation
{
    class LinkManager
    {
        class Link
        {
            public string uri;
            public float top, bottom, left, right;
        }
        private readonly float pageHeight;
        protected readonly PdfPage page;
        private readonly List<Link> links=new List<Link>();

        public LinkManager(float pageHeight, PdfPage page)
        {
            this.pageHeight = pageHeight;
            this.page = page;
        }

        public void AssignLink(PdfTextBlock text)
        {
            var link = links.FirstOrDefault(l => Match(l, text));
            if (link != null)
                text.Link = link.uri;
        }

        private bool Match(Link link, PdfTextBlock text)
        {
            return link.bottom<=text.Bottom&&
                   link.top>=text.Top&&
                   link.left<=text.Left&&
                   link.right >= text.Right;
        }

       public void FindLinks()
        {
            foreach (var link in page.GetAnnotations().OfType<PdfLinkAnnotation>())
            {
                var uri = link.GetAction()?.GetAsString(PdfName.URI);
                if (uri == null)
                    continue;
                var linkCoordinateArray = link.GetPdfObject().GetAsArray(PdfName.Rect);
                links.Add(new Link
                {
                    uri = uri.ToString(),
                    left = Parse(linkCoordinateArray, 0)-1,
                    top =pageHeight- Parse(linkCoordinateArray, 1)-1,
                    right = Parse(linkCoordinateArray, 2)+1,
                    bottom =pageHeight- Parse(linkCoordinateArray, 3)+1,
                });
            }

            float Parse(PdfArray array, int index)
            {
                return float.Parse(array.Get(index).ToString());
            }
        }
    }
}