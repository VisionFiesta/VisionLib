using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VisionLib.Common.Collections;

namespace VisionLib.Common
{
    public static class VisionAssembly
    {
        public static string Name = Assembly.GetEntryAssembly().GetName().Name;
        public static string Version = Assembly.GetEntryAssembly().GetName().Version.ToString(3);

        public static IEnumerable<Pair<TAttribute, MethodInfo>> GetMethodsWithAttribute<TAttribute>() where TAttribute : Attribute
        {
            return ((IEnumerable<Assembly>)AppDomain.CurrentDomain.GetAssemblies())
                .Where(assembly => !assembly.GlobalAssemblyCache)
                .SelectMany(assembly => assembly.GetTypes())
                .SelectMany(type => type.GetMethods())
                .Select(Method => new
                {
                    Method,
                    Attribute = Attribute.GetCustomAttribute((MemberInfo)Method, typeof(TAttribute), false) as TAttribute
                }).Where(_param1 => (object)_param1.Attribute != null).Select(_param1 => new Pair<TAttribute, MethodInfo>(_param1.Attribute, _param1.Method));
        }
    }
}
