using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScrewTurn.Wiki.PluginFramework;
using ScrewTurn.Wiki;
using System.IO;
using AclEngine = ScrewTurn.Wiki.AclEngine;
using System.Diagnostics;

namespace WikiXmlExport
{
    public class ConsoleSettingsStorageProvider : ISettingsStorageProviderV30
    {
        private const string AclFile = "ACL.cs";

        private IHostV30 host;

        private AclEngine.IAclManager aclManager;

        private AclStorer aclStorer;

        public string GetSetting(string name)
        {
            switch (name)
            {
                case "EnableSectionEditing":
                    return "false";

                case "EnableSectionAnchors":
                    return "false";

                case "ProcessSingleLineBreaks":
                case "DefaultPagesProvider":
                    return "ScrewTurn.Wiki.Plugins.SqlServer.SqlServerPagesStorageProvider";

                case "DefaultCacheProvider":
                case "ChangeModerationMode":
                    return null;

                default:
                    return null;
            }
        }

        public bool SetSetting(string name, string value)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, string> GetAllSettings()
        {
            throw new NotImplementedException();
        }

        public void BeginBulkUpdate()
        {
            throw new NotImplementedException();
        }

        public void EndBulkUpdate()
        {
            throw new NotImplementedException();
        }

        public void LogEntry(string message, EntryType entryType, string user)
        {
            //Console.WriteLine("{0} {1}", entryType, message);
            //Debug.WriteLine("{0} {1}", entryType, message);            
        }

        public LogEntry[] GetLogEntries()
        {
            throw new NotImplementedException();
        }

        public void ClearLog()
        {
            throw new NotImplementedException();
        }

        public int LogSize
        {
            get { throw new NotImplementedException(); }
        }

        public string GetMetaDataItem(MetaDataItem item, string tag)
        {
            throw new NotImplementedException();
        }

        public bool SetMetaDataItem(MetaDataItem item, string tag, string content)
        {
            throw new NotImplementedException();
        }

        public RecentChange[] GetRecentChanges()
        {
            return new RecentChange[0];
        }

        public bool AddRecentChange(string page, string title, string messageSubject, DateTime dateTime, string user, Change change, string descr)
        {
            throw new NotImplementedException();
        }

        public string[] ListPluginAssemblies()
        {
            throw new NotImplementedException();
        }

        public bool StorePluginAssembly(string filename, byte[] assembly)
        {
            throw new NotImplementedException();
        }

        public byte[] RetrievePluginAssembly(string filename)
        {
            throw new NotImplementedException();
        }

        public bool DeletePluginAssembly(string filename)
        {
            throw new NotImplementedException();
        }

        public bool SetPluginStatus(string typeName, bool enabled)
        {
            throw new NotImplementedException();
        }

        public bool GetPluginStatus(string typeName)
        {
            return true;
        }

        public bool SetPluginConfiguration(string typeName, string config)
        {
            throw new NotImplementedException();
        }

        public string GetPluginConfiguration(string typeName)
        {
            throw new NotImplementedException();
        }

        public ScrewTurn.Wiki.AclEngine.IAclManager AclManager
        {
            get { return this.aclManager; }
        }

        public bool StoreOutgoingLinks(string page, string[] outgoingLinks)
        {
            throw new NotImplementedException();
        }

        public string[] GetOutgoingLinks(string page)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, string[]> GetAllOutgoingLinks()
        {
            throw new NotImplementedException();
        }

        public bool DeleteOutgoingLinks(string page)
        {
            throw new NotImplementedException();
        }

        public bool UpdateOutgoingLinksForRename(string oldName, string newName)
        {
            throw new NotImplementedException();
        }

        public bool IsFirstApplicationStart()
        {
            throw new NotImplementedException();
        }

        public void Init(IHostV30 host, string config)
        {
            this.host = host;

            // Initialize ACL Manager and Storer
            this.aclManager = new StandardAclManager();
            aclStorer = new AclStorer(aclManager, GetFullPath(AclFile));
            aclStorer.LoadData();
        }

        public void Shutdown()
        {
            throw new NotImplementedException();
        }

        public ComponentInformation Information
        {
            get { throw new NotImplementedException(); }
        }

        public string ConfigHelpHtml
        {
            get { throw new NotImplementedException(); }
        }

        private string GetFullPath(string name)
        {
            return Path.Combine(host.GetSettingValue(SettingName.PublicDirectory), name);
        }
    }
}
