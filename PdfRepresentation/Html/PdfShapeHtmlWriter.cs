using System.Text;
using PdfRepresentation.Model.Pdf;

namespace PdfRepresentation.Html
{
    public abstract class PdfShapeHtmlWriter
    {
        public abstract void AddShapes(PageDetails page, StringBuilder sb);

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