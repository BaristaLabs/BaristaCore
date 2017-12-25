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
    }
}
