using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScrewTurn.Wiki.PluginFramework;
using ScrewTurn.Wiki;

namespace PathfinderFr.WikiXmlExport
{
    public class ConsoleHost : IHostV30
    {
        string workingDirectory;

        public ConsoleHost(string workingDirectory)
        {
            this.workingDirectory = workingDirectory;
        }

        public string GetSettingValue(SettingName name)
        {
            switch (name)
            {
                case SettingName.PublicDirectory:
                    return this.workingDirectory;

                case SettingName.CacheSize:
                    return "1000";

                default:
                    throw new NotImplementedException();
            }
        }

        public UserInfo[] GetUsers()
        {
            throw new NotImplementedException();
        }

        public UserInfo FindUser(string username)
        {
            throw new NotImplementedException();
        }

        public UserInfo GetCurrentUser()
        {
            throw new NotImplementedException();
        }

        public UserGroup[] GetUserGroups()
        {
            throw new NotImplementedException();
        }

        public UserGroup FindUserGroup(string name)
        {
            throw new NotImplementedException();
        }

        public bool CheckActionForGlobals(string action, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public bool CheckActionForNamespace(NamespaceInfo nspace, string action, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public bool CheckActionForPage(PageInfo page, string action, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public bool CheckActionForDirectory(StDirectoryInfo directory, string action, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public string GetTheme(NamespaceInfo nspace)
        {
            throw new NotImplementedException();
        }

        public NamespaceInfo[] GetNamespaces()
        {
            throw new NotImplementedException();
        }

        public NamespaceInfo FindNamespace(string name)
        {
            throw new NotImplementedException();
        }

        public PageInfo[] GetPages(NamespaceInfo nspace)
        {
            throw new NotImplementedException();
        }

        public CategoryInfo[] GetCategories(NamespaceInfo nspace)
        {
            throw new NotImplementedException();
        }

        public Snippet[] GetSnippets()
        {
            return Snippets.GetSnippets().ToArray();
        }

        public NavigationPath[] GetNavigationPaths(NamespaceInfo nspace)
        {
            throw new NotImplementedException();
        }

        public CategoryInfo[] GetCategoriesPerPage(PageInfo page)
        {
            throw new NotImplementedException();
        }

        public PageInfo FindPage(string fullName)
        {
            throw new NotImplementedException();
        }

        public PageContent GetPageContent(PageInfo page)
        {
            return Content.GetPageContent(page, true);
        }

        public int[] GetBackups(PageInfo page)
        {
            throw new NotImplementedException();
        }

        public PageContent GetBackupContent(PageInfo page, int revision)
        {
            throw new NotImplementedException();
        }

        public string GetFormattedContent(PageInfo page)
        {
            throw new NotImplementedException();
        }

        public string Format(string raw)
        {
            var formatter = new Formatter();
            return formatter.Format(raw, false, FormattingContext.Unknown, null);
        }

        public string PrepareContentForIndexing(PageInfo page, string content)
        {
            return content;
        }

        public string PrepareTitleForIndexing(PageInfo page, string title)
        {
            return title;
        }

        public ScrewTurn.Wiki.SearchEngine.SearchResultCollection PerformSearch(string query, bool fullText, bool filesAndAttachments, ScrewTurn.Wiki.SearchEngine.SearchOptions options)
        {
            throw new NotImplementedException();
        }

        public StDirectoryInfo[] ListDirectories(StDirectoryInfo directory)
        {
            throw new NotImplementedException();
        }

        public StFileInfo[] ListFiles(StDirectoryInfo directory)
        {
            throw new NotImplementedException();
        }

        public StFileInfo[] ListPageAttachments(PageInfo page)
        {
            throw new NotImplementedException();
        }

        public bool SendEmail(string recipient, string sender, string subject, string body, bool html)
        {
            return true;
        }

        public void LogEntry(string message, LogEntryType entryType, string user, object caller)
        {
            Console.WriteLine("{0} {1}", entryType, message);
        }

        public void ChangeCurrentUserLanguage(string language)
        {
            throw new NotImplementedException();
        }

        public DateTime AlignDateTimeWithPreferences(DateTime dt)
        {
            throw new NotImplementedException();
        }

        public void ClearCache(CacheData data)
        {
            throw new NotImplementedException();
        }

        public void AddToolbarItem(ToolbarItem item, string text, string value)
        {
            throw new NotImplementedException();
        }

        public string GetDefaultProvider(Type providerType)
        {
            throw new NotImplementedException();
        }

        public IPagesStorageProviderV30[] GetPagesStorageProviders(bool enabled)
        {
            throw new NotImplementedException();
        }

        public IUsersStorageProviderV30[] GetUsersStorageProviders(bool enabled)
        {
            throw new NotImplementedException();
        }

        public IFilesStorageProviderV30[] GetFilesStorageProviders(bool enabled)
        {
            throw new NotImplementedException();
        }

        public ICacheProviderV30[] GetCacheProviders(bool enabled)
        {
            throw new NotImplementedException();
        }

        public IFormatterProviderV30[] GetFormatterProviders(bool enabled)
        {
            throw new NotImplementedException();
        }

        public ISettingsStorageProviderV30 GetSettingsStorageProvider()
        {
            throw new NotImplementedException();
        }

        public string GetProviderConfiguration(string providerTypeName)
        {
            throw new NotImplementedException();
        }

        public bool SetProviderConfiguration(IProviderV30 provider, string configuration)
        {
            throw new NotImplementedException();
        }

        public bool UpgradePageStatusToAcl(PageInfo page, char oldStatus)
        {
            throw new NotImplementedException();
        }

        public bool UpgradeSecurityFlagsToGroupsAcl(UserGroup administrators, UserGroup users)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<UserAccountActivityEventArgs> UserAccountActivity;

        public event EventHandler<UserGroupActivityEventArgs> UserGroupActivity;

        public event EventHandler<NamespaceActivityEventArgs> NamespaceActivity;

        public event EventHandler<PageActivityEventArgs> PageActivity;

        public event EventHandler<FileActivityEventArgs> FileActivity;

        public Dictionary<string, CustomToolbarItem> CustomSpecialTags
        {
            get { throw new NotImplementedException(); }
        }

        public void OnAttachmentActivity(string provider, string attachment, string page, string oldAttachmentName, FileActivity activity)
        {
            throw new NotImplementedException();
        }

        public void OnDirectoryActivity(string provider, string directory, string oldDirectoryName, FileActivity activity)
        {
            throw new NotImplementedException();
        }

        public void OnFileActivity(string provider, string file, string oldFileName, FileActivity activity)
        {
            throw new NotImplementedException();
        }

        public void OnPageActivity(PageInfo page, string pageOldName, string author, PageActivity activity)
        {
            throw new NotImplementedException();
        }

        public void OnNamespaceActivity(NamespaceInfo nspace, string nspaceOldName, NamespaceActivity activity)
        {
            throw new NotImplementedException();
        }

        public void OnUserAccountActivity(UserInfo user, UserAccountActivity activity)
        {
            throw new NotImplementedException();
        }

        public void OnUserGroupActivity(UserGroup group, UserGroupActivity activity)
        {
            throw new NotImplementedException();
        }


        public void OverridePublicDirectory(string fullPath)
        {
            throw new NotImplementedException();
        }
    }
}
