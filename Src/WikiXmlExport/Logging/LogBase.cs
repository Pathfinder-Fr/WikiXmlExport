namespace PathfinderFr.WikiXmlExport.Logging
{
    public abstract class LogBase
    {
        public virtual LogLevel Level { get; set; }

        public bool IsVerbose
        {
            get { return this.HasLevel(LogLevel.Verbose); }
        }

        public bool HasLevel(LogLevel level)
        {
            return this.Level.HasFlag(level);
        }

        public void Error(string format, params object[] args)
        {
            this.Event(LogLevel.Error, format, args);
        }

        public void Error(string message)
        {
            this.Event(LogLevel.Error, message);
        }

        public void Warning(string format, params object[] args)
        {
            if (!this.HasLevel(LogLevel.Warning))
                return;
            
            this.Event(LogLevel.Warning, format, args);
        }

        public void Warning(string message)
        {
            if (!this.HasLevel(LogLevel.Warning))
                return;
            
            this.Event(LogLevel.Warning, message);
        }

        public void Info(string format, params object[] args)
        {
            if (!this.HasLevel(LogLevel.Information))
                return;
            
            this.Event(LogLevel.Information, format, args);
        }

        public void Info(string message)
        {
            if (!this.HasLevel(LogLevel.Information))
                return;
            
            this.Event(LogLevel.Information, message);
        }

        public void Verbose(string format, params object[] args)
        {
            if (!this.HasLevel(LogLevel.Verbose))
                return;
                        
            this.Event(LogLevel.Verbose, format, args);
        }

        public void Verbose(string message)
        {
            if (!this.HasLevel(LogLevel.Verbose))
                return;
            
            this.Event(LogLevel.Verbose, message);
        }

        public abstract void Event(LogLevel level, string format, params object[] args);
    }
}
