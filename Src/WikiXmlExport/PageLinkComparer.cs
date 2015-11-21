namespace WikiXmlExport
{
    using System;
    using System.Collections.Generic;

    public class PageLinkComparer : IComparer<PageLink>, IEqualityComparer<PageLink>
    {
        private static readonly PageLinkComparer defaultComparer = new PageLinkComparer(true);

        private static readonly PageLinkComparer skipAnchorComparer = new PageLinkComparer(false);

        /// <summary>
        /// Indique si l'ancre doit être incluse lors de la comparaison de deux liens.
        /// </summary>
        private readonly bool includeAnchor;

        private PageLinkComparer(bool includeAnchor)
        {
            this.includeAnchor = includeAnchor;
        }

        public static PageLinkComparer DefaultComparer
        {
            get { return defaultComparer; }
        }

        /// <summary>
        /// Obtient une instance de <see cref="PageLinkComparer"/> considérant ignorant les différences d'ancres (#...) pour déterminer si deux liens sont identiques.
        /// </summary>
        public static PageLinkComparer SkipAnchorComparer
        {
            get { return skipAnchorComparer; }
        }

        public int Compare(PageLink x, PageLink y)
        {
            int result;

            result = string.Compare(x.Namespace, y.Namespace, StringComparison.OrdinalIgnoreCase);

            if (result != 0)
            {
                return result;
            }

            result = string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);

            if (result != 0)
            {
                return result;
            }

            if (!this.includeAnchor)
            {
                return 0;
            }

            return string.Compare(x.Anchor, y.Anchor, StringComparison.OrdinalIgnoreCase);
        }

        public bool Equals(PageLink x, PageLink y)
        {
            if (object.ReferenceEquals(x, y))
            {
                return true;
            }

            if (!string.Equals(x.Namespace, y.Namespace, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (!string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (!this.includeAnchor)
            {
                return true;
            }

            return string.Equals(x.Anchor, y.Anchor, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(PageLink obj)
        {
            if (!this.includeAnchor)
            {
                return obj.GetHashCode(true, false);
            }
            else
            {
                return obj.GetHashCode();
            }
        }
    }
}
