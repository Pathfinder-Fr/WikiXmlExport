namespace WikiXmlExport.Commands
{
    using ScrewTurn.Wiki;
    using ScrewTurn.Wiki.PluginFramework;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    public class ExtractPagesCommand : CommandBase
    {
        private const string AnchorHrefPattern = @"href=""((?<Namespace>[\w \-]+)\.)?(?<Name>[\w \-\'\(\)\,\/\’\%]+).ashx(#(?<Anchor>[\w \-\(\)]+))?""";

        private const string AshxHrefPattern = @"href=""(?<Href>[^""]+.ashx([^""])*)""";

        private readonly string outDir;

        private WikiPage currentPage;

        private PageLink currentPageLink;

        private bool trackCrossNamespaceLinks;

        private string filter;

        private string templateContent;

        public ExtractPagesCommand(string fullName, string outDir, ILog log)
            : base(fullName, log)
        {
            this.trackCrossNamespaceLinks = fullName == "*";
            this.outDir = outDir;
        }

        public string Extension { get; set; }

        public string Filter
        {
            get { return this.filter; }
            set
            {
                if (this.filter != value)
                {
                    this.filter = value;
                    if (this.filter != null)
                        base.CustomFilter = this.IsAllowedByFilter;
                    else
                        base.CustomFilter = null;
                }
            }
        }

        public string Template { get; set; }

        public static String RemoveDiacritics(string s)
        {
            String normalizedString = s.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder(s.Length);

            for (int i = 0; i < normalizedString.Length; i++)
            {
                Char c = normalizedString[i];
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    stringBuilder.Append(c);
            }

            return stringBuilder.ToString();
        }

        protected override void RunCore()
        {
            if (this.Extension == null)
            {
                this.Extension = Path.GetExtension(this.Template).Substring(1);
            }

            this.Log.Verbose("Début de l'extraction des pages");
            this.Log.Verbose("Template: {0}", this.Template);
            this.Log.Verbose("Filter: {0}", this.Filter);
            this.Log.Verbose("Extension: {0}", this.Extension);
            this.Log.Verbose("FullName: {0}", this.FullName);
            this.Log.Verbose("OutDir: {0}", this.outDir);

            if (!string.IsNullOrEmpty(this.Template))
            {
                templateContent = File.ReadAllText(this.Template);
            }

            this.PopulateRedirections();

            this.ExtractPages();

            this.AnalyzePages();
        }

        private void ExtractPages()
        {
            this.Log.Info("*** Analyse des pages ***");
            int pageCount = 0;
            foreach (var page in this.Pages)
            {
                this.BuildPageLinks(page);
                pageCount++;
            }

            this.Log.Info("Analyse terminée, {0} pages traitées", pageCount);

            this.Log.Info("*** Extraction des pages ***");
            pageCount = 0;
            foreach (var page in this.Pages)
            {
                this.ExtractPage(page);
                pageCount++;
            }

            this.Log.Info("Extraction terminée, {0} pages traitées", pageCount);
        }

        /// <summary>
        /// Vérifie si la page répond au filtre des pages <see cref="Filter"/>.
        /// </summary>
        private bool IsAllowedByFilter(WikiPage page)
        {
            if (page == null || string.IsNullOrEmpty(this.Filter))
            {
                return true;
            }

            if (this.Filter[0] == '-')
            {
                // Sauf les pages qui appartiennent à la catégorie donnée
                var filter = this.Filter.Substring(1);
                return page.Categories.All(c => !c.FullName.Equals(filter, StringComparison.OrdinalIgnoreCase));
            }

            // Uniquement les pages qui appartiennent à la catégorie donnée
            return page.Categories.Any(c => c.FullName.Equals(this.Filter, StringComparison.OrdinalIgnoreCase));
        }

        private void BuildPageLinks(WikiPage page)
        {
            //this.Log.Write("Construction liens page {0}", page.FullName);

            this.currentPageLink = new PageLink(page);

            if (this.Redirections.ContainsKey(this.currentPageLink))
            {
                // Cette page est une redirection, on l'ignore
                this.Log.Info("Construction liens page {0} ignorée, redirection", page.FullName);
                return;
            }

            this.currentPage = page;

            var pageContent = page.GetContent();

            if (pageContent == null)
            {
                this.Log.Info("Construction liens page {0} ignorée, vide", page.FullName);
                return;
            }

            var formatter = new Formatter();
            var formatted = string.Empty;
            try
            {
                formatted = page.FormatWithPhase1And2(formatter);
            }
            catch (Exception ex)
            {
                this.Log.Error("Erreur durant la mise en forme de la page {0}, extraction annulée. Détail : {1}", page.FullName, ex);
            }

            // We create a list with all links in this page
            var pageLinks = new List<PageLink>(page.Links.Length);
            foreach (var link in page.Links)
            {
                // We create a PageLink for this link
                string linkNS, linkName;
                NameTools.ExpandFullName(link, out linkNS, out linkName);

                if (!this.trackCrossNamespaceLinks && linkNS != this.currentPageLink.Namespace)
                {
                    // Link with another namespace, we do not track them
                    continue;
                }

                int index = linkName.IndexOf('#');
                if (index > -1)
                {
                    linkName = linkName.Substring(0, index);
                }

                var pageLink = new PageLink(linkNS, linkName, null);

                // Pas sur que cela ait un intérêt : les liens dans la page envoient toujours vers la page qui n'existe plus, donc
                // peu d'intérêt de modifier les liens dans les métadonnées de la page.
                // Il faudrait peut-être à terme générer aussi les pages de redirection sous forme de fichiers, afin de permettre de les reconstruire ensuite.
                //// We eventually replace the link with the real redirected page
                ////PageLink redirectedLink;
                ////if (this.Redirections.TryGetValue(pageLink, out redirectedLink))
                ////{
                ////    pageLink = new PageLink(redirectedLink.Namespace, redirectedLink.Name, redirectedLink.Anchor);
                ////}

                ////if (!pageLinks.Contains(pageLink))
                ////{
                ////    pageLinks.Add(pageLink);
                ////}
                pageLinks.Add(pageLink);


                PageLinkStatus linkStatus;

                if (!this.PageLinks.TryGetValue(pageLink, out linkStatus))
                {
                    // The linked page is not yet registered (existing or not),
                    // we create then register it
                    var linkedPage = this.FindPage(pageLink.FullName);
                    linkStatus = new PageLinkStatus(pageLink, linkedPage);
                    this.PageLinks.Add(pageLink, linkStatus);
                }

                // We add this page as an incoming link
                linkStatus.EnteringLinks.Add(this.currentPageLink);
            }

            // We check the current page status
            PageLinkStatus pageStatus;
            if (!this.PageLinks.TryGetValue(this.currentPageLink, out pageStatus))
            {
                // This page is not yet registered (not yet linked),
                // we create then register it
                var linkedPage = this.FindPage(this.currentPageLink.FullName);
                pageStatus = new PageLinkStatus(this.currentPageLink, linkedPage);
                this.PageLinks.Add(this.currentPageLink, pageStatus);
            }

            // The page now exists
            pageStatus.Exists = true;

            // We set all outgoing links
            pageStatus.Links = pageLinks;
        }

        private void ExtractPage(WikiPage page)
        {
            this.currentPageLink = new PageLink(page);
            this.currentPage = page;

            if (this.Redirections.ContainsKey(this.currentPageLink))
            {
                // Cette page est une redirection, on l'ignore
                this.Log.Info("Extraction page {0} ignorée, redirection", page.FullName);

                return;
            }

            var pageContent = page.GetContent();

            if (pageContent == null)
            {
                this.Log.Info("Extraction page {0} ignorée, vide", page.FullName);
                return;
            }

            List<string> warnings = new List<string>();

            var formatter = new Formatter();
            string formatted = string.Empty;
            try
            {
                formatted = page.FormatWithPhase3(formatter);
            }
            catch (Exception ex)
            {
                this.Log.Error("Erreur durant la mise en forme de la page {0}, extraction annulée. Détail : {1}", page.FullName, ex);
                warnings.Add("Erreur lors de la mise en page, opération annulée");
            }


            // Traitements personnalisés
            formatted = Regex.Replace(formatted, AnchorHrefPattern, ReplaceLinks);

            var matches = Regex.Matches(formatted, AshxHrefPattern);
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    warnings.Add(string.Format("Lien en dur : [url=http://www.pathfinder-fr.org/Wiki/{0}]{0}[/url]", match.Groups["Href"].Value));
                }
            }

            var ns = NameTools.GetNamespace(page.FullName);

            if (string.IsNullOrEmpty(ns))
            {
                ns = ".";
            }

            var fullPath = Path.Combine(outDir, string.Format(@"{0}\{1}.{2}", ns, PageLink.SanitizeName(NameTools.GetLocalName(page.FullName)), this.Extension));
            var dir = Path.GetDirectoryName(fullPath);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string newContent;

            if (!string.IsNullOrEmpty(Template))
            {
                // Catégories
                var categoryBuilder = new StringBuilder();
                foreach (var category in page.Categories)
                {
                    if (category != null)
                    {
                        categoryBuilder.AppendFormat("<category>{0}</category>", category.FullName);
                    }
                }

                var categories = categoryBuilder.ToString();

                var currentPageLinks = this.PageLinks[currentPageLink];

                // Liens entrants
                string inLinks = string.Empty;
                foreach (var link in currentPageLinks.EnteringLinks)
                {
                    inLinks += "<link>" + link.FullName + "</link>";
                }

                // Liens sortants
                string outLinks = string.Empty;
                foreach (var link in currentPageLinks.Links)
                {
                    outLinks += "<link>" + link.FullName + "</link>";
                }

                newContent = string.Format(
                    templateContent,
                    formatted, // 0
                    pageContent.Title,
                    categories,
                    inLinks,
                    outLinks,
                    pageContent.LastModified.ToString("u"), // 5
                    page.ProviderGetBackups().Max<int, int?>(i => i) ?? 0,
                    System.Net.WebUtility.HtmlEncode(formatted),
                    pageContent.Content, // 8                    
                    page.FullName // 9
                );
            }
            else
            {
                newContent = formatted;
            }

            if (File.Exists(fullPath))
            {
                var bytes = Encoding.Default.GetBytes(newContent);

                string newHash = string.Empty;
                string oldHash = string.Empty;

                using (Crc32 crc = new Crc32())
                {
                    foreach (var hashByte in crc.ComputeHash(bytes))
                    {
                        newHash += hashByte.ToString("x2");
                    }

                    using (var fs = File.OpenRead(fullPath))
                    {
                        foreach (var hashByte in crc.ComputeHash(fs))
                        {
                            oldHash += hashByte.ToString("x2");
                        }
                    }
                }

                if (oldHash.Equals(newHash, StringComparison.Ordinal))
                {
                    this.Log.Verbose("Extraction page {0} ignorée, non modifiée", page.FullName);
                    return;
                }
            }

            using (var writer = new StreamWriter(fullPath, false))
            {
                writer.Write(newContent);
            }

            if (warnings.Count == 0)
            {
                this.Log.Verbose("Extraction page {0} terminée", page.FullName);
            }
            else
            {
                this.Log.Warning("Extraction page {0} terminée avec avertissements :", page.FullName);

                foreach (var line in warnings)
                {
                    this.Log.Warning("- {0}", line);
                }
            }
        }

        private string ReplaceLinks(Match match)
        {
            var link = new PageLink(match);

            var pageLink = new PageLink(link.Namespace ?? this.currentPageLink.Namespace, link.Name, null);

            PageLink redirect;

            var currentNs = NameTools.GetNamespace(this.currentPage.FullName);
            if (this.Redirections.TryGetValue(pageLink, out redirect))
            {
                return redirect.ToStaticHref(currentNs);
            }
            else
            {
                return link.ToStaticHref(currentNs);
            }
        }

        private void AnalyzePages()
        {
            // Liste des pages manquantes (qui n'existent pas)
            var missingPages = this.PageLinks.Values
                .Where(p => !p.Exists && this.IsAllowedByFilter(p.Page))
                .OrderBy(p => p.Id.Name);

            foreach (var status in missingPages)
            {
                this.Log.Warning(
                    "Page manquante {1} (http://www.pathfinder-fr.org/Wiki/{0}), référencée par : {2}",
                    status.Id.ToWikiHref(),
                    status.Id.Name,
                    string.Join(", ", status.EnteringLinks.Take(5).Select(i => string.Format("{1} (http://www.pathfinder-fr.org/Wiki/{0})", i.ToWikiHref(), i.Name)))
                    );
            }

            // Liste des pages orphelines
            var orphans = this.PageLinks.Values
                .Where(p => p.EnteringLinks.Count == 0 && p.Exists && this.IsAllowedByFilter(p.Page))
                .OrderBy(p => p.Id.Name);

            foreach (var status in orphans)
            {
                this.Log.Warning("Page orpheline {1} (http://www.pathfinder-fr.org/Wiki/{0})", status.Id.ToWikiHref(), status.Id.Name);
            }
        }
    }
}
