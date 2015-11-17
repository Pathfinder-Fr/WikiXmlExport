using System.Resources;
using ScrewTurn.Wiki;

namespace PathfinderFr.WikiXmlExport
{
    /// <summary>
    /// Implements a Resource Exchanger.
    /// </summary>
    public class ResourceExchanger : IResourceExchanger
    {
        private ResourceManager manager;

        /// <summary>
        /// Initialises a new instance of the <b>ResourceExchanger</b> class.
        /// </summary>
        public ResourceExchanger()
        {
            manager = new ResourceManager(typeof(Properties.Messages).FullName, typeof(Properties.Messages).Assembly);
        }

        /// <summary>
        /// Gets a Resource String.
        /// </summary>
        /// <param name="name">The Name of the Resource.</param>
        /// <returns>The Resource String.</returns>
        public string GetResource(string name)
        {
            return manager.GetString(name);
        }

    }
}
