using System;

namespace PdfRepresantation
{
    public class ConsoleLogger : ILogger
    {
        public bool DebugSupported { get; set; }
        public bool InfoSupported { get; set; }
        public bool ErrorSupported { get; set; }
        public void Debug(string s)
        {
            Console.WriteLine(s);
        }

        public void Info(string s)
        {
            Console.WriteLine(s);
        }

        public void Error(string s)
        {
            Console.Error.WriteLine(s);
        }
    }
}