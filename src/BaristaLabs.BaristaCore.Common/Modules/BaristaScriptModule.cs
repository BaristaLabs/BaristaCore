namespace BaristaLabs.BaristaCore.Modules
{
    using System;

    /// <summary>
    /// Represents a module that returns a script that will be parsed when imported.
    /// </summary>
    [BaristaModule("ScriptModule", "Built-in module that allows for specifing the script as a string. Not to be imported directly by scripts.", IsDiscoverable = false)]
    public sealed class BaristaScriptModule : IBaristaScriptModule
    {
        private readonly string m_name;
        private readonly string m_description;

        public BaristaScriptModule(string name, string description = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            m_name = name;
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

        /// <summary>
        /// Gets or sets the script that will be executed.
        /// </summary>
        public string Script
        {
            get;
            set;
        }

        public JsValue ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
        {
            return context.CreateString(Script);
        }
    }
}
