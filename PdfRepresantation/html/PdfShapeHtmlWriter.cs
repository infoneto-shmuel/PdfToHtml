using System.Drawing;
using System.Text;

namespace PdfRepresantation
{
    public abstract class PdfShapeHtmlWriter
    {
        public abstract void AddShapes(PdfPageDetails page, StringBuilder sb);

        public abstract void AddScript(StringBuilder sb);


        public virtual void AddStyle(StringBuilder sb)
        {
            sb.Append(@"        
        .canvas{
            margin: 0 auto 0 auto;
            display: block;
        }");
       }
    }
}