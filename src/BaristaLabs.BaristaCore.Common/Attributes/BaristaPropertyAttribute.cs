namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.Extensions;
    using System;
    using System.Reflection;

    /// <summary>
    /// Instructs the BaristaTypeConversionStrategy to project the member with the specified name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Event, AllowMultiple = false)]
    public sealed class BaristaPropertyAttribute : Attribute
    {
        private readonly string m_name;

        public BaristaPropertyAttribute(string name)
        {
            m_name = name;
            Configurable = true;
            Enumerable = true;
            Writable = true;
        }

        /// <summary>
        /// Gets or sets a value that indicates if the member should be configurable. True if the type of this property descriptor may be changed and if the property may be deleted from the corresponding object. Defaults to true.
        /// </summary>
        public bool Configurable
        {
            get;
            set;
        }

        public string Name
        {
            get { return m_name; }
        }

        /// <summary>
        /// Gets or sets a value that indicates if the member should be enumerable. True if this property should show up during enumeration of the properties on the corresponding object. Defaults to true.
        /// </summary>
        public bool Enumerable
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value that indicates if the member should be writable. True if the value associated with the property may be changed with an assignment operator. Defaults to true.
        /// </summary>
        public bool Writable
        {
            get;
            set;
        }

        public static string GetMemberName(MemberInfo member)
        {
            var attributes = member.GetCustomAttributes(typeof(BaristaPropertyAttribute), false);
            if (attributes.Length == 1 && attributes[0] is BaristaPropertyAttribute attr)
            {
                return attr.Name;
            }

            return member.Name.Camelize();
        }

        public static BaristaPropertyAttribute GetAttribute(MemberInfo member)
        {
            var attributes = member.GetCustomAttributes(typeof(BaristaPropertyAttribute), false);
            if (attributes.Length == 1 && attributes[0] is BaristaPropertyAttribute attr)
            {
                return attr;
            }

            return new BaristaPropertyAttribute(member.Name.Camelize());
        }
    }
}
