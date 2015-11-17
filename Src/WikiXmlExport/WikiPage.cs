namespace PathfinderFr.WikiXmlExport
{
    using System;
    using ScrewTurn.Wiki;
    using ScrewTurn.Wiki.PluginFramework;

    public class WikiPage
    {
        private readonly PageInfo page;

        private CategoryInfo[] categories;

        private PageContent content;

        private string formatPhase1Cache;

        private string formatPhase3Cache;

        private string[] links;

        public WikiPage(PageInfo page)
        {
            this.page = page;
        }

        public string FullName
        {
            get { return this.page.FullName; }
        }

        public string[] Links
        {
            get { return this.links; }
        }

        public void Load(Formatter formatter)
        {
            this.FormatWithPhase3(formatter);
        }

        public string FormatWithPhase1And2(Formatter formatter)
        {
            var content = this.GetContent();

            if (content == null)
            {
                this.formatPhase1Cache = string.Empty;
                this.formatPhase3Cache = string.Empty;
                this.links = new string[0];
            }

            return this.formatPhase1Cache ?? (this.formatPhase1Cache = FormattingPipeline.FormatWithPhase1And2(formatter, content.Content, false, FormattingContext.PageContent, this.page, out this.links));
        }

        public string FormatWithPhase3(Formatter formatter)
        {
            return this.formatPhase3Cache ?? (this.formatPhase3Cache = FormattingPipeline.FormatWithPhase3(formatter, this.FormatWithPhase1And2(formatter), FormattingContext.PageContent, this.page));
        }

        public PageInfo ProviderGetPage(string fullName)
        {
            return this.page.Provider.GetPage(fullName);
        }

        public void ProviderRebindPage(string[] categories)
        {
            this.page.Provider.RebindPage(this.page, categories);
        }

        public PageContent GetContent()
        {
            return this.content ?? (this.content = this.page.Provider.GetContent(this.page));
        }

        public CategoryInfo[] Categories
        {
            get
            {
                if (this.categories == null)
                {
                    this.categories = this.page.Provider.GetCategoriesForPage(this.page);
                }

                return this.categories;
            }
        }

        public int[] ProviderGetBackups()
        {
            return this.page.Provider.GetBackups(this.page);
        }
    }
}
