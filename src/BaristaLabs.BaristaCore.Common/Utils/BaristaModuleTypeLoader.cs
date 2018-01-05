namespace BaristaLabs.BaristaCore
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    public static class BaristaModuleTypeLoader
    {
        /// <summary>
        /// For the given assembly path, returns any types that implement IBaristaModule.
        /// </summary>
        /// <param name="path"></param>
        public static IEnumerable<Type> LoadBaristaModulesFromAssembly(string path)
        {
            if (!File.Exists(path))
                throw new ArgumentException($"A file does not exist at the specified path {path}");

            try
            {
                var assembly = Assembly.LoadFile(path);
                var types = assembly.GetExportedTypes();
                return LoadBaristaModulesFromAssembly(assembly);
            }
            catch (Exception)
            {
                return Enumerable.Empty<Type>();
            }
        }

        /// <summary>
        /// For the given assembly, returns any types that implement IBaristaModule
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static IEnumerable<Type> LoadBaristaModulesFromAssembly(Assembly assembly)
        {
            foreach (var type in assembly.GetExportedTypes())
            {
                var typeList = type.FindInterfaces(BaristaModuleInterfaceFilter, typeof(IBaristaModule));
                if (!type.IsAbstract && !type.IsInterface && !type.IsGenericType && !type.IsNotPublic && typeList.Length > 0)
                {
                    yield return type;
                }
            }
        }

        private static bool BaristaModuleInterfaceFilter(Type typeObj, Object criteriaObj)
        {
            if (typeObj.ToString() == criteriaObj.ToString())
                return true;

            return false;
        }
    }
}
