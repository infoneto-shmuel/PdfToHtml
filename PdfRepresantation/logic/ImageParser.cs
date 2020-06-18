using System.Collections.Generic;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas.Parser.Data;

namespace PdfRepresantation
{
    public class ImageParser
    {
        public readonly IList<PdfImageDetails> images = new List<PdfImageDetails>();
        private readonly float pageHeight;
        private readonly float pageWidth;

        public ImageParser(float pageHeight, float pageWidth)
        {
            this.pageHeight = pageHeight;
            this.pageWidth = pageWidth;
        }

        public virtual void ParseImage(ImageRenderInfo data)
        {
            var bytes = data.GetImage().GetImageBytes();
            // var start = data.GetStartPoint();
            var ctm = data.GetImageCtm();
            var width = ctm.Get(Matrix.I11);
            var height = ctm.Get(Matrix.I22);
            var x=ctm.Get(Matrix.I31 );
            var y=pageHeight- ctm.Get(Matrix.I32 );
            images.Add(new PdfImageDetails
            {
                Buffer = bytes,
                Bottom = y,
                Top = y-height,
                Left = x,
                Right = pageWidth-x-width,
                Width = width,
                Height = height,
            });
        }
    }
}