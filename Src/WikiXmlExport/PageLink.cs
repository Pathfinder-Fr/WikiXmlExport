namespace WikiXmlExport
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using ScrewTurn.Wiki.PluginFramework;

    public class PageLink
    {
        private readonly string ns;

        private readonly string name;

        private readonly string anchor;

        /// <summary>
        /// Nom complet du lien, sans les ancres.
        /// </summary>
        private string fullName;

        /// <summary>
        /// Nom complet du lien avec les ancres.
        /// </summary>
        private string fullNameWithAnchor;

        public PageLink(WikiPage page)
            : this(NameTools.GetNamespace(page.FullName), NameTools.GetLocalName(page.FullName), null)
        {
        }

        public PageLink(string fullName)
            : this(NameTools.GetNamespace(fullName), NameTools.GetLocalName(fullName), null)
        {
        }

        public PageLink(Match match)
        {
            this.name = Uri.UnescapeDataString(match.Groups["Name"].Value);

            Group group;

            group = match.Groups["Namespace"];
            if (group.Success)
                this.ns = Uri.UnescapeDataString(group.Value);

            group = match.Groups["Anchor"];
            if (group.Success)
                this.anchor = Uri.UnescapeDataString(group.Value);
        }

        public PageLink(string ns, string name, string anchor)
        {
            this.ns = ns;
            this.name = name;
            this.anchor = anchor;

            if (this.ns == string.Empty)
                this.ns = null;

            if (this.anchor == string.Empty)
                this.anchor = null;
        }

        public string Namespace
        {
            get { return this.ns; }
        }

        public string Name
        {
            get { return this.name; }
        }

        public string FullName
        {
            get { return NameTools.GetFullName(this.ns, this.name); }
        }

        public string Anchor
        {
            get { return this.anchor; }
        }

        public static string SanitizeName(string name)
        {
            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(name.Replace(' ', '-').ToLowerInvariant());
        }

        public static string SanitizeAnchor(string anchor)
        {
            if (anchor == null)
                return null;

            // On ne passe plus le lien en minuscules car cela casse les liens internes
            //return anchor.ToLowerInvariant();
            return anchor;
        }

        public override string ToString()
        {
            return this.ToString(true, true);
        }

        public string ToString(bool withNamespace, bool withAnchor)
        {
            if (withNamespace)
            {
                if (withAnchor)
                {
                    return this.fullName ?? (this.fullName = this.ToStringCore("{0}.{1}#{2}", "{1}#{2}", "{0}.{1}", "{1}", null, true, false));
                }
                else
                {
                    return this.fullNameWithAnchor ?? (this.fullNameWithAnchor = this.ToStringCore("{0}.{1}#{2}", "{1}#{2}", "{0}.{1}", "{1}", null, true, true));
                }
            }
            else
            {
                return this.ToStringCore("{0}.{1}#{2}", "{1}#{2}", "{0}.{1}", "{1}", null, withNamespace, withAnchor);
            }
        }

        public string ToStaticHref()
        {
            return ToStaticHref(null);
        }

        public string ToStaticHref(string currentNamespace)
        {
            string format;

            if (this.anchor != null && this.ns != null && this.ns != currentNamespace)
            {
                format = "href=\"../{0}/{1}.html#{2}\"";
            }
            else if (this.anchor != null)
            {
                format = "href=\"{1}.html#{2}\"";
            }
            else if (this.ns != null && this.ns != currentNamespace)
            {
                format = "href=\"../{0}/{1}.html\"";
            }
            else
            {
                format = "href=\"{1}.html\"";
            }

            return string.Format(CultureInfo.InvariantCulture, format, this.ns, SanitizeName(this.name), SanitizeAnchor(this.anchor));
        }

        public string ToWikiHref()
        {
            return this.ToWikiHref(null);
        }

        public string ToWikiHref(string currentNamespace)
        {
            return ToStringCore("{0}.{1}.ashx#{2}", "{1}.ashx#{2}", "{0}.{1}.ashx", "{1}.ashx", currentNamespace, true, true);
        }

        private string ToStringCore(string fullFormat, string anchorFormat, string nsFormat, string minFormat, string currentNamespace, bool withNamespace, bool withAnchor)
        {
            string format;

            if (withAnchor && withNamespace && this.anchor != null && this.ns != null && this.ns != currentNamespace)
            {
                format = fullFormat;
            }
            else if (withAnchor && this.anchor != null)
            {
                format = anchorFormat;
            }
            else if (withNamespace && this.ns != null)
            {
                format = nsFormat;
            }
            else
            {
                format = minFormat;
            }

            return string.Format(CultureInfo.InvariantCulture, format, this.ns, this.name, this.anchor);
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj))
                return true;

            if (null == obj)
                return false;

            var other = obj as PageLink;
            if (null == other)
                return base.Equals(obj);

            return string.Equals(this.ns, other.ns, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(this.name, other.name, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(this.anchor, other.anchor, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return this.ToString().ToLowerInvariant().GetHashCode();
        }

        public int GetHashCode(bool withNamespace, bool withAnchor)
        {
            return this.ToString(withNamespace, withAnchor).ToLowerInvariant().GetHashCode();
        }
    }
}
