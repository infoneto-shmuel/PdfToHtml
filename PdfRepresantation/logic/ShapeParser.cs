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
            ShapeLine ConvertLine(Line line)
            {
                var points = line.GetBasePoints()
                    .Select(p => new ShapePoint {X = width*p.x+x,
                        Y =pageHeight- height*(p.y)-y})
                    .ToArray();
                return new ShapeLine
                {
                    Start = points[0],
                    End = points[1]
                };

            }
            foreach (var subpath in data.GetPath().GetSubpaths())
            {
                if(!subpath.IsClosed())
                   continue;
                var segments = subpath.GetSegments();
                if(segments.Count==0)
                    continue;
                var lines=segments.OfType<Line>().ToArray();
                if(lines.Length==0)
                    continue;
                
                shapes.Add(new ShapeDetails
                {
                    ShapeOperation = shapeOperation,
                    StrokeColor = strokeColor,
                    FillColor = fillColor,
                    LineWidth = lineWidth,                   
                    Lines = lines.Select(ConvertLine).ToArray(),
                });
            }
            
        }

        private ShapeLine ConvertLine(Line line, float lineWidth)
        {
            var points = line.GetBasePoints()
                .Select(p => new ShapePoint {X = p.x+2, Y =pageHeight- (p.y)+lineWidth+2})
                .ToArray();
            return new ShapeLine
            {
                Start = points[0],
                End = points[1]
            };

        }
    }
}