namespace WikiXmlExport.Logging
{
    using System;
    using System.Collections.Generic;

    public class CombineLog : LogBase, ILog
    {
        private readonly IEnumerable<ILog> logs;

        public CombineLog(IEnumerable<ILog> logs)
        {
            this.logs = logs;
        }

        public CombineLog(params ILog[] logs)
        {
            this.logs = logs;
        }

        public void Write(string message)
        {
            ForEach(l => l.Write(message));
        }

        public void WriteLine(string message)
        {
            ForEach(l => l.WriteLine(message));
        }

        public override void Event(LogLevel level, string format, params object[] args)
        {
            ForEach(l => l.Event(level, format, args));
        }

        private void ForEach(Action<ILog> action)
        {
            foreach (var log in this.logs)
            {
                action(log);
            }
        }
    }
}
