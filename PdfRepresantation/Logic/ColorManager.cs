using System;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using Color = System.Drawing.Color;

namespace PdfRepresantation.Logic
{
    public class ColorManager
    {
        public static readonly ColorManager Instance = new ColorManager();


        internal Color? GetColor(iText.Kernel.Colors.Color colorPfd, float alpha)
        {
            var value = colorPfd.GetColorValue();
            switch (colorPfd)
            {
                case CalGray calGray:
                case DeviceGray deviceGray:
                    return FromGray(value[0],alpha);
                case CalRgb calRgb:
                case DeviceRgb deviceRgb:
                    return FromRGB(value[0], value[1], value[2], alpha);
                case DeviceCmyk deviceCmyk:
                    return FromCmyk(value[0], value[1], value[2], value[3], alpha);
                case IccBased iccBased:
                    var colorSpace = colorPfd.GetColorSpace();
                    var source = ((PdfArray) colorSpace.GetPdfObject()).GetAsStream(1);
                    var alt = source.GetAsName(PdfName.Alternate);
                    if (Equals(alt, PdfName.CalGray) || Equals(alt, PdfName.DeviceGray))
                        return FromGray(value[0],alpha);
                    if (Equals(alt, PdfName.CalRGB) || Equals(alt, PdfName.DeviceRGB))
                        return FromRGB(value[0], value[1], value[2], alpha);
                    if (Equals(alt, PdfName.DeviceCMYK))
                        return FromCmyk(value[0], value[1], value[2], value[3], alpha);
                    return FromUnkown(colorPfd, alpha);
                case PatternColor patternColor:
                    var pattern = patternColor.GetPattern();
                    return PatternColorManager.GetColor(pattern, alpha);
                case DeviceN deviceN:
                case Indexed indexed:
                case Lab lab:
                case Separation separation:
                default:
                    return FromUnkown(colorPfd, alpha);
            }
        }

        private Color? FromUnkown(iText.Kernel.Colors.Color colorPfd, float alpha)
        {
            var value = colorPfd.GetColorValue();
            switch (colorPfd.GetNumberOfComponents())
            {
                case 0:
                    return null;
                case 1:
                    return FromGray(value[0], alpha);
                case 3:
                    return FromRGB(value[0], value[1], value[2], alpha);
                case 4:
                    return FromCmyk(value[0], value[1], value[2], value[3], alpha);
                default: return null;
            }
        }

        private Color FromRGB(float red, float green, float blue, float alpha)
        {
            return Color.FromArgb((int) (alpha * 255),
                (int) (red * 255),
                (int) (green * 255),
                (int) (blue * 255));
        }

        private static Color? FromGray(float gray,float alpha)
        {
            var g = (int) (gray * 255);
            return Color.FromArgb((int) (alpha * 255), g, g, g);
        }

        private static Color FromCmyk(float c, float m, float y, float k, float alpha)
        {
            var r = (int) (255 * (1 - c) * (1 - k));
            var g = (int) (255 * (1 - m) * (1 - k));
            var b = (int) (255 * (1 - y) * (1 - k));
            return Color.FromArgb((int) (alpha * 255), r, g, b);
        }

        public Color? GetColor(TextRenderInfo text)
        {
            iText.Kernel.Colors.Color color;

            float alpha;
            switch (text.GetTextRenderMode())
            {
                case 0: // Fill text
                case 4: // Fill text and add to path for clipping
                    color = text.GetFillColor();
                    alpha = text.GetGraphicsState().GetFillOpacity();
                    break;
                case 1: //  Stroke text
                case 2: //Fill, then stroke text
                case 5: // Stroke text and add to path for clipping
                case 6: // Fill, then stroke text and add to path for clipping
                    color = text.GetStrokeColor();
                    alpha = text.GetGraphicsState().GetStrokeOpacity();
                    break;
                case 3: // Invisible
                    return System.Drawing.Color.Transparent;
                case 7: // Add text to padd for clipping
                    return System.Drawing.Color.Black;
                default: throw new ApplicationException("wrong render mode");
            }


//           var value = text.GetStrokeColor().GetColorValue();
//           if(value.Length==1&&value[0]==0)
//               value = text.GetFillColor().GetColorValue();
            return GetColor(color, alpha);
        }
    }
}