namespace BaristaLabs.BaristaCore.ModuleLoaders
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Represents a module loader that scans assemblies in a specified folder for all IBaristaModule implementations.
    /// </summary>
    public class AssembliesInPathModuleLoader : IBaristaModuleLoader, IDisposable
    {
        private readonly string m_moduleFolderFullPath;
        private readonly Dictionary<string, IBaristaModule> m_loadedModules;

        public AssembliesInPathModuleLoader(string modulePath = ".\\barista_modules")
        {
            if (string.IsNullOrWhiteSpace(modulePath))
                throw new ArgumentNullException(nameof(modulePath));

            m_moduleFolderFullPath = Path.GetFullPath(modulePath);
            m_loadedModules = new Dictionary<string, IBaristaModule>();
        }

        public IBaristaModule GetModule(string name)
        {
            //Look for any already loaded modules
            if (m_loadedModules.ContainsKey(name))
                return m_loadedModules[name];

            //If not found scan the configured folder.
            ScanModuleFolder();

            if (m_loadedModules.ContainsKey(name))
                return m_loadedModules[name];

            //Couldn't find one? Get outta Dodge.
            return null;
        }

        /// <summary>
        /// Scans the configured folder for assemblies.
        /// </summary>
        private void ScanModuleFolder()
        {
            var moduleFolderPathInfo = new DirectoryInfo(m_moduleFolderFullPath);
            if (!moduleFolderPathInfo.Exists)
                return;

            //Add our custom assembly resolver
            AppDomain.CurrentDomain.AssemblyResolve += ModuleAssemblyLoader;
            try
            {
                foreach (var assemblyFileInfo in moduleFolderPathInfo.GetFileSystemInfos("*.dll"))
                {
                    LoadBaristaModulesFromAssembly(assemblyFileInfo.FullName);
                }
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= ModuleAssemblyLoader;
            }
        }

        /// <summary>
        /// For the given assembly path, loads any types that implement IBaristaModule.
        /// </summary>
        /// <param name="assemblyFile"></param>
        private void LoadBaristaModulesFromAssembly(string assemblyFile)
        {
            Assembly asm = null;
            Type[] types = null;

            try
            {
                asm = Assembly.LoadFile(assemblyFile);
                types = asm.GetExportedTypes();
            }
            catch (Exception)
            {
                var msg = $"Unable to load assembly: {Path.GetFileNameWithoutExtension(assemblyFile)}";
                return;
            }

            foreach (var type in types)
            {
                var typeList = type.FindInterfaces(BaristaModuleInterfaceFilter, typeof(IBaristaModule));
                if (typeList.Length > 0)
                {
                    var baristaModule = Activator.CreateInstance(type) as IBaristaModule;
                    m_loadedModules.Add(baristaModule.Name, baristaModule);
                }
            }
        }

        /// <summary>
        /// Callback when assembly load events are fired.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private Assembly ModuleAssemblyLoader(object sender, ResolveEventArgs args)
        {
            // Ignore missing resources
            if (args.Name.Contains(".resources"))
                return null;

            // check for assemblies already loaded
            Assembly assembly = AppDomain.CurrentDomain
                .GetAssemblies()
                .FirstOrDefault(a => a.FullName == args.Name);

            if (assembly != null)
                return assembly;

            // Try to load by filename - split out the filename of the full assembly name
            // and append the base path of the original assembly (ie. look in the same dir)
            var filename = args.Name.Split(',')[0] + ".dll".ToLower();

            var moduleAssemblyPath = Path.Combine(m_moduleFolderFullPath, filename);

            try
            {
                return Assembly.LoadFile(moduleAssemblyPath);
            }
            catch (Exception)
            {
                return null;
            }
        }

        #region IDisposable Support
        private bool m_isDisposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!m_isDisposed)
            {
                if (disposing)
                {
                    foreach (var moduleLoader in m_loadedModules.Values)
                    {
                        if (moduleLoader is IDisposable disposableModuleLoader)
                        {
                            try
                            {
                                disposableModuleLoader.Dispose();
                            }
                            catch
                            {
                                //Do Nothing.
                            }
                        }
                    }
                }

                m_loadedModules.Clear();
                m_isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

        private static bool BaristaModuleInterfaceFilter(Type typeObj, Object criteriaObj)
        {
            if (typeObj.ToString() == criteriaObj.ToString())
                return true;

            return false;
        }

    }
}
