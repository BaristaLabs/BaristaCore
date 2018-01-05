namespace BaristaLabs.BaristaCore
{
    using System;

    /// <summary>
    /// Attribute that is required to be placed on BaristaModules which provides module metadata.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class BaristaModuleAttribute : Attribute
    {
        public BaristaModuleAttribute(string name)
            : this(name, null)
        {
        }

        public BaristaModuleAttribute(string name, string description)
        {
            Name = name;
            Description = description;
            IsDiscoverable = true;
        }

        /// <summary>
        /// Gets or sets the name of the module.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the version of the module.
        /// </summary>
        public string Version
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the description of the module.
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value that indicates if the module should be able to be discovered by module loaders that use reflection to dynamically discover modules in assemblies. Defaults to true.
        /// </summary>
        /// <remarks>
        /// Modules that require configuration -- such as those that do not contain a default constructor or supply values at runtime (such as BaristaScriptModule) should be marked as not discoverable by setting this value to false.
        /// </remarks>
        public bool IsDiscoverable
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the BaristaModuleAttribute that decorates the specified type.
        /// </summary>
        /// <param name="baristaModuleType"></param>
        /// <param name="throwMissingAttributeException"></param>
        /// <returns></returns>
        public static BaristaModuleAttribute GetBaristaModuleAttributeFromType(Type baristaModuleType, bool throwMissingAttributeException = true)
        {
            if (baristaModuleType is IBaristaModule)
                throw new ArgumentException(nameof(baristaModuleType));

            var attributes = baristaModuleType.GetCustomAttributes(typeof(BaristaModuleAttribute), false);
            if (attributes.Length != 1 && throwMissingAttributeException)
                throw new BaristaModuleMissingAttributeException($"Type {baristaModuleType} that implements IBaristaModule is expected to be decorated with an BaristaModuleAttribute.");

            return attributes[0] as BaristaModuleAttribute;
        }
    }
}
