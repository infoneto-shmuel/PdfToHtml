using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser.Data;

namespace PdfRepresantation
{
    public class FontManager
    {
        readonly Regex fontFamilyRegex =
            new Regex(@"^.+\+|(PS|PSMT|MT|MS)$|((PS|PSMT|MT|MS)?[,-])?(Bold|Italic|MT|PS)+$",
                RegexOptions.ExplicitCapture);

        public PdfFontDetails CreateFont(PdfFont pdfFont)
        {
            var font = new PdfFontDetails();
            var fontName
                = pdfFont.GetFontProgram().GetFontNames().GetFontName();
            font.FontFamily = fontName;
            var basicFontFamily = fontFamilyRegex.Replace(fontName, "");
            basicFontFamily = new string(SpaceInCamelCase(basicFontFamily).ToArray());
            font.BasicFontFamily = basicFontFamily;
            font.Bold = pdfFont.GetFontProgram().GetFontNames().GetFontWeight() >= FontWeights.BOLD ||
                        fontName.Contains("bold") || fontName.Contains("Bold");
            font.Italic = fontName.Contains("italic") || fontName.Contains("Italic");
            return font;
        }

        private IEnumerable<char> SpaceInCamelCase(string s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                var c = s[i];
                if (i > 1 && char.IsUpper(c) && char.IsLower(s[i - 1]))
                    yield return ' ';
                yield return c;
            }
        }
        public float GetFontSize(TextRenderInfo textRenderInfo, LineSegment baseline, LineSegment ascentLine)
        {
            var fontSize = textRenderInfo.GetFontSize();
            var height = ascentLine.GetStartPoint().Get(1) - baseline.GetStartPoint().Get(1);
            if (fontSize > 0.99 && fontSize < 1.01)
            {
                return height * 1.05F;
            }
            var ctm = textRenderInfo.GetGraphicsState().GetCtm();
            var heightFont = ctm.Get(Matrix.I22);
            if (heightFont >0.99&&heightFont <1.01)
            {
                if (fontSize > height * 1.3)
                {
                     return height * 1.3F;
                }               
            }
            else
            {
                fontSize *=heightFont>0?heightFont: -heightFont;
             }

            return fontSize;
        }
    }
}