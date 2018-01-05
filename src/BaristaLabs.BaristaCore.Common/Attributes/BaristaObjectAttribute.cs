namespace BaristaLabs.BaristaCore
{
    using System;

    /// <summary>
    /// Instructs the BaristaTypeConversionStrategy how to project the object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class BaristaObjectAttribute : Attribute
    {
        private readonly string m_name;

        public BaristaObjectAttribute(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            m_name = name;
        }

        /// <summary>
        /// Gets the name of the object.
        /// </summary>
        public string Name
        {
            get { return m_name; }
        }

        public static string GetBaristaObjectNameFromType(Type type)
        {
            var attributes = type.GetCustomAttributes(typeof(BaristaObjectAttribute), false);
            if (attributes.Length == 1 && attributes[0] is BaristaObjectAttribute attr)
            {
                return attr.Name;
            }
            return type.Name;
        }
    }
}
