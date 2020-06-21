using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas.Parser.Data;

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
            if(shapeOperation==ShapeOperation.None)
                return;
            var fillColor = ColorManager.Instance.GetColor(data.GetFillColor());
            if(shapeOperation!=ShapeOperation.Stroke&&(fillColor==null||fillColor==Color.Black))
                return;
            
            var strokeColor = ColorManager.Instance.GetColor(data.GetStrokeColor());
            var lineWidth = data.GetLineWidth();
            var lineCap = data.GetLineCapStyle();
            var ctm=data.GetCtm();
            var width = ctm.Get(Matrix.I11);
            var height = ctm.Get(Matrix.I22);
            var x=ctm.Get(Matrix.I31 );
            var y= ctm.Get(Matrix.I32 );
            ShapeLine ConvertLine(IShape line)
            {
                var points = line.GetBasePoints()
                    .Select(p => new ShapePoint {X = width*p.x+x,
                        Y =pageHeight- height*(p.y)-y})
                    .ToArray();
                var result = new ShapeLine
                {
                    Start = points[0],
                    End = points[points.Length-1]
                };
                if (points.Length > 2)
                {
                    result.CurveControlPoint1 = points[1];
                    if(points.Length>3)
                    result.CurveControlPoint2 = points[2];
                }
                return result;

            }
            foreach (var subpath in data.GetPath().GetSubpaths())
            {
                var segments = subpath.GetSegments();
                if(segments.Count==0)
                    continue;
                
                shapes.Add(new ShapeDetails
                {
                    ShapeOperation = shapeOperation,
                    StrokeColor = strokeColor,
                    FillColor = fillColor,
                    LineWidth = lineWidth,                   
                    Lines = segments.Select(ConvertLine).ToArray(),
                });
            }
            
        }

    }
}