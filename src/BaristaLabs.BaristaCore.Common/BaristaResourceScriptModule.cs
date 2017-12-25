namespace BaristaLabs.BaristaCore
{
    using System;
    using System.Globalization;
    using System.Resources;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a module that returns scripts from a resource file
    /// </summary>
    [BaristaModule("ResourceScriptModule", "Built-in module that allows for specifying a script as coming from a resource.")]

    public sealed class BaristaResourceScriptModule : IBaristaScriptModule
    {
        private readonly ResourceManager m_resourceManager;
        private readonly string m_name;
        private readonly string m_description;

        public BaristaResourceScriptModule(string name, ResourceManager resourceManager, string description = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            m_name = name;
            m_resourceManager = resourceManager ?? throw new ArgumentNullException(nameof(resourceManager));
            m_description = description;
        }

        /// <summary>
        /// Gets the name of the script module.
        /// </summary>
        public string Name
        {
            get { return m_name; }
        }

        /// <summary>
        /// Gets the description of the script module.
        /// </summary>
        public string Description
        {
            get { return m_description; }
        }

        public ResourceManager ResourceManager
        {
            get { return m_resourceManager; }
        }

        /// <summary>
        /// Gets or sets the culture that the resource will be retrieved for.
        /// </summary>
        public CultureInfo ResourceCulture
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the resource that will be used.
        /// </summary>
        public string ResourceName
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
