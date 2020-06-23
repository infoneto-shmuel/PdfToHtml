namespace PdfRepresantation
{
    public static class Log
    {
        public static ILogger logger;
        public static bool DebugSupported => logger != null && logger.DebugSupported;
        public static bool InfoSupported => logger != null && logger.InfoSupported;
        public static bool ErrorSupported => logger != null && logger.ErrorSupported;

        public static void Debug(string s)
        {
            if (DebugSupported) logger.Debug(s);
        }

        public static void Info(string s)
        {
            if (InfoSupported) logger.Info(s);
        }

        public static void Error(string s)
        {
            if (ErrorSupported) logger.Error(s);
        }
    }
}