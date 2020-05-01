using System.Collections.Generic;
using Vision.Core.Extensions;

namespace Vision.Core.Collections
{
    public static class CollectionConverter
    {
        public static FastList<TSource> ToList<TSource>(IEnumerable<TSource> Source)
        {
            return Source.ToFastList<TSource>();
        }
    }
}
