using System;
using System.Collections.Generic;

namespace Unity.GraphToolkit
{
    // ReSharper disable once InconsistentNaming
    static class IEnumerableExtensions
    {
        public static int IndexOf<T>(this IEnumerable<T> source, T element)
        {
            if (source is IList<T> list)
                return list.IndexOf(element);

            int i = 0;
            foreach (var x in source)
            {
                if (Equals(x, element))
                    return i;
                i++;
            }

            return -1;
        }
    }
}
