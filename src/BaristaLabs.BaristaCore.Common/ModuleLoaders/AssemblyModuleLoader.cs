namespace BaristaLabs.BaristaCore.ModuleLoaders
{
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a module loader that loads all modules in a specified assembly.
    /// </summary>
    public class AssemblyModuleLoader : IBaristaModuleLoader, IDisposable
    {
        private readonly ServiceCollection m_serviceCollection;
        private readonly Dictionary<string, Type> m_loadedModules;

        public AssemblyModuleLoader(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            m_loadedModules = new Dictionary<string, Type>();
            m_serviceCollection = new ServiceCollection();

            foreach (var moduleType in BaristaModuleTypeLoader.LoadBaristaModulesFromAssembly(assembly))
            {
                var baristaModuleAttribute = BaristaModuleAttribute.GetBaristaModuleAttributeFromType(moduleType);

                string targetModuleName = baristaModuleAttribute.Name;

                if (string.IsNullOrWhiteSpace(targetModuleName))
                    throw new InvalidOperationException($"The specfied module ({moduleType}) must indicate a name.");
                m_serviceCollection.AddTransient(typeof(IBaristaModule), moduleType);

                if (m_loadedModules.ContainsKey(targetModuleName))
                    throw new InvalidOperationException($"A module with the specified name ({targetModuleName}) has already been registered. Ensure all modules in the specified assembly have unique names.");

                m_loadedModules.Add(targetModuleName, moduleType);
            }
        }

        public Task<IBaristaModule> GetModule(string name)
        {
            //Try locating The Module
            if (TryGetModuleByName(name, out IBaristaModule newBaristaModule))
            {
                return Task.FromResult(newBaristaModule);
            }

            //Couldn't find one? Get outta Dodge.
            return null;
        }

        private bool TryGetModuleByName(string moduleName, out IBaristaModule baristaModule)
        {
            baristaModule = null;
            if (m_loadedModules.ContainsKey(moduleName))
            {
                var moduleType = m_loadedModules[moduleName];
                using (var serviceProvider = m_serviceCollection.BuildServiceProvider())
                {
                    var modules = serviceProvider.GetServices<IBaristaModule>();
                    baristaModule = modules.Where(s => s.GetType() == moduleType).FirstOrDefault();
                }
            }

            return baristaModule != null;
        }

        #region IDisposable Support
        private bool m_isDisposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!m_isDisposed)
            {
                if (disposing)
                {
                    //Do Nothing.
                }

                m_loadedModules.Clear();
                m_serviceCollection.Clear();
                m_isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
