using System.Linq;
using iText.Kernel.Colors;

namespace PdfRepresantation
{
   public class ColorManager
   {
       public static readonly ColorManager Instance=new ColorManager();
        internal System.Drawing.Color? GetColor(Color colorPfd)
        {
            var value = colorPfd.GetColorValue();
            if (value.Length == 0 || value.All(v => v == 0))
                return null;
            switch (value.Length)
            {
                case 0:
                    return null;
                case 1:
                    return System.Drawing.Color.FromArgb((int) (value[0] * 255),
                        (int) (value[0] * 255),
                        (int) (value[0] * 255));
                  case 3:  
                    return System.Drawing.Color.FromArgb((int) (value[0] * 255),
                        (int) (value[1] * 255),
                        (int) (value[2] * 255));
                case 4:  
                    return System.Drawing.Color.FromArgb((int) (value[0] * 255),
                        (int) (value[1] * 255),
                        (int) (value[2] * 255),
                        (int) (value[3] * 255));
                default: return null;
            }
           
;
        }

    }
}