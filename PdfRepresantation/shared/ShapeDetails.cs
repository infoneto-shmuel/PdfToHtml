using System.Collections.Generic;
using System.Drawing;

namespace PdfRepresantation
{
    public enum ShapeOperation
    {
        None=0,Stroke=1,Fill=2,Both=3
    }
    public class ShapeDetails
    {
        public ShapeOperation ShapeOperation { get; set; }
        public Color? StrokeColor{ get; set; }
        public Color? FillColor{ get; set; }
        public IList<ShapeLine> Lines=new List<ShapeLine>();
        public float LineWidth { get; set; }
    }
}