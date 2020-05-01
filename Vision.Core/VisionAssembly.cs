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

        public static IEnumerable<Pair<TAttribute, MethodInfo>> GetMethodsWithAttribute<TAttribute>() where TAttribute : Attribute
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => !assembly.GlobalAssemblyCache)
                .SelectMany(assembly => assembly.GetTypes())
                .SelectMany(type => type.GetMethods())
                .Select(method => new
                {
                    Method = method,
                    Attribute = Attribute.GetCustomAttribute(method, typeof(TAttribute), false) as TAttribute
                }).Where(param1 => param1.Attribute != null).Select(param1 => new Pair<TAttribute, MethodInfo>(param1.Attribute, param1.Method));
        }

        public static IEnumerable<TypeInfo> GetTypesOfBase<T>()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => !assembly.GlobalAssemblyCache)
                .SelectMany(assembly => assembly.GetTypes())
                .Where(a => a.BaseType == typeof(T))
                .Select(t => t.GetTypeInfo());
        }
    }
}
