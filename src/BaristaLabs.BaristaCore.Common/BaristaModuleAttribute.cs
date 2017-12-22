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
        }

        /// <summary>
        /// Gets the name of the module.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the description of the module.
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        public static BaristaModuleAttribute GetBaristaModuleAttributeFromType(Type baristaModuleType, bool throwMissingAttributeException = true)
        {
            if (baristaModuleType is IBaristaModule)
                throw new ArgumentException(nameof(baristaModuleType));

            var attributes = baristaModuleType.GetCustomAttributes(typeof(BaristaModuleAttribute), false);
            if (attributes.Length != 1)
                throw new BaristaModuleMissingAttributeException($"Type {baristaModuleType} that implements IBaristaModule is expected to be decorated with an BaristaModuleAttribute.");

            return attributes[0] as BaristaModuleAttribute;
        }
    }
}
