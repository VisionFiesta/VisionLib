using System;
using System.Collections.Generic;
using System.Linq;

namespace VisionLib.Common.Extensions
{
    public static class EnumExtensions
    {
        public static IEnumerable<T> GetValues<T>() => Enum.GetValues(typeof(T)).Cast<T>();

        public static IEnumerable<TCastType> GetValuesCasted<TEnumType, TCastType>() => Enum.GetValues(typeof(TEnumType)).Cast<TCastType>();
    }
}
