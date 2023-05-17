using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using System.Collections.Generic;
using System.Linq;
using PdfRepresentation.Logging;
using PdfRepresentation.Model.Pdf;

namespace PdfRepresentation.Logic
{
    class LinkManager
    {
        class Link
        {
            public string uri;
            public float top, bottom, left, right;
        }
        protected readonly PageContext pageContext;
        private readonly List<Link> links=new List<Link>();

        public LinkManager(PageContext pageContext)
        {
             this.pageContext = pageContext;
        }

        public void AssignLink(PdfTextBlock current, TextResult text, ref LinkResult link)
        {
            if (current.Link != null)
            {
                if (!string.Equals(link?.Link, current.Link))
                {
                    if(link!=null&&Log.DebugSupported)
                        Log.Debug("link:"+link);
                    link = new LinkResult {Link = current.Link};
                }
                link.Children.Add(text);
                text.LinkParent = link;
            }
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
            foreach (var link in pageContext.Page.GetAnnotations().OfType<PdfLinkAnnotation>())
            {
                var uri = link.GetAction()?.GetAsString(PdfName.URI);
                if (uri == null)
                    continue;
                var linkCoordinateArray = link.GetPdfObject().GetAsArray(PdfName.Rect);
                links.Add(new Link
                {
                    uri = uri.ToString(),
                    left = Parse(linkCoordinateArray, 0)-1,
                    top =pageContext.PageHeight- Parse(linkCoordinateArray, 1)-1,
                    right = Parse(linkCoordinateArray, 2)+1,
                    bottom =pageContext.PageHeight- Parse(linkCoordinateArray, 3)+1,
                });
            }

            float Parse(PdfArray array, int index)
            {
                return float.Parse(array.Get(index).ToString());
            }
        }
    }
}