using System;
using System.Drawing;
using System.Linq;
using System.Text;

namespace PdfRepresantation
{
    public    class PdfShapeCanvasHtmlWriter:PdfShapeHtmlWriter
    {
        public override void AddShapes(PdfPageDetails page, StringBuilder sb)
        {
                
            sb.Append(@"
    <canvas class=""canvas"" id=""canvas-").Append(page.PageNumber)
                .Append("\" style=\"width: ")
                .Append(Math.Round(page.Width))
                .Append("px;height:").Append(Math.Round(page.Height))
                .Append("px;margin-top:-").Append(Math.Round(page.Height) + 2)
                .Append("px;\" width=\" ")
                .Append(Math.Round(page.Width))
                .Append("\" height=\"").Append(Math.Round(page.Height)).Append("\"></canvas>");

            sb.Append(@"
    <script>
        currentCanvas= document.getElementById('canvas-").Append(page.PageNumber).Append(@"');");
            foreach (var shape in page.Shapes)
            {
                AddShape(shape, sb);
            }

            sb.Append(@"
    </script>");
        }
        protected virtual void AddShape(ShapeDetails shape, StringBuilder sb)
        {
            sb.Append(@"
        draw([");
            for (var i = 0; i < shape.Lines.Count; i++)
            {
                if (i != 0)
                    sb.Append(",");
                sb.Append("[");
                var points =  shape.Lines[i].AllPoints.ToArray();
                for (var j = 0; j < points.Length; j++)
                {
                    if (j != 0)
                        sb.Append(",");
                    var p = points[j];
                    sb.Append(p.X).Append(",").Append(p.Y);
                }
                sb.Append("]");
            }

            sb.Append("],").Append((int) shape.ShapeOperation).Append(",");
            AppendColor(shape.StrokeColor, sb);
            sb.Append(",");
            AppendColor(shape.FillColor, sb);
            sb.Append(",").Append(shape.LineWidth)
                .Append(",'").Append(shape.EvenOddRule?"evenodd":"nonzero").Append("',").Append("null").Append(");");
        }

        public override void AddScript(StringBuilder sb)
        {
            sb.Append(@"
    <script>
        var currentCanvas;
        function draw(lines,operation,strokeColor, fillColor, lineWidth,fillRule,lineCap) {
             if (!currentCanvas.getContext)
                 return;
             var ctx = currentCanvas.getContext('2d');
             if (lineWidth) 
                 ctx.lineWidth = lineWidth;
             if (!lineCap) 
                 ctx.lineCap= lineCap;
             ctx.fillStyle=fillColor||'white';               
             ctx.strokeStyle=strokeColor||'black';
             ctx.beginPath();
            var position={x:'-',y:'-'};
             var drawLine=function (line) {
                 if (position.x!=line[0]||position.y!=line[1])
                     ctx.moveTo(line[0], line[1]);
                 switch (line.length)
                 {
                     case 4:ctx.lineTo(line[2], line[3]);break;
                     case 6:ctx.quadraticCurveTo(line[2], line[3],line[4], line[5]);break;
                     case 8:ctx.bezierCurveTo(line[2], line[3],line[4], line[5],line[6], line[7]);break;
                 }
                 position.x=line[line.length-2];
                 position.y=line[line.length-1];
             }; 
             for (var i = 0; i < lines.length; i++) 
                 drawLine(lines[i]);
             switch (operation) {
                 case 1:ctx.stroke();break;
                 case 2:ctx.fill(fillRule);break;
                 case 3:ctx.stroke();
                     ctx.fill(fillRule);break;
             }
        }
    </script>");
        }
        protected   void AppendColor(Color? color, StringBuilder sb)
        {
            if (color.HasValue)
            {
                sb.Append("'");
                PdfHtmlWriter.AppendColor(color.Value, sb);
                sb.Append("'");
            }
            else
                sb.Append("null");
        }

    }
}