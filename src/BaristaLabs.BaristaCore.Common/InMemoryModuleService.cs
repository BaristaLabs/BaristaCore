namespace BaristaLabs.BaristaCore
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a simple in-memory barista module service.
    /// </summary>
    public class InMemoryModuleService : IBaristaModuleService
    {
        private IDictionary<string, IBaristaModule> m_modules = new Dictionary<string, IBaristaModule>();

        public void RegisterModule(IBaristaModule module)
        {
            if (string.IsNullOrWhiteSpace(module.Name))
                throw new InvalidOperationException("The specfied module must indicate a name.");

            if (m_modules.ContainsKey(module.Name))
                throw new InvalidOperationException("A module with the specified name has already been registered.");

            m_modules.Add(module.Name, module);
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
