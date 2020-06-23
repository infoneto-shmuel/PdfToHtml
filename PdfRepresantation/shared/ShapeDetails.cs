using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace PdfRepresantation
{
    public enum ShapeOperation
    {
        None = 0,
        Stroke = 1,
        Fill = 2,
        Both = 3
    }

    public class ShapeDetails
    {
        public ShapeOperation ShapeOperation { get; set; }
        public Color? StrokeColor { get; set; }
        public Color? FillColor { get; set; }
        public IList<ShapeLine> Lines = new List<ShapeLine>();
        public float LineWidth { get; set; }
        public bool EvenOddRule { get; set; }

        public override string ToString()
        {
            var minX = Lines.Min(l => l.AllPoints.Min(p => p.X));
            var minY = Lines.Min(l => l.AllPoints.Min(p => p.Y));
            var maxX = Lines.Max(l => l.AllPoints.Max(p => p.X));
            var maxY = Lines.Max(l => l.AllPoints.Max(p => p.Y));
            var sb = new StringBuilder();
            sb.Append("").Append("")
                .Append("start:")
                .Append(minX.ToString("F2")).Append(",")
                .Append(minY.ToString("F2"))
                .Append(" end:")
                .Append(maxX.ToString("F2")).Append(",")
                .Append(maxY.ToString("F2"));
            switch (ShapeOperation)
            {
                case ShapeOperation.None: break;
                case ShapeOperation.Stroke:
                    sb.Append(" stroke:").Append(StringifyColor(StrokeColor));
                    break;
                case ShapeOperation.Fill:
                    sb.Append(" fill:").Append(StringifyColor(FillColor));
                    break;
                case ShapeOperation.Both:
                    sb.Append(" stroke:").Append(StringifyColor(StrokeColor));
                    sb.Append(" fill:").Append(StringifyColor(FillColor));
                    break;
                default: throw new ArgumentOutOfRangeException();
            }

            return sb.ToString();
        }

        string StringifyColor(Color? c)
        {
            if (c == null)
                return "-";
            var color = c.Value;
            if (color.A == 0)
                return "transparent";
            if (color.R == 0)
            {
                if (color.G == 0)
                {
                    if (color.B == 0) return "black";
                    if (color.B == 255) return "blue";
                }
                else if (color.G == 255)
                {
                    if (color.B == 0) return "green";
                    if (color.B == 255) return "ciel";
                }
            }
            else if (color.R == 255)
            {
                if (color.G == 0)
                {
                    if (color.B == 0) return "red";
                    if (color.B == 255) return "purple";
                }
                else if (color.G == 255)
                {
                    if (color.B == 0) return "yellow";
                    if (color.B == 255) return "white";
                }
            }

            if (color.R == color.G && color.R == color.B)
                return $"gray ({color.R})";
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}{(color.A == 255 ? "" : color.A.ToString("X2"))}";
        }
    }
}