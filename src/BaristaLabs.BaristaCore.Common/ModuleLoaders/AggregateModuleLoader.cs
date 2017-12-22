namespace BaristaLabs.BaristaCore.ModuleLoaders
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Represents a module loader that consolidates multiple module loaders into a single module loader. 
    /// </summary>
    public class AggregateModuleLoader : IBaristaModuleLoader
    {
        private readonly Dictionary<string, IBaristaModuleLoader> m_moduleLoaders;
        private string m_prefixSeperator = "!";
        
        public AggregateModuleLoader()
        {
            m_moduleLoaders = new Dictionary<string, IBaristaModuleLoader>();
            FallbackModuleLoader = null;
        }

        /// <summary>
        /// Gets or sets the prefix seperator that will be used to separate
        /// </summary>
        public string PrefixSeperator
        {
            get { return m_prefixSeperator; }
            set { m_prefixSeperator = value; }
        }

        /// <summary>
        /// Gets or sets the module loader that will be used if a prefix is not specified.
        /// </summary>
        public IBaristaModuleLoader FallbackModuleLoader
        {
            get;
            set;
        }

        public void RegisterModuleLoader(string prefix, IBaristaModuleLoader moduleLoader)
        {
            if (string.IsNullOrWhiteSpace(prefix))
                throw new ArgumentNullException(nameof(prefix));

            if (m_moduleLoaders.ContainsKey(prefix))
                throw new ArgumentException("A module loader with the specified prefix has already been registered.", nameof(prefix));

            if (moduleLoader == null)
                throw new ArgumentNullException(nameof(moduleLoader));

            m_moduleLoaders.Add(prefix, moduleLoader);
        }

        public bool RemoveModuleLoader(string prefix)
        {
            return m_moduleLoaders.Remove(prefix);
        }

        public IBaristaModule GetModule(string name)
        {
            var moduleNameRegex = new Regex($"^((?<Prefix>.+?){Regex.Escape(PrefixSeperator)})(?<Name>.+)$", RegexOptions.Compiled);
            var moduleName = name;

            if (moduleNameRegex.IsMatch(name))
            {
                var moduleNameMatch = moduleNameRegex.Match(name);
                string modulePrefix = null;
                if (moduleNameMatch.Groups["Prefix"] != null)
                    modulePrefix = moduleNameMatch.Groups["Prefix"].Value;

                moduleName = moduleNameMatch.Groups["Name"].Value;

                if (!string.IsNullOrWhiteSpace(modulePrefix) && m_moduleLoaders.ContainsKey(modulePrefix))
                {
                    var selectedModuleLoader = m_moduleLoaders[modulePrefix];
                    return selectedModuleLoader.GetModule(moduleName);
                }
                else
                {
                    //A prefix was specified, but no module loader with that prefix was found, return null.
                    return null;
                }
            }

            //We didn't have a prefix match, use the fallback module loader if specified.
            if (FallbackModuleLoader != null)
            {
                return FallbackModuleLoader.GetModule(moduleName);
            }

            //We didn't have a prefix match, no fallback module loader is specified. That's it, we're done.
            return null;
        }
    }
}
