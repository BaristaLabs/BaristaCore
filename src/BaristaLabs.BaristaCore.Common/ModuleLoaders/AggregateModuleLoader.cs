namespace BaristaLabs.BaristaCore.ModuleLoaders
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Represents a module loader that consolidates multiple module loaders into a single module loader using a module name prefix as a specifier. 
    /// </summary>
    public class AggregateModuleLoader : IBaristaModuleLoader
    {
        private readonly Dictionary<string, Tuple<IBaristaModuleLoader, Func<string, string, IBaristaModuleLoader, IBaristaModule>>> m_moduleLoaders;
        private string m_prefixSeperator = "!";
        
        public AggregateModuleLoader()
        {
            m_moduleLoaders = new Dictionary<string, Tuple<IBaristaModuleLoader, Func<string, string, IBaristaModuleLoader, IBaristaModule>>>();
            FallbackModuleLoader = null;
        }

        /// <summary>
        /// Gets or sets the prefix seperator that will be used to separate the module prefix from the module name. Defaults to '!' 
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

        /// <summary>
        /// Registers a module loader for the specified prefix, optionally specifiying an initializer.
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="moduleLoader"></param>
        /// <param name="moduleInitializer"></param>
        public void RegisterModuleLoader(string prefix, IBaristaModuleLoader moduleLoader, Func<string, string, IBaristaModuleLoader, IBaristaModule> moduleLoaderFactory = null)
        {
            if (string.IsNullOrWhiteSpace(prefix))
                throw new ArgumentNullException(nameof(prefix));

            if (m_moduleLoaders.ContainsKey(prefix))
                throw new ArgumentException("A module loader with the specified prefix has already been registered.", nameof(prefix));

            if (moduleLoader == null)
                throw new ArgumentNullException(nameof(moduleLoader));

            if (moduleLoaderFactory == null)
                moduleLoaderFactory = InitializeAndReturnModule;

            m_moduleLoaders.Add(prefix, new Tuple<IBaristaModuleLoader, Func<string, string, IBaristaModuleLoader, IBaristaModule>>(moduleLoader, moduleLoaderFactory));
        }

        /// <summary>
        /// Removes the module loader with the specified prefix.
        /// </summary>
        /// <param name="prefix">The prefix to remove.</param>
        /// <returns>true if the element is successfully found and removed; otherwise, false.</returns>
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
                    var moduleLoaderRecord = m_moduleLoaders[modulePrefix];
                    var moduleLoader = moduleLoaderRecord.Item1;
                    var moduleLoaderFactory = moduleLoaderRecord.Item2;

                    return moduleLoaderFactory(modulePrefix, moduleName, moduleLoader);
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

        /// <summary>
        /// Nominal implementation to retrieve a module.
        /// </summary>
        /// <param name="modulePrefix"></param>
        /// <param name="moduleName"></param>
        /// <param name="moduleLoader"></param>
        /// <returns></returns>
        private IBaristaModule InitializeAndReturnModule(string modulePrefix, string moduleName, IBaristaModuleLoader moduleLoader)
        {
            return moduleLoader.GetModule(moduleName);
        }
    }
}
