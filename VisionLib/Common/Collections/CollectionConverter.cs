using System.Collections.Generic;
using VisionLib.Common.Extensions;

namespace VisionLib.Common.Collections
{
    public static class CollectionConverter
    {
        public static FastList<TSource> ToList<TSource>(IEnumerable<TSource> Source)
        {
            return Source.ToFastList<TSource>();
        }
    }
}
