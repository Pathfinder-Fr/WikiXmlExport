namespace PathfinderFr.WikiXmlExport
{
    using System;
    using System.Collections.Generic;
    using ScrewTurn.Wiki.PluginFramework;

    /// <summary>
    /// Contient les informations sur les liens d'une page.
    /// </summary>
    public class PageLinkStatus
    {
        private readonly PageLink id;

        private readonly WikiPage page;

        private readonly List<PageLink> enteringLinks = new List<PageLink>();

        private List<PageLink> links;

        public PageLinkStatus(PageLink id, WikiPage page)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            this.id = id;
            this.page = page;
        }

        public PageLink Id
        {
            get { return this.id; }
        }

        public WikiPage Page
        {
            get { return this.page; }
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si la page désignée existe ou non.
        /// </summary>
        public bool Exists { get; set; }

        /// <summary>
        /// Obtient ou définit la liste des liens sortants de la page.
        /// </summary>
        public List<PageLink> Links
        {
            get { return this.links; }
            set { this.links = value; }
        }

        /// <summary>
        /// Obtient ou définit la liste des liens entrants de la page.
        /// </summary>
        public List<PageLink> EnteringLinks
        {
            get { return this.enteringLinks; }
        }
    }
}