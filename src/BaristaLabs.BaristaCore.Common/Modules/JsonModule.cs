namespace BaristaLabs.BaristaCore.Modules
{
    using System;

    /// <summary>
    /// Module that returns a json object as its default export.
    /// </summary>
    [BaristaModule("barista-json", "Built-in module that returns a json object. Not to be imported directly by scripts.", IsDiscoverable = false)]
    public class JsonModule : IBaristaModule
    {
        private readonly string m_name;
        private readonly string m_description;
        private readonly string m_jsonText;

        public JsonModule(string name, string description, string jsonText)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            m_name = name;
            m_description = description;
            m_jsonText = jsonText ?? throw new ArgumentNullException(nameof(jsonText));
        }

        /// <summary>
        /// Gets the name of the json module.
        /// </summary>
        public string Name
        {
            get { return m_name; }
        }

        /// <summary>
        /// Gets the description of the json module.
        /// </summary>
        public string Description
        {
            get { return m_description; }
        }

        /// <summary>
        /// Gets the json text associated with the module.
        /// </summary>
        public string JsonText
        {
            get { return m_jsonText; }
        }

        public JsValue ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
        {
            return context.JSON.Parse(context.CreateString(JsonText));
        }
    }
}
