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
        public ShapePoint End { get; set; }
        public override string ToString() =>$"{Start}=>{End}";

    }
}