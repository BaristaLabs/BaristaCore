namespace BaristaLabs.BaristaCore.Extensions
{
    using System;

    public static class TypeExtensions
    {
        public static bool IsSameOrSubclass(this Type potentialBase, Type potentialDescendant)
        {
            return potentialDescendant.IsSubclassOf(potentialBase)
                   || potentialDescendant == potentialBase;
        }

        public static object GetDefaultValue(this Type t)
        {
            if (t.IsValueType)
                return Activator.CreateInstance(t);

            return null;
        }
    }
}
