using System.Collections.Generic;
using System.Linq;
using Colorful;

namespace Vision.Core.Extensions
{
    public static class FormatterExtensions
    {
        public static int Length(this Formatter formatter) => formatter.Target.ToString().Length;

        public static int Length(this IEnumerable<Formatter> formatters) => formatters.Select(f => f.Length()).Sum();
    }
}
