namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.Extensions;
    using System;
    using System.Reflection;

    /// <summary>
    /// Instructs the BaristaTypeConversionStrategy to project the member with the specified name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method, AllowMultiple = false)]
    public sealed class BaristaPropertyAttribute : Attribute
    {
        private readonly string m_name;

        public BaristaPropertyAttribute()
        {
        }

        public BaristaPropertyAttribute(string name)
        {
            m_name = name;
        }

        public string Name
        {
            get { return m_name; }
        }

        public static string GetPropertyName(PropertyInfo property)
        {
            var attributes = property.GetCustomAttributes(typeof(BaristaPropertyAttribute), false);
            if (attributes.Length == 1 && attributes[0] is BaristaPropertyAttribute attr)
            {
                return attr.Name;
            }
            return property.Name.Camelize();
        }

        public static string GetMethodName(MethodInfo method)
        {
            var attributes = method.GetCustomAttributes(typeof(BaristaPropertyAttribute), false);
            if (attributes.Length == 1 && attributes[0] is BaristaPropertyAttribute attr)
            {
                return attr.Name;
            }
            return method.Name.Camelize();
        }
    }
}
