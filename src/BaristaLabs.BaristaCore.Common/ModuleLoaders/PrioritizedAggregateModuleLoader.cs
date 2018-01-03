namespace BaristaLabs.BaristaCore.ModuleLoaders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a module loader that consolidates multiple module loaders into a single module loader with each module loader specifying a priority.
    /// </summary>
    public class PrioritizedAggregateModuleLoader : IBaristaModuleLoader
    {
        private readonly IList<ModuleLoaderRecord> m_moduleLoaders;

        public PrioritizedAggregateModuleLoader()
        {
            m_moduleLoaders = new List<ModuleLoaderRecord>();
        }

        /// <summary>
        /// Registers a module loader with the specified priority, optionally specifiying an initializer.
        /// </summary>
        /// <param name="priority"></param>
        /// <param name="moduleLoader"></param>
        /// <param name="moduleInitializer"></param>
        public void RegisterModuleLoader(IBaristaModuleLoader moduleLoader, int priority = 100, Func<string, int, IBaristaModuleLoader, Task<IBaristaModule>> moduleLoaderFactory = null)
        {
            if (moduleLoader == null)
                throw new ArgumentNullException(nameof(moduleLoader));

            if (moduleLoaderFactory == null)
                moduleLoaderFactory = InitializeAndReturnModule;

            m_moduleLoaders.Add(new ModuleLoaderRecord()
            {
                Priority = priority,
                ModuleLoader = moduleLoader,
                ModuleLoaderFactory = moduleLoaderFactory
            });
        }

        public Task<IBaristaModule> GetModule(string name)
        {
            var moduleLoaders = m_moduleLoaders.OrderBy(ml => ml.Priority);
            foreach(var moduleLoader in moduleLoaders)
            {
                var module = moduleLoader.ModuleLoaderFactory(name, moduleLoader.Priority, moduleLoader.ModuleLoader);
                if (module != null)
                    return module;
            }

            //We couldn't find a ModuleLoader out of any registered, That's it, we're done.
            return null;
        }

        /// <summary>
        /// Nominal implementation to retrieve a module.
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="modulePriority"></param>
        /// <param name="moduleLoader"></param>
        /// <returns></returns>
        private Task<IBaristaModule> InitializeAndReturnModule(string moduleName, int modulePriority, IBaristaModuleLoader moduleLoader)
        {
            return moduleLoader.GetModule(moduleName);
        }

        private class ModuleLoaderRecord
        {
            public IBaristaModuleLoader ModuleLoader
            {
                get;
                set;
            }

            public Func<string, int, IBaristaModuleLoader, Task<IBaristaModule>> ModuleLoaderFactory
            {
                get;
                set;
            }

            public int Priority
            {
                get;
                set;
            }
        }
    }
}
