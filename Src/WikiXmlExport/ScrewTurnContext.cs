using System;
using ScrewTurn.Wiki;
using ScrewTurn.Wiki.PluginFramework;
using System.Configuration;

namespace WikiXmlExport
{
    public static class ScrewTurnContext
    {
        private static bool initialized = false;

        public static void Initialize(string command, string dir, ILog log)
        {
            if (initialized)
                return;

            log = log ?? Logging.NullLog.Instance;

            initialized = true;

            log.Verbose("Initialization...");

            ConsoleHost host = new ConsoleHost(dir);

            ConsoleSettingsStorageProvider ssp = new ConsoleSettingsStorageProvider();
            ssp.Init(host, string.Empty);
            Collectors.SettingsProvider = ssp;

            Exchanger.ResourceExchanger = new ResourceExchanger();

            MimeTypes.Init();

            // Load Providers
            Collectors.FileNames = new System.Collections.Generic.Dictionary<string, string>(10);
            Collectors.UsersProviderCollector = new ProviderCollector<IUsersStorageProviderV30>();
            Collectors.PagesProviderCollector = new ProviderCollector<IPagesStorageProviderV30>();
            Collectors.FilesProviderCollector = new ProviderCollector<IFilesStorageProviderV30>();
            Collectors.FormatterProviderCollector = new ProviderCollector<IFormatterProviderV30>();
            Collectors.CacheProviderCollector = new ProviderCollector<ICacheProviderV30>();
            Collectors.DisabledUsersProviderCollector = new ProviderCollector<IUsersStorageProviderV30>();
            Collectors.DisabledPagesProviderCollector = new ProviderCollector<IPagesStorageProviderV30>();
            Collectors.DisabledFilesProviderCollector = new ProviderCollector<IFilesStorageProviderV30>();
            Collectors.DisabledFormatterProviderCollector = new ProviderCollector<IFormatterProviderV30>();
            Collectors.DisabledCacheProviderCollector = new ProviderCollector<ICacheProviderV30>();

            IPagesStorageProviderV30 p = new PagesStorageProvider();
            if (!ProviderLoader.IsDisabled(p.GetType().FullName))
            {
                p.Init(host, "");
                Collectors.PagesProviderCollector.AddProvider(p);
                Log.LogEntry("Provider " + p.Information.Name + " loaded (Enabled)", EntryType.General, Log.SystemUsername);
            }
            else
            {
                Collectors.DisabledPagesProviderCollector.AddProvider(p);
                Log.LogEntry("Provider " + p.Information.Name + " loaded (Disabled)", EntryType.General, Log.SystemUsername);
            }

            p = new ScrewTurn.Wiki.Plugins.SqlServer.SqlServerPagesStorageProvider();
            if (!ProviderLoader.IsDisabled(p.GetType().FullName))
            {
                p.Init(host, ConfigurationManager.ConnectionStrings["Default"].ConnectionString);
                Collectors.PagesProviderCollector.AddProvider(p);
                Log.LogEntry("Provider " + p.Information.Name + " loaded (Enabled)", EntryType.General, Log.SystemUsername);
            }
            else
            {
                Collectors.DisabledPagesProviderCollector.AddProvider(p);
                Log.LogEntry("Provider " + p.Information.Name + " loaded (Disabled)", EntryType.General, Log.SystemUsername);
            }

            CacheProvider c = new CacheProvider();
            if (!ProviderLoader.IsDisabled(c.GetType().FullName))
            {
                c.Init(host, "");
                Collectors.CacheProviderCollector.AddProvider(c);
                Log.LogEntry("Provider " + c.Information.Name + " loaded (Enabled)", EntryType.General, Log.SystemUsername);
            }
            else
            {
                Collectors.DisabledCacheProviderCollector.AddProvider(c);
                Log.LogEntry("Provider " + c.Information.Name + " loaded (Disabled)", EntryType.General, Log.SystemUsername);
            }

            log.Verbose("Initialisation OK");

            //CommandBase commandBase;

            //switch (command.ToLowerInvariant())
            //{
            //    case "extract":
            //        commandBase = new ExtractPagesCommand(args[2], args[3])
            //        {
            //            Filter = args.Length > 4 ? args[4] : null
            //        };
            //        break;

            //    case "buildindex":
            //        commandBase = new BuildIndexCommand(args[2], args[3]);
            //        break;

            //    case "addlinkscategory":
            //        commandBase = new AddLinksCategoryCommand(args[2], args[3]);
            //        break;

            //    default:
            //        throw new ArgumentOutOfRangeException(command);
            //}

            //commandBase.Run();   
        }
    }
}
