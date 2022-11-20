using iText.IO.Font.Constants;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using PdfRepresentation.Logging;
using FontDetails = PdfRepresentation.Model.Pdf.FontDetails;

namespace PdfRepresentation.Logic
{
    public class FontManager
    {
        readonly Regex fontFamilyRegex =
            new Regex(@"^.+\+|(PS|PSMT|MT|MS)$|((PS|PSMT|MT|MS)?[,-])?(Bold|Italic|MT|PS)+$",
                RegexOptions.ExplicitCapture);

        public static readonly FontManager Instance = new FontManager();

        private FontManager()
        {
        }

        public FontDetails CreateFont(PdfFont pdfFont)
        {
            var font = new FontDetails();
            var fontName
                = pdfFont.GetFontProgram().GetFontNames().GetFontName() ?? string.Empty;
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
                LogWrongFontSize("no font size. take height of line:" + height);
               
                return height * 1.05F;
            }

            var ctm = textRenderInfo.GetGraphicsState().GetCtm();
            var heightFont = ctm.Get(Matrix.I22);
            if (heightFont > 0.99 && heightFont < 1.01)
            {
                if (fontSize > height * 1.3)
                {
                    LogWrongFontSize("big fontSize:" + fontSize + ". take height of line:" + height);
                    return height * 1.3F;
                }
            }
            else
            {
                if (heightFont > 0)
                {
                    fontSize *= heightFont;
                    LogWrongFontSize("height font positive: " + heightFont);
                }
                else
                    fontSize *= -heightFont;
            }

            return fontSize;
        }

        private string lastLog;
        private void LogWrongFontSize(string m)
        {
            if (!Log.DebugSupported||lastLog == m)
                return;
            lastLog = m;
            Log.Debug(m);
        }
    }
}