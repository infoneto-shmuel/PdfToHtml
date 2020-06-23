using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using Point = iText.Kernel.Geom.Point;

namespace PdfRepresantation
{
    public class ShapeParser
    {
        public readonly IList<ShapeDetails> shapes = new List<ShapeDetails>();
        private readonly PageContext pageContext;

        internal ShapeParser(PageContext pageContext)
        {
            this.pageContext = pageContext;
        }

        public virtual void ParsePath(PathRenderInfo data)
        {
            var shapeOperation = (ShapeOperation) data.GetOperation();
            if (shapeOperation == ShapeOperation.None)
                return;
            bool evenOddRule = data.GetRule() == PdfCanvasConstants.FillingRule.EVEN_ODD;
            var fillColor = ColorManager.Instance.GetColor(data.GetFillColor());
            if (shapeOperation != ShapeOperation.Stroke && (fillColor == null || fillColor == Color.Black))
                return;

            var strokeColor = ColorManager.Instance.GetColor(data.GetStrokeColor());
            var lineWidth = data.GetLineWidth();
            var lineCap = data.GetLineCapStyle();
            var ctm = data.GetCtm();
            var lines = ConvertLines(data.GetPath(), ctm).ToArray();
            if(lines.Length==0)
                return;
            

            var shapeDetails = new ShapeDetails
            {
                ShapeOperation = shapeOperation,
                StrokeColor = strokeColor,
                FillColor = fillColor,
                LineWidth = lineWidth,
                EvenOddRule = evenOddRule,
                Lines = lines
            };
            if (Log.DebugSupported)
            {
                Log.Debug($"shape: {shapeDetails}");
            }
            shapes.Add(shapeDetails);
        }

        protected IEnumerable<ShapeLine> ConvertLines(Path path, Matrix ctm)
        {
            return from subpath in path.GetSubpaths()
                from line in subpath.GetSegments() 
                select ConvertLine(line, ctm);
        }

        protected ShapeLine ConvertLine(IShape line, Matrix ctm)
        {
            var points = line.GetBasePoints()
                .Select(p => ConvertPoint(p, ctm))
                .ToArray();
            var result = new ShapeLine
            {
                Start = points[0],
                End = points[points.Length - 1]
            };
            if (points.Length > 2)
            {
                result.CurveControlPoint1 = points[1];
                if (points.Length > 3)
                    result.CurveControlPoint2 = points[2];
            }

            return result;
        }

        protected ShapePoint ConvertPoint(Point p, Matrix ctm)
        {
            Vector vector = new Vector((float) p.x, (float) p.y, 1);
            vector = vector.Cross(ctm);
            return new ShapePoint
            {
                X = vector.Get(Vector.I1),
                Y = pageContext.PageHeight - vector.Get(Vector.I2)
            };
        }
    }
}