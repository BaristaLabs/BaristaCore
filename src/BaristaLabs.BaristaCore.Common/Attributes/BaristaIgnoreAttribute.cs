namespace BaristaLabs.BaristaCore
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Instructs the BaristaTypeConversionStrategy not to project the public field, property or method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Event, AllowMultiple = false)]
    public sealed class BaristaIgnoreAttribute : Attribute
    {
        public BaristaIgnoreAttribute()
        {
        }

        public static bool ShouldIgnore(FieldInfo fieldInfo)
        {
            var attributes = fieldInfo.GetCustomAttributes(typeof(BaristaIgnoreAttribute), false);
            if (attributes.Length == 1)
                return true;

            return false;
        }

        public static bool ShouldIgnore(PropertyInfo propertyInfo)
        {
            var attributes = propertyInfo.GetCustomAttributes(typeof(BaristaIgnoreAttribute), false);
            if (attributes.Length == 1)
                return true;

            return false;
        }

        public static bool ShouldIgnore(MethodInfo methodInfo)
        {
            var attributes = methodInfo.GetCustomAttributes(typeof(BaristaIgnoreAttribute), false);
            if (attributes.Length == 1)
                return true;

            return false;
        }
    }
}
