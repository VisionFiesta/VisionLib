using System;
using System.Collections.Generic;
using System.Linq;

namespace Vision.Core.Common.Extensions
{
    public static class EnumExtensions
    {
        public static IEnumerable<T> GetValues<T>() => Enum.GetValues(typeof(T)).Cast<T>();

        public static IEnumerable<TCastType> GetValuesCasted<TEnumType, TCastType>() => Enum.GetValues(typeof(TEnumType)).Cast<TCastType>();

        public static T GetValueOrDefault<T>(string name, T @default) where T : struct, IConvertible
        {
            var enumType = typeof(T);
            if (!enumType.IsEnum)
            {
                throw new Exception("T must be an Enumeration type");
            }

            return Enum.TryParse(name, true, out T val) ? val : @default;
        }

        public static T GetValueOrDefault<T>(byte number, T @default) where T : struct, IConvertible
        {
            var enumType = typeof(T);
            if (!enumType.IsEnum)
            {
                throw new Exception("T must be an Enumeration type");
            }

            if (Enum.IsDefined(enumType, number))
            {
                return (T)Enum.ToObject(enumType, number);
            }

            return @default;
        }

        public static T GetValueOrDefault<T>(int number, T @default) where T : struct, IConvertible
        {
            var enumType = typeof(T);
            if (!enumType.IsEnum)
            {
                throw new Exception("T must be an Enumeration type");
            }

            if (Enum.IsDefined(enumType, number))
            {
                return (T) Enum.ToObject(enumType, number);
            }

            return @default;
        }
    }
}
