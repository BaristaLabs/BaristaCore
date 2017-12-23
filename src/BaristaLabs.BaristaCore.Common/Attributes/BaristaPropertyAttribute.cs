namespace BaristaLabs.BaristaCore.Attributes
{
    using System;

    /// <summary>
    /// Instructs the BaristaTypeConversionStrategy to project the member with the specified name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class BaristaPropertyAttribute : Attribute
    {
        public BaristaPropertyAttribute()
        {
        }

        public BaristaPropertyAttribute(string propertyName)
        {
            PropertyName = PropertyName;
        }

        public string PropertyName
        {
            get;
            set;
        }
    }
}
