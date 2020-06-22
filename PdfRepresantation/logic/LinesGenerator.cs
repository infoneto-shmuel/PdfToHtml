using System;
using System.Collections.Generic;
using System.Linq;

namespace PdfRepresantation
{
    class LinesGenerator
    {
        private readonly float pageWidth;

        public LinesGenerator(float pageWidth)
        {
            this.pageWidth = pageWidth;
        }


        public IList<PdfTextLineDetails> CreateLines(IList<PdfTextBlock> texts, bool pageRTL)
        {
            AssignStartEnd(texts, pageRTL);
            return texts
                .OrderBy(t => t.Start)
                .GroupBy(t => (float) Math.Round(t.Bottom, 2))
                .OrderBy(g => g.Key)
                .SelectMany(g => new LineGenarator(pageWidth, pageRTL, g).Lines)
                .ToList();
        }

        private void AssignStartEnd(IList<PdfTextBlock> texts, bool pageRTL)
        {
            foreach (var t in texts)
            {
                if (pageRTL)
                {
                    t.Start = pageWidth - t.Right;
                    t.End = pageWidth - t.Left;
                }
                else
                {
                    t.Start = t.Left;
                    t.End = t.Right;
                }
            }
        }
    }
}