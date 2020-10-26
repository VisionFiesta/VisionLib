using System;
using System.Collections.Generic;
using System.Linq;

namespace Vision.Core.Extensions
{
    public static class SetExtensions
    {
        public static ISet<T> Filter<T>(this ISet<T> set, Func<T, bool> predicate) => set.Where(predicate).ToHashSet();
    }
}
