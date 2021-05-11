using System.Collections.Generic;
using System.Linq;

using Vision.Core.Collections;
using Vision.Core.Extensions;

namespace Vision.Core.IO.SHN
{
    public class SHNHashHelper
    {
        private const int HashChunkLength = 32;

        public static List<string> SeparateBigHash(string bigHash)
        {
            if (bigHash.Length % HashChunkLength == 0)
            {
                return Enumerable.Range(0, bigHash.Length / HashChunkLength)
                    .Select(i => bigHash.Substring(i * HashChunkLength, HashChunkLength)).ToList();
            }

            return Enumerable.Empty<string>().ToList();
        }

        public static Dictionary<SHNType, string> GetHashedSHNs(string bigHash, SHNManager shnManager)
        {
            var bigHashSplit = SeparateBigHash(bigHash);
            var hashedSHNs = new FastDictionary<SHNType, string>();
            var matchedSHNHashes = new FastDictionary<SHNType, string>();

            // foreach (var shnType in SHNTypeExtensions.NAHashOrder)
            foreach (var shnType in SHNTypeExtensions.NAClientSHNs)
            {
                if (shnManager.Load(shnType, out var shnFile))
                {
                    hashedSHNs.Add(shnType, shnFile.MD5Hash);
                }
            }

            var rawMatches = bigHashSplit.Count(hash => hashedSHNs.ContainsValue(hash));

            foreach (var (shnType, hash) in bigHashSplit.Where(shnHash => hashedSHNs.ContainsValue(shnHash)).Select(hash => hashedSHNs.First(w => w.Value.Equals(hash))))
            {
                matchedSHNHashes.Add(shnType, hash);
            }

            return matchedSHNHashes;
        }
    }
}
