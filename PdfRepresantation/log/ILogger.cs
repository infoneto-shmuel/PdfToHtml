namespace PdfRepresantation
{
    public interface ILogger
    {
        bool DebugSupported { get; }
        bool InfoSupported { get; }
        bool ErrorSupported { get; }
        void Debug(string s);
        void Info(string s);
        void Error(string s);
    }
}