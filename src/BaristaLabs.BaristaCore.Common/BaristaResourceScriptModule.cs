namespace BaristaLabs.BaristaCore
{
    using System;
    using System.Globalization;
    using System.Resources;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a module that returns scripts from a resource file
    /// </summary>
    public class BaristaResourceScriptModule : IBaristaScriptModule
    {
        private readonly ResourceManager m_resourceManager;

        public BaristaResourceScriptModule(ResourceManager resourceManager)
        {
            m_resourceManager = resourceManager ?? throw new ArgumentNullException(nameof(resourceManager));
        }

        /// <summary>
        /// Gets or sets the name of the script module.
        /// </summary>
        public virtual string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the description of the script module.
        /// </summary>
        public virtual string Description
        {
            get;
            set;
        }

        public virtual ResourceManager ResourceManager
        {
            get { return m_resourceManager; }
        }

        /// <summary>
        /// Gets or sets the culture that the resource will be retrieved for.
        /// </summary>
        public virtual CultureInfo ResourceCulture
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the resource that will be used.
        /// </summary>
        public virtual string ResourceName
        {
            get;
            set;
        }

        public Task<object> ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
        {
            return Task.FromResult<object>(m_resourceManager.GetString(ResourceName, ResourceCulture));
        }
    }
}
