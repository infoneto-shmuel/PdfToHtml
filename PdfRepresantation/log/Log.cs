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
            if (logger?.DebugSupported == true)
                logger.Debug(s);
        }

        public static void Info(string s)
        {
            if (logger?.InfoSupported == true)
                logger.Info(s);
        }

        public static void Error(string s)
        {
            if (logger?.ErrorSupported == true)
                logger.Error(s);
        }
    }
}