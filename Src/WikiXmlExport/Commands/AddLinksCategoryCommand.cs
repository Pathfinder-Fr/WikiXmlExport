namespace WikiXmlExport.Commands
{
    using System.Linq;
    using ScrewTurn.Wiki;
    using ScrewTurn.Wiki.PluginFramework;

    public class AddLinksCategoryCommand : CommandBase
    {
        private string category;

        private bool remove;

        public AddLinksCategoryCommand(string fullName, string category, ILog log)
            : base(fullName, log)
        {
            this.category = category;

            if (this.category[0] == '+')
            {
                this.category = this.category.Substring(1);
            }
            else if (this.category[0] == '-')
            {
                this.category = this.category.Substring(1);
                this.remove = true;
            }
        }

        protected override void RunCore()
        {
            this.Log.Info("{2} catégorie {0} aux pages liées par la page {1}", this.category, this.FullName, this.remove ? "Suppression" : "Ajout");

            var formatter = new Formatter();
            foreach (var page in this.Pages)
            {
                this.Log.Info("Chargement page {0}", page.FullName);

                var content = page.GetContent();

                var formatted = page.FormatWithPhase1And2(formatter);

                this.Log.Info("Examen {0} liens", page.Links.Length);

                foreach (var link in page.Links)
                {
                    var linkedPage = this.FindPage(link);

                    if (linkedPage == null)
                        continue;

                    var categories = linkedPage.Categories;

                    if (!this.remove)
                    {
                        if (!categories.Any(p => p.FullName == this.category))
                        {
                            this.Log.Info("Ajout catégorie {0} à la page {1}", this.category, linkedPage.FullName);

                            var newList = categories.Select(c => c.FullName).Union(new[] { this.category }).ToArray();

                            linkedPage.ProviderRebindPage(newList);
                        }
                    }
                    else
                    {
                        if (categories.Any(p => p.FullName == this.category))
                        {
                            this.Log.Info("Suppression catégorie {0} à la page {1}", this.category, linkedPage.FullName);

                            var newList = categories.Where(c => c.FullName != this.category).Select(c => c.FullName).ToArray();

                            linkedPage.ProviderRebindPage(newList);
                        }

                    }
                }
            }
        }
    }
}
