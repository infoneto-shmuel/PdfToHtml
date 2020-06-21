using System.Collections.Generic;

namespace PdfRepresantation
{
    public class ShapePoint
    {
        public double X { get; set; }
        public double Y{ get; set; }
        public override string ToString() =>$"{X},{Y}";
    }
    public class ShapeLine
    {
        public ShapePoint Start { get; set; }
        public ShapePoint CurveControlPoint1 { get; set; }
        public ShapePoint CurveControlPoint2 { get; set; }
        public ShapePoint End { get; set; }
        public IEnumerable<ShapePoint> AllPoints
        {
            get
            {
                yield return Start;
                if (CurveControlPoint1 != null) yield return CurveControlPoint1;
                if (CurveControlPoint2 != null) yield return CurveControlPoint2;
                yield return End;
            }
        }

        public override string ToString() =>$"{Start}=>{End}";

    }    
}