namespace PathfinderFr.WikiXmlExport.Logging
{
    using System;
    using System.IO;

    public class StreamWriterLog : LogBase, ILog
    {
        private readonly StreamWriter writer;

        public StreamWriterLog(StreamWriter writer)
        {
            this.writer = writer;
        }

        public override void Event(LogLevel level, string format, params object[] args)
        {
            string errorLevel = "    ";

            switch (level)
            {
                case LogLevel.Verbose:
                    errorLevel = "VERB";
                    break;

                case LogLevel.Information:
                    errorLevel = "INFO";
                    break;

                case LogLevel.Warning:
                    errorLevel = "WARN";
                    break;

                case LogLevel.Error:
                    errorLevel = "ERR ";
                    break;

                case LogLevel.Critical:
                    errorLevel = "CRIT";
                    break;
            }

            lock (this)
            {
                this.writer.WriteLine("[{0:yyyy-MM-dd HH:mm:ss}] {1} {2}", DateTime.Now, errorLevel, string.Format(format, args));
            }
        }

        public void Write(string message)
        {
            if (this.IsVerbose)
            {
                lock (this)
                {
                    this.writer.Write(message);
                }
            }
        }

        public void WriteLine(string message)
        {
            if (this.IsVerbose)
            {
                lock (this)
                {
                    this.writer.WriteLine(message);
                }
            }
        }

        private static string Left(string value, int limit)
        {
            if (value == null)
                return value;

            if (value.Length <= limit)
            {
                return value;
            }

            return value.Substring(0, limit);
        }
    }
}
