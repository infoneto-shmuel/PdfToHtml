using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using Point = iText.Kernel.Geom.Point;

namespace PdfRepresantation
{
    public class ShapeParser
    {
        public readonly IList<ShapeDetails> shapes = new List<ShapeDetails>();
        private readonly float pageHeight;

        public ShapeParser(float pageHeight, int pageNumber)
        {
            this.pageHeight = pageHeight;
        }

        public virtual void ParsePath(PathRenderInfo data)
        {
            var shapeOperation = (ShapeOperation) data.GetOperation();
            if (shapeOperation == ShapeOperation.None)
                return;
            var fillColor = ColorManager.Instance.GetColor(data.GetFillColor());
            if (shapeOperation != ShapeOperation.Stroke && (fillColor == null || fillColor == Color.Black))
                return;
            
            var strokeColor = ColorManager.Instance.GetColor(data.GetStrokeColor());
            var lineWidth = data.GetLineWidth();
            var lineCap = data.GetLineCapStyle();
            var ctm = data.GetCtm();
            foreach (var subpath in data.GetPath().GetSubpaths())
            {
                var segments = subpath.GetSegments();
                if (segments.Count == 0)
                    continue;

                shapes.Add(new ShapeDetails
                {
                    ShapeOperation = shapeOperation,
                    StrokeColor = strokeColor,
                    FillColor = fillColor,
                    LineWidth = lineWidth,
                    Lines = segments.Select(shape => ConvertLine(shape, ctm)).ToArray(),
                });
            }
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
                Y = pageHeight - vector.Get(Vector.I2)
            };
        }
    }
}