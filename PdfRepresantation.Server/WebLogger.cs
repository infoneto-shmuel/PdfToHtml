using Microsoft.Extensions.Logging;
using ILogger = PdfRepresantation.Interfaces.Log.ILogger;

namespace PdfRepresantation.Server
{
    public class WebLogger : ILogger
    {
        private readonly Microsoft.Extensions.Logging.ILogger aspLogger;

        public WebLogger(Microsoft.Extensions.Logging.ILogger aspLogger)
        {
            this.aspLogger = aspLogger;
        }

        public bool DebugSupported => aspLogger.IsEnabled(LogLevel.Debug);
        public bool InfoSupported => aspLogger.IsEnabled(LogLevel.Information);
        public bool ErrorSupported =>  aspLogger.IsEnabled(LogLevel.Error);
        public void Debug(string s)
        {
            aspLogger.LogDebug(s);
        }

        public void Info(string s)
        {
            aspLogger.LogInformation(s);
        }

        public void Error(string s)
        {
            aspLogger.LogError(s);
        }
    }
}