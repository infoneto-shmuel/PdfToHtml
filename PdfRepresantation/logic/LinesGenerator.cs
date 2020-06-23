using System;
using System.Collections.Generic;
using System.Linq;

namespace PdfRepresantation
{
    class LinesGenerator
    {
        private readonly PageContext pageContext;

        public LinesGenerator(PageContext pageContext)
        {
            this.pageContext = pageContext;
        }


        public IList<PdfTextLineDetails> CreateLines(IList<PdfTextBlock> texts)
        {
            AssignStartEnd(texts);
            return texts
                .OrderBy(t => t.Start)
                .GroupBy(t => (float) Math.Round(t.Bottom, 1))
                .OrderBy(g => g.Key)
                .SelectMany(g => new LineGenarator(pageContext, g).Lines)
                .ToList();
        }

        private void AssignStartEnd(IList<PdfTextBlock> texts)
        {
            foreach (var t in texts)
            {
                if (pageContext.PageRTL)
                {

                    if (t.Value == " " && t.Width < t.CharSpacing)
                    {
                        t.Left = t.Right - t.CharSpacing;
                        t.Width = t.CharSpacing;
                    }
                    t.Start = pageContext.PageWidth - t.Right;
                    t.End =  pageContext.PageWidth - t.Left;
                }
                else
                {
                 
                    if (t.Value == " " && t.Width < t.CharSpacing)
                        t.Width = t.CharSpacing;
                    t.Start = t.Left;
                    t.End = t.Right;
                }
            }
        }
    }
}