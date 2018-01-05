namespace BaristaLabs.BaristaCore.Modules
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Module that returns a text value as its default export.
    /// </summary>
    [BaristaModule("barista-raw-text", "Built-in module that returns a text value. Not to be imported directly by scripts.", IsDiscoverable = false)]
    public class RawTextModule : IBaristaModule
    {
        private readonly string m_name;
        private readonly string m_description;
        private readonly string m_rawText;

        public RawTextModule(string name, string description, string rawText)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            m_name = name;
            m_description = description;
            m_rawText = rawText ?? throw new ArgumentNullException(nameof(rawText));
        }

        /// <summary>
        /// Gets the name of the raw text module.
        /// </summary>
        public string Name
        {
            get { return m_name; }
        }

        /// <summary>
        /// Gets the description of the raw text module.
        /// </summary>
        public string Description
        {
            get { return m_description; }
        }

        /// <summary>
        /// Gets the text associated with the module.
        /// </summary>
        public string RawText
        {
            get { return m_rawText; }
        }

        public Task<object> ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
        {
            return Task.FromResult<object>(m_rawText);
        }
    }
}
