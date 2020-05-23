using System.Security.Cryptography;
using System.Text;

namespace Vision.Core.Utils
{
    public static class Md5Utils
    {
        public static string CalcMd5(string input)
        {
            // Use input string to calculate MD5 hash
            using var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            var sb = new StringBuilder();
            foreach (var b in hashBytes)
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }

        public static string CalcMd5(ref byte[] input)
        {
            using var md5 = MD5.Create();
            var hashBytes = md5.ComputeHash(input);

            // Convert the byte array to hexadecimal string
            var sb = new StringBuilder();
            foreach (var b in hashBytes)
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
