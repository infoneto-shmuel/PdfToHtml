using System;

namespace PdfRepresantation.Model.Pdf
{
    public class ShapePoint
    {
        public double X { get; set; }
        public double Y { get; set; }
        public bool Equals(ShapePoint other) =>
           other != null && Math.Abs(X - other.X) < 0.01 && Math.Abs(Y - other.Y) < 0.01;
        public override bool Equals(object obj) => Equals(obj as ShapePoint);

        public override string ToString() => $"{X:F2},{Y:F2}";
    }
}