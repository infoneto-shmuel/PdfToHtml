using System;
using System.Drawing;
using System.Text;
using PdfRepresantation.Internals.Helpers;
using PdfRepresantation.Model.Enums;
using PdfRepresantation.Model.Pdf;

namespace PdfRepresantation.Html
{
    public class PdfShapeSvgHtmlWriter : PdfShapeHtmlWriter
    {
        public override void AddShapes(PageDetails page, StringBuilder sb)
        {
            sb.Append(@"
    <svg class=""canvas"" height=""").Append(Math.Round(page.Height))
                .Append("\" width=\"").Append(Math.Round(page.Width))
                .Append("\" style=\"margin-top:-").Append(Math.Round(page.Height) + 2)
                .Append("px;\">");
            foreach (var shape in page.Shapes)
            {
                AddShape(shape, sb);
            }

            sb.Append(@"
    </svg>");
        }

        private void AddPoint(ShapePoint p, StringBuilder sb)
        {
            sb.Append(" ").Append(Math.Round(p.X,2)).Append(" ").Append(Math.Round(p.Y,2));
        }

        private void AddShape(ShapeDetails shape, StringBuilder sb)
        {
            if(shape.ShapeOperation==ShapeOperation.None)
                return;
            ShapePoint lastEnd = null;
            sb.Append(@"
        <path d=""");
            foreach (var line in shape.Lines)
            {
                if (!line.Start.Equals(lastEnd))
                {
                    sb.Append("M");
                    AddPoint(line.Start, sb);
                }

                if (line.CurveControlPoint1 == null)
                    sb.Append(" L");
                else
                {
                    if (line.CurveControlPoint2 != null)
                    {
                        sb.Append(" C");
                        AddPoint(line.CurveControlPoint1, sb);
                        sb.Append(",");
                        AddPoint(line.CurveControlPoint2, sb);
                        sb.Append(",");
                    }
                    else
                    {
                        sb.Append(" Q");
                        AddPoint(line.CurveControlPoint1, sb);
                        sb.Append(",");
                    }
                }

                AddPoint(line.End, sb);
                lastEnd = line.End;
            }

            sb.Append("\" stroke-width=\"").Append(shape.LineWidth)
                .Append("\" fill=\"");
            AppendColor(shape.ShapeOperation==ShapeOperation.Stroke?null:shape.FillColor, sb);
           if(shape.EvenOddRule)
                sb.Append("\" fill-rule=\"evenodd");
            sb.Append("\" stroke=\"");
            AppendColor(shape.ShapeOperation==ShapeOperation.Fill?null:shape.StrokeColor, sb);
            sb.Append("\"/>");
        }

        private void AppendColor(Color? color, StringBuilder sb)
        {
            if (color.HasValue)
            {
                ColorHelper.AppendColor(color.Value, sb);
            }
            else
            {
                sb.Append("transparent");
            }
        }

        public override void AddScript(StringBuilder sb)
        {
        }
    }
}