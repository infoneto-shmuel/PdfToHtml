using log4net;

namespace PdfRepresantation.Server
{
    public class Log4NetLogger : ILogger
    {
        private readonly ILog log4NetLogger;

        public Log4NetLogger(ILog log4NetLogger)
        {
            this.log4NetLogger = log4NetLogger;
        }

        public bool DebugSupported => log4NetLogger.IsDebugEnabled;
        public bool InfoSupported => log4NetLogger.IsInfoEnabled;
        public bool ErrorSupported => log4NetLogger.IsErrorEnabled;

        public void Debug(string s)
        {
            log4NetLogger.Debug(s);
        }

        public void Info(string s)
        {
            log4NetLogger.Info(s);
        }

        public void Error(string s)
        {
            log4NetLogger.Error(s);
        }
    }
}