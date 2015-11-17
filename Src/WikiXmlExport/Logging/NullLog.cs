namespace PathfinderFr.WikiXmlExport.Logging
{
    public class NullLog : LogBase, ILog
    {
        public static readonly ILog Instance = new NullLog();

        NullLog()
        {
        }

        public override void Event(LogLevel level, string format, params object[] args)
        {
        }

        public void Write(string message)
        {
        }

        public void WriteLine(string message)
        {
        }
    }
}
