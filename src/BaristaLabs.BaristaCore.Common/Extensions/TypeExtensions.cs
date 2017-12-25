namespace BaristaLabs.BaristaCore.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Numerics;

    public static class TypeExtensions
    {
        private static readonly HashSet<Type> m_numTypes = new HashSet<Type>
            {
                typeof(int),  typeof(double),  typeof(decimal),
                typeof(long), typeof(short),   typeof(sbyte),
                typeof(byte), typeof(ulong),   typeof(ushort),
                typeof(uint), typeof(float),   typeof(BigInteger)
            };

        public static bool IsSameOrSubclass(this Type potentialBase, Type potentialDescendant)
        {
            return potentialDescendant.IsSubclassOf(potentialBase)
                   || potentialDescendant == potentialBase;
        }

        public static object GetDefaultValue(this Type t)
        {
            if (t == null)
                return null;

            if (t.IsValueType)
                return Activator.CreateInstance(t);

            return null;
        }

        public static bool IsNumeric(this Type t)
        {
            if (t == null)
                return false;

            return m_numTypes.Contains(t);
        }
    }
}
