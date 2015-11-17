using System.IO;
using ScrewTurn.Wiki;
using ScrewTurn.Wiki.Plugins.SqlServer;

namespace PathfinderFr.WikiXmlExport.Commands
{
    public class SnippetsCommand
    {
        private readonly string backup;

        private readonly string custom;

        public SnippetsCommand(string backup, string custom)
        {
            this.backup = backup;
            this.custom = custom;

            if (!Directory.Exists(backup))
                Directory.CreateDirectory(backup);

            if (!Directory.Exists(custom))
                Directory.CreateDirectory(custom);
            
        }

        public void BackupAndDelete()
        {
            var snippets = Snippets.GetSnippets().ToArray();

            var fileProvider = Collectors.PagesProviderCollector.GetProvider(typeof(PagesStorageProvider).FullName);
            
            foreach (var snippet in snippets)
            {
                // Ecriture sauvegarde
                var path = Path.Combine(this.backup, string.Format("{0}.cs", snippet.Name));
                File.WriteAllText(path, snippet.Content);

                // Suppression snippet
                Snippets.RemoveSnippet(snippet);

                // Ajout snippet personnalisé
                var customPath = Path.Combine(this.custom, string.Format("{0}.cs", snippet.Name));
                if (!File.Exists(customPath))
                {
                    // Snippet personnalisé inexistant
                    File.WriteAllText(customPath, string.Empty);

                    // Ajout snippet vide en base
                    Snippets.AddSnippet(snippet.Name, string.Empty, fileProvider);
                }
                else
                {
                    var newContent = File.ReadAllText(customPath);

                    // Ajout snippet personnalisé en base
                    Snippets.AddSnippet(snippet.Name, newContent, fileProvider);
                }
            }
        }

        public void Restore()
        {
            var snippets = Snippets.GetSnippets().ToArray();

            var fileProvider = Collectors.PagesProviderCollector.GetProvider(typeof(PagesStorageProvider).FullName);

            // Suppression des snippets actuels
            foreach (var snippet in snippets)
            {
                Snippets.RemoveSnippet(snippet);
            }

            // Restauration snippets sauvegardés
            foreach (var file in Directory.GetFiles(this.backup, "*.cs"))
            {
                var name = Path.GetFileNameWithoutExtension(file);
                var content = File.ReadAllText(file);

                Snippets.AddSnippet(name, content, fileProvider);
            }
        }
    }
}