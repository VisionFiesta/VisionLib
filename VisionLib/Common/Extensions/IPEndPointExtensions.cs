using System.Net;

namespace VisionLib.Common.Extensions
{
    public static class IPEndPointExtensions
    {
        public static string ToSimpleString(this IPEndPoint endPoint) => $"{endPoint.Address}:{endPoint.Port}";
    }
}
