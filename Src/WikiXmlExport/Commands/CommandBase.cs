namespace PathfinderFr.WikiXmlExport.Commands
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using ScrewTurn.Wiki;
    using ScrewTurn.Wiki.PluginFramework;

    public abstract class CommandBase
    {
        private const string RedirectionSnippetPattern = @"{s:Redirection\|((?<Namespace>[^\.]+)\.)?(?<Name>[^\|]+)(\|(?<Anchor>[^}]+))?}";

        private const string RedirectionPattern = @">>> ((?<Namespace>[^\.]+)\.)?(?<Name>[^\|]+)";

        private readonly ConcurrentDictionary<PageLink, PageLink> redirections = new ConcurrentDictionary<PageLink, PageLink>(PageLinkComparer.SkipAnchorComparer);

        private readonly ConcurrentDictionary<PageLink, PageLinkStatus> pageLinks = new ConcurrentDictionary<PageLink, PageLinkStatus>(PageLinkComparer.SkipAnchorComparer);

        private readonly string fullName;

        private readonly ILog log;

        private List<WikiPage> pagesCache;

        private Stopwatch watch;

        private int step;

        private int count;

        private int stepSize;

        private Formatter formatter;

        protected CommandBase(string fullName, ILog log)
        {
            this.log = log ?? Logging.NullLog.Instance;
            this.fullName = fullName;

            if (this.fullName.StartsWith("\"") && this.fullName.EndsWith("\""))
            {
                this.fullName = this.fullName.Substring(1, this.fullName.Length - 2);
            }
        }

        protected ILog Log
        {
            get { return this.log; }
        }

        protected string FullName
        {
            get { return this.fullName; }
        }

        /// <summary>
        /// Obtient la liste des redirections, sous la forme Source vers Cible.
        /// </summary>
        protected IDictionary<PageLink, PageLink> Redirections
        {
            get { return this.redirections; }
        }

        /// <summary>
        /// Obtient, pour une page, la liste des pages liées.
        /// </summary>
        protected IDictionary<PageLink, PageLinkStatus> PageLinks
        {
            get { return this.pageLinks; }
        }

        protected Func<WikiPage, bool> CustomFilter { get; set; }

        protected List<WikiPage> Pages
        {
            get { return this.pagesCache; }
        }

        public void Run()
        {
            // On commence par charger l'ensemble des pages du wiki
            this.Load();

            // On exécute la commande
            this.RunCore();
        }

        protected abstract void RunCore();

        protected WikiPage FindPage(string fullName)
        {
            return this.pagesCache.FirstOrDefault(p => p.FullName == fullName);
        }

        protected void PopulateRedirections()
        {
            Parallel.ForEach(this.Pages, DetectPageRedirection);

            var copy = this.redirections.ToList();

            foreach (var redirection in copy)
            {
                var target = redirection.Value;

                int i = 0;
                PageLink newTarget;
                while (this.redirections.TryGetValue(target, out newTarget) && i < 10)
                {
                    // Continue
                    //this.log.Verbose("Chaîne de redirection détectée pour {0} : {1} => {2}", redirection.Key, target, newTarget);
                    target = newTarget;
                    i++;
                }

                if (i == 10)
                {
                    throw new InvalidOperationException(string.Format("Redirection trop profonde (>= 10) depuis {0}", redirection.Key));
                }
                else if (i > 0)
                {
                    this.log.Info(string.Format("Redirection modifiée pour {0} : {1} => {2}", redirection.Key, redirection.Value, target));
                    this.redirections[redirection.Key] = target;
                }
            }
        }

        private void DetectPageRedirection(WikiPage page)
        {
            if (page == null)
            {
                return;
            }

            var content = page.GetContent();

            if (content == null)
            {
                return;
            }

            // Détection via snippet
            var match = Regex.Match(content.Content, RedirectionSnippetPattern, RegexOptions.ExplicitCapture);

            if (!match.Success)
            {
                // Détection url standard
                match = Regex.Match(content.Content, RedirectionPattern, RegexOptions.ExplicitCapture);
            }

            if (match.Success)
            {
                // La page est une redirection
                var sourceLink = new PageLink(page);
                var targetLink = new PageLink(match);
                this.redirections.TryAdd(sourceLink, targetLink);

                PageLinkStatus pageStatus = this.pageLinks.GetOrAdd(sourceLink, x => new PageLinkStatus(x, page));

                if (pageStatus.Links == null)
                {
                    pageStatus.Links = new List<PageLink>(1);
                }

                pageStatus.Exists = true;
                pageStatus.Links.Add(targetLink);

                var linkStatus = this.pageLinks.GetOrAdd(targetLink, x =>
                {
                    // La page destination n'existe pas encore, on la créée
                    var targetPage = this.FindPage(NameTools.GetFullName(x.Namespace, x.Name));
                    return new PageLinkStatus(new PageLink(x.Namespace, x.Name, null), targetPage);
                });

                linkStatus.EnteringLinks.Add(sourceLink);

                this.log.Info(string.Format("Redirection détectée : {0} => {1}", sourceLink, targetLink));
            }
        }

        private void Load()
        {
            this.log.Info("*** Chargement des pages ***");

            this.pagesCache = new List<WikiPage>(8000);

            foreach (var provider in Collectors.PagesProviderCollector.AllProviders)
            {
                this.log.Verbose("Démarrage chargement pages pour le fournisseur {0}", provider.GetType().Name);

                if (this.fullName == "*")
                {
                    foreach (var ns in provider.GetNamespaces())
                    {
                        this.LoadNamespace(ns);
                    }
                }
                else if (this.fullName.StartsWith("++"))
                {
                    // Full Namespace
                    var nsName = this.fullName.Substring(2);

                    var ns = provider.GetNamespace(nsName);

                    if (ns != null)
                    {
                        this.LoadNamespace(ns);
                    }
                }
                else
                {
                    var page = new WikiPage(provider.GetPage(this.fullName));

                    if (page == null)
                    {
                        this.log.Warning("Impossible de charger la page {0} : résultat null", this.fullName);
                    }

                    if (page != null && (this.CustomFilter == null || this.CustomFilter(page)))
                    {
                        this.pagesCache.Add(page);
                    }
                }
            }
        }

        private void LoadNamespace(NamespaceInfo ns)
        {
            this.log.Info("*** Chargement de l'espace de noms {0} ***", ns.Name);

            var pages = ns.Provider
                .GetPages(ns)
                .Select(page => new WikiPage(page));

            if (this.CustomFilter != null)
            {
                pages = pages.Where(this.CustomFilter);
            }

            // Nombre de pages
            var pagesList = pages.ToList();

            this.count = pagesList.Count;

            this.step = 1;
            this.stepSize = 2000;

            this.watch = new Stopwatch();
            this.watch.Start();

            for (int i = 0; i < count; i++)
            {
                var page = pagesList[i];
                page.GetContent();

                var elapsed = this.watch.ElapsedMilliseconds;
                if (elapsed > this.step * stepSize && i != 0)
                {
                    var totalTime = (elapsed * count) / i;
                    var remaining = TimeSpan.FromMilliseconds(totalTime - elapsed);
                    this.log.Verbose("{0:p1} {1}/{2} (reste {3}m {4}s)", (float)i / count, i, count, (int)remaining.TotalMinutes, remaining.Seconds);
                    this.step = ((int)elapsed / stepSize) + 1;
                }

                this.pagesCache.Add(page);
            }

            watch.Stop();
            this.log.Verbose("Chargement espace de noms terminé : {0} pages chargées en {1}m {2}s ({3:0} pages / minute)", count, (int)watch.Elapsed.TotalMinutes, watch.Elapsed.Seconds, count / watch.Elapsed.TotalMinutes);


            this.log.Info("*** Formattage de l'espace de noms {0} ***", ns.Name);
            this.formatter = new Formatter();
            this.watch = new Stopwatch();
            this.watch.Start();

            this.step = 1;

            Parallel.ForEach(pagesList, this.FormatPage);

            watch.Stop();
            this.log.Verbose("Formattage terminé : {0} pages traitées en {1}m {2}s ({3:0} pages / minute)", count, (int)watch.Elapsed.TotalMinutes, watch.Elapsed.Seconds, count / watch.Elapsed.TotalMinutes);
        }

        private int x;

        private void FormatPage(WikiPage page)
        {
            var current = Interlocked.Increment(ref x);

            try
            {
                page.Load(formatter);
            }
            catch(Exception ex)
            {
                this.log.Error("Erreur survenue durant le chargement de la page {0} : chargement annulé. Détail de l'erreur : {1}", page.FullName, ex);
            }

            if (current % 10 == 0)
            {
                lock (this)
                {
                    var elapsed = this.watch.ElapsedMilliseconds;
                    if (elapsed > step * stepSize && current != 0)
                    {
                        var totalTime = (elapsed * count) / current;
                        var remaining = TimeSpan.FromMilliseconds(totalTime - elapsed);
                        this.log.Verbose("{0:p1} {1}/{2} (reste {3}m {4}s)", (float)current / count, current, count, (int)remaining.TotalMinutes, remaining.Seconds);
                        this.step = ((int)elapsed / stepSize) + 1;
                    }
                }
            }

        }
    }
}