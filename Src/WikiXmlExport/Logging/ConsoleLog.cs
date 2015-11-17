namespace PathfinderFr.WikiXmlExport.Logging
{
    using System;

    public class ConsoleLog : LogBase, ILog
    {
        public ConsoleLog()
        {
        }

        public void Write(string message)
        {
            if (!this.IsVerbose)
                return;

            this.Write(message, ConsoleColor.White);
        }

        public void WriteLine(string message)
        {
            if (!this.IsVerbose)
                return;

            this.Write(message, ConsoleColor.White);
            this.Write(Environment.NewLine, ConsoleColor.White);
        }

        public override void Event(LogLevel level, string format, params object[] args)
        {
            ConsoleColor color = ConsoleColor.White;

            if ((int)level <= (int)LogLevel.Error)
            {
                color = ConsoleColor.Red;
            }
            else if ((int)level <= (int)LogLevel.Warning)
            {
                color = ConsoleColor.Yellow;
            }
            else if ((int)level > (int)LogLevel.Information)
            {
                color = ConsoleColor.Gray;
            }

            this.Write(string.Format(format, args), color);
            Console.WriteLine();
        }

        private void Write(string message, ConsoleColor color = ConsoleColor.White)
        {
            var previous = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ForegroundColor = previous;
        }
    }
}
