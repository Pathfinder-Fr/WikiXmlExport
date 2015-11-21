namespace WikiXmlExport.Console
{
    using System.Configuration;
    using System.IO;
    using System.Web;
    using WikiXmlExport.Commands;

    class Program
    {
        public static string BaseDir;

        static void Main(string[] args)
        {
            var consoleLog = new Logging.ConsoleLog();
            consoleLog.Level = Logging.LogLevel.All;

            HttpContext.Current = new HttpContext(new HttpRequest("Index.html", "http://localhost", string.Empty), new HttpResponse(new StringWriter()));

            BaseDir = args.ArgNamed("basedir") ?? ConfigurationManager.AppSettings["BaseDir"] ?? ".";

            var inDir = Path.GetFullPath(Path.Combine(BaseDir, @"In"));

            if (!Directory.Exists(inDir))
                Directory.CreateDirectory(inDir);
            
            ScrewTurnContext.Initialize(string.Empty, inDir, consoleLog);

            
            //ScrewTurnContext.Initialize(string.Empty, BaseDir, consoleLog);

            var action = args[0].ToLowerInvariant();

            switch (action)
            {
                case "export":
                    Export(consoleLog, args.ArgNamed("ptrn"), args.ArgNamed("out"), args.ArgNamed("tmpl"), args.ArgNamed("filter"), args.ArgNamed("ext", "html"));
                    break;

                case "applysnippets":
                    ApplySnippets();
                    break;

                case "restoresnippets":
                    RestoreSnippets();
                    break;
            }
        }

        private static void ApplySnippets()
        {
            var cmd = new SnippetsCommand(Path.Combine(Program.BaseDir, @"Snippets\Backup"), Path.Combine(Program.BaseDir, @"Snippets\Custom"));
            cmd.BackupAndDelete();
        }

        private static void RestoreSnippets()
        {
            var cmd = new SnippetsCommand(Path.Combine(Program.BaseDir, @"Snippets\Custom"), Path.Combine(Program.BaseDir, @"Snippets\Backup"));
            cmd.BackupAndDelete();
        }

        static void Export(ILog consoleLog, string pattern, string @out, string template, string filter, string extension)
        {
            using (var writer = new StreamWriter(@"Export.log", false))
            {
                var logFile = new Logging.StreamWriterLog(writer);
                logFile.Level = Logging.LogLevel.Verbose;

                var logger = new Logging.CombineLog(logFile, consoleLog) { Level = Logging.LogLevel.All };

                var cmd = new ExtractPagesCommand(pattern, Path.Combine(Program.BaseDir, @out), logger);
                cmd.Filter = filter;
                cmd.Extension = extension;
                cmd.Template = Path.Combine(BaseDir, template);
                cmd.Run();
            }
        }
    }
}