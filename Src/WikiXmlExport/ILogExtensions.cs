namespace PathfinderFr.WikiXmlExport
{
    public static class ILogExtensions
    {
        public static void Write(this ILog log, string format, params object[] args)
        {
            log.Write(string.Format(format, args));
        }
        public static void WriteLine(this ILog log, string format, params object[] args)
        {
            log.WriteLine(string.Format(format, args));
        }

        public static void WriteLine(this ILog log)
        {
            log.WriteLine(string.Empty);
        }
    }
}
