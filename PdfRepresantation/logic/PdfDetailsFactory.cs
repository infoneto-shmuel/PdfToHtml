using System;
using System.IO;
using iText.Kernel.Pdf;

namespace PdfRepresantation
{
    public class PdfDetailsFactory
    {
        public static PdfDetails Create(PdfDocument source)
        {
            return new PdfParser().Parse(source);
        }

        public static PdfDetails Create(string path)
        {
            try
            {
                return Create(new PdfReader(path));
            }
            catch (Exception e)
            {
                Log.Error($"Error in parsing file '{path}'\r\n\r\n{e}");
                throw;
            }
        }

        public static PdfDetails Create(Stream stream)
        {
            return Create(new PdfReader(stream));
        }

        public static PdfDetails Create(byte[] buffer)
        {
            return Create(new MemoryStream(buffer));
        }

        private static PdfDetails Create(PdfReader reader)
        {
            using (reader)
            {
                var doc = new PdfDocument(reader);
                return new PdfParser().Parse(doc);
            }
        }
    }
}