using System.Drawing;
using System.Text;

namespace PdfRepresentation.Internals.Helpers
{
    internal class ColorHelper
    {
        public static void AppendColor(Color color, StringBuilder stringBuilder)
        {
            stringBuilder.Append("#").Append(color.R.ToString("X2"))
                .Append(color.G.ToString("X2"))
                .Append(color.B.ToString("X2"));
            if (color.A != 255)
            {
                stringBuilder.Append(color.A.ToString("X2"));
            }
        }
    }
}
