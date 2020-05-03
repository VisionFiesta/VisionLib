using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Vision.Core.Collections;

namespace Vision.Core
{
    public static class VisionAssembly
    {
        public static string Name = Assembly.GetEntryAssembly()?.GetName().Name;
        public static string Version = Assembly.GetEntryAssembly()?.GetName().Version.ToString(3);

        public static IEnumerable<Assembly> VisionAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(assembly => !assembly.GlobalAssemblyCache);
        public static IEnumerable<Type> VisionTypes = VisionAssemblies.SelectMany(assembly => assembly.GetTypes());


        public static IEnumerable<Pair<TAttribute, MethodInfo>> GetMethodsWithAttribute<TAttribute>() where TAttribute : Attribute
        {
            return VisionTypes
                .SelectMany(type => type.GetMethods())
                .Select(method => new
                {
                    Method = method,
                    Attribute = Attribute.GetCustomAttribute(method, typeof(TAttribute), false) as TAttribute
                }).Where(param1 => param1.Attribute != null).Select(param1 => new Pair<TAttribute, MethodInfo>(param1.Attribute, param1.Method));
        }

        public static IEnumerable<Type> GetTypes<T>() => VisionTypes.Where(a => a == typeof(T));

        public static IEnumerable<Type> GetTypesOfBase<T>() => VisionTypes.Where(a => a.BaseType == typeof(T));
    }
}
