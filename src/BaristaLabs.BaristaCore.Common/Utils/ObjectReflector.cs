namespace BaristaLabs.BaristaCore.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Utility method to help with obtaining member info of a type.
    /// </summary>
    public sealed class ObjectReflector
    {
        private readonly Type m_type;
        private readonly TypeInfo m_typeInfo;

        public ObjectReflector(Type t)
        {
            m_type = t ?? throw new ArgumentNullException(nameof(t));
            m_typeInfo = m_type.GetTypeInfo();
        }

        public IEnumerable<ConstructorInfo> GetConstructors()
        {
            return m_typeInfo.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
        }

        public IEnumerable<EventInfo> GetEvents(bool instance)
        {
            return m_typeInfo.DeclaredEvents.Where(e => (instance ? !e.AddMethod.IsStatic : e.AddMethod.IsStatic));
        }

        public IEnumerable<MethodInfo> GetMethods(bool instance)
        {
            return m_typeInfo.DeclaredMethods.Where(m => (instance ? !m.IsStatic : m.IsStatic) && !m.Attributes.HasFlag(MethodAttributes.SpecialName));
        }

        public IEnumerable<PropertyInfo> GetProperties(bool instance)
        {
            return m_typeInfo.DeclaredProperties.Where(p => (instance ? !(p.GetMethod?.IsStatic ?? p.SetMethod.IsStatic) : (p.GetMethod?.IsStatic ?? p.SetMethod.IsStatic)));
        }

        public bool HasBaseType
        {
            get
            {
                return m_typeInfo.BaseType != null;
            }
        }

        public Type GetBaseType()
        {
            return m_typeInfo.BaseType;
        }

        public bool IsDelegateType
        {
            get
            {
                return typeof(Delegate).IsAssignableFrom(m_type);
            }
        }
    }
}
