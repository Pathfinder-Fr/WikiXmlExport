namespace PathfinderFr.WikiXmlExport.Logging
{
    using System;

    [Flags]
    public enum LogLevel
    {
        None = 0,

        Critical = 1,

        Error = 2 | Critical,

        Warning = 4 | Error,

        Information = 8 | Warning,

        Verbose = 16 | Information,

        All = Int32.MaxValue
    }
}
