namespace BaristaLabs.BaristaCore.ModuleLoaders
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a simple in-memory barista module loader.
    /// </summary>
    public class InMemoryModuleLoader : IBaristaModuleLoader
    {
        private IDictionary<string, IBaristaModule> m_modules = new Dictionary<string, IBaristaModule>();

        public void RegisterModule(IBaristaModule module)
        {
            if (module == null)
                throw new ArgumentNullException(nameof(module));

            string name;
            if (module is IBaristaScriptModule scriptModule)
            {
                name = scriptModule.Name;
            }
            else
            {
                var baristaModuleAttribute = BaristaModuleAttribute.GetBaristaModuleAttributeFromType(module.GetType());
                name = baristaModuleAttribute.Name;
            }
            
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidOperationException("The specfied module must indicate a name.");

            if (m_modules.ContainsKey(name))
                throw new InvalidOperationException($"A module with the specified name ({name}) has already been registered.");

            m_modules.Add(name, module);
        }

        public IBaristaModule GetModule(string name)
        {
            if (m_modules.ContainsKey(name))
            {
                return m_modules[name];
            }

            return null;
        }
    }
}
