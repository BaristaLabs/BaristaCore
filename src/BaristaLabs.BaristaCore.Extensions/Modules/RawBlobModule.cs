namespace BaristaLabs.BaristaCore.Modules
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Module that returns a blob as its default export.
    /// </summary>
    public class RawBlobModule : IBaristaModule
    {
        private readonly string m_name;
        private readonly string m_description;
        private readonly Blob m_blob;

        public RawBlobModule(string name, string description, Blob blob)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            m_name = name;
            m_description = description;
            m_blob = blob ?? throw new ArgumentNullException(nameof(blob));
        }

        /// <summary>
        /// Gets the name of the raw blob module.
        /// </summary>
        public string Name
        {
            get { return m_name; }
        }

        /// <summary>
        /// Gets the description of the raw blob module.
        /// </summary>
        public string Description
        {
            get { return m_description; }
        }

        /// <summary>
        /// Gets the blob associated with the module.
        /// </summary>
        public Blob Blob
        {
            get { return m_blob; }
        }

        public Task<object> ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
        {
            return Task.FromResult<object>(m_blob);
        }
    }
}
