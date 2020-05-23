using System.Collections.Generic;
using Vision.Core.Utils;

namespace Vision.Core.IO.SHN
{
    public class SHNHashManager
    {
        private readonly bool _isNA;
        private readonly SortedDictionary<SHNType, string> _shnHashes;

        public SHNHashManager(bool isNA = true)
        {
            _isNA = isNA;
            _shnHashes = new SortedDictionary<SHNType, string>(new SHNHashOrderComparer(isNA));
        }

        protected class SHNHashOrderComparer : IComparer<SHNType>
        {
            private readonly bool _isNA;
            protected internal SHNHashOrderComparer(bool isNA)
            {
                _isNA = isNA;
            }
            public int Compare(SHNType x, SHNType y)
            {
                int xIndex, yIndex;
                if (_isNA)
                {
                    xIndex = SHNTypeExtensions.NAHashOrder.IndexOf(x);
                    yIndex = SHNTypeExtensions.NAHashOrder.IndexOf(y);
                }
                else
                {
                    xIndex = SHNTypeExtensions.EUHashOrder.IndexOf(x);
                    yIndex = SHNTypeExtensions.EUHashOrder.IndexOf(y);
                }

                if (xIndex > yIndex)
                {
                    return -1;
                }
                else if (xIndex < yIndex)
                {
                    return 1;
                }

                return 0;
            }
        }

        public void AddHash(SHNType type, ref byte[] fileBytes)
        {
            if (_isNA && !type.InHash_NA()) return;
            if (!_isNA && !type.InHash_EU()) return;
            if (_shnHashes.ContainsKey(type)) return;
            _shnHashes.Add(type, Md5Utils.CalcMd5(ref fileBytes));
        }


        public bool GetFullHash(out string hash, bool isNA = true)
        {
            hash = null;

            if (_shnHashes.Count != (isNA ? 56 : 50)) return false;

            foreach (var value in _shnHashes.Values)
            {
                hash = hash += value;

            }
            return true;
        }
    }
}
