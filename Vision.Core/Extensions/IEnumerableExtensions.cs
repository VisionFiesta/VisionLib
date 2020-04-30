using System;
using System.Collections.Generic;
using Vision.Core.Common.Collections;

namespace Vision.Core.Common.Extensions
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
