using System;
using System.Collections.Generic;
using VisionLib.Common.Collections;

namespace VisionLib.Common.Extensions
{
    public static class IEnumerableExtensions
    {
        public static FastList<TSource> ToFastList<TSource>(this IEnumerable<TSource> Source)
        {
            if (Source == null)
                throw new ArgumentNullException(nameof(Source));
            return new FastList<TSource>(Source);
        }
    }
}
