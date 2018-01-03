namespace BaristaLabs.BaristaCore.ModuleLoaders
{
    using BaristaLabs.BaristaCore.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

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

            var name = module.GetModuleName();

            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidOperationException("The specfied module must indicate a name.");

            if (m_modules.ContainsKey(name))
                throw new InvalidOperationException($"A module with the specified name ({name}) has already been registered.");

            m_modules.Add(name, module);
        }

        public Task<IBaristaModule> GetModule(string name)
        {
            if (m_modules.ContainsKey(name))
            {
                return Task.FromResult(m_modules[name]);
            }

            return null;
        }
    }
}
