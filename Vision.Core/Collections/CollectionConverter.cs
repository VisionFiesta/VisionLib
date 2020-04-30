using System.Collections.Generic;
using Vision.Core.Common.Extensions;

namespace Vision.Core.Common.Collections
{
    public static class CollectionConverter
    {
        public static FastList<TSource> ToList<TSource>(IEnumerable<TSource> Source)
        {
            return Source.ToFastList<TSource>();
        }
    }
}
