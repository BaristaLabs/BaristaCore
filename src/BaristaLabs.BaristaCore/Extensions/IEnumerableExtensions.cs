namespace BaristaLabs.BaristaCore.Extensions
{
    using System.Collections.Generic;

    internal static class InternalExtensions
    {
        public static IEnumerable<T> PrependWith<T>(this IEnumerable<T> sequence, params T[] itemsToPrepend)
        {
            foreach (var item in itemsToPrepend)
                yield return item;

            foreach (var item in sequence)
                yield return item;
        }

        public static IEnumerable<T> PrependWith<T>(this IEnumerable<T> sequence, T itemToPrepend)
        {
            yield return itemToPrepend;

            foreach (var item in sequence)
                yield return item;
        }
    }
}
