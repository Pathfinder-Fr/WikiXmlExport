using System.Text;
using System.Xml;
using ScrewTurn.Wiki.PluginFramework;

namespace PathfinderFr.WikiXmlExport.Commands
{
    public class BuildIndexCommand : CommandBase
    {
        private readonly string outPath;

        public BuildIndexCommand(string fullName, string outPath, ILog log)
            : base(fullName, log)
        {
            this.outPath = outPath;
        }

        protected override void RunCore()
        {
            this.PopulateRedirections();

            using (var writer = XmlTextWriter.Create(this.outPath, new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true }))
            {
                writer.WriteStartDocument();

                writer.WriteStartElement("index");
                foreach (var page in this.Pages)
                {
                    var link = new PageLink(page);
                    if (!this.Redirections.ContainsKey(link))
                    {
                        writer.WriteStartElement("page");
                        writer.WriteAttributeString("ref", link.ToString());

                        var content = page.GetContent();
                        writer.WriteAttributeString("title", content.Title);

                        var categories = page.Categories;
                        if (categories.Length > 0)
                        {
                            writer.WriteStartElement("categories");

                            foreach (var category in categories)
                            {
                                writer.WriteStartElement("category");
                                writer.WriteAttributeString("name", NameTools.GetLocalName(category.FullName));
                                writer.WriteEndElement();
                            }

                            writer.WriteEndElement();
                        }

                        writer.WriteEndElement();
                    }
                }

                writer.WriteEndElement();

                writer.WriteEndDocument();
            }
        }
    }
}
