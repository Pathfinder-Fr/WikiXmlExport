namespace PathfinderFr.WikiXmlExport
{
    public interface ILog
    {
        Logging.LogLevel Level { get; set; }

        bool IsVerbose { get; }

        void Error(string format, params object[] args);

        void Error(string message);

        void Warning(string format, params object[] args);

        void Warning(string message);

        void Info(string format, params object[] args);

        void Info(string message);

        void Verbose(string format, params object[] args);

        void Verbose(string message);

        void Write(string message);

        void WriteLine(string message);

        void Event(Logging.LogLevel level, string format, params object[] args);

    }
}
