using System.Net;

namespace Vision.Core.Common.Extensions
{
    public static class IPEndPointExtensions
    {
        public static string ToSimpleString(this IPEndPoint endPoint) => $"{endPoint.Address}:{endPoint.Port}";
    }
}
