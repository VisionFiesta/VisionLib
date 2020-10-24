using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Console = Colorful.Console;

namespace Vision.Core.IO.SHN
{
    public class SHNHashManager
    {
        private readonly SHNManager _shnManager;
        private readonly bool _isNA;
        private readonly SortedDictionary<SHNType, string> _shnHashes;

        public SHNHashManager(SHNManager shnManager, bool isNA)
        {
            _shnManager = shnManager;
            _isNA = isNA;
            _shnHashes = new SortedDictionary<SHNType, string>(new SHNOrderComparer(isNA));
        }

        protected class IntegerOrderComparer : IComparer<int>
        {
            public int Compare(int one, int two)
            {
                return one.CompareTo(two);
            }
        }

        protected class SHNOrderComparer : IComparer<SHNType>
        {
            private readonly ImmutableList<SHNType> _orderList;
            protected internal SHNOrderComparer(bool isNA)
            {
                _orderList = isNA ? SHNTypeExtensions.NAHashOrder : SHNTypeExtensions.EUHashOrder;

            }
            public int Compare(SHNType s1, SHNType s2)
            {
                return _orderList.IndexOf(s1).CompareTo(_orderList.IndexOf(s2));
            }
        }

        public void AddHash(SHNType type, string hash)
        {
            if (_isNA && !type.IsInNAHash()) return;
            if (!_isNA && !type.IsInEUHash()) return;
            if (_shnHashes.ContainsKey(type)) return;
            _shnHashes.Add(type, hash);
        }

        public void LoadRemainingHashes()
        {
            var shnsToHash = _isNA ? SHNTypeExtensions.NAHashOrder : SHNTypeExtensions.EUHashOrder;
            shnsToHash.RemoveAll(s => _shnHashes.ContainsKey(s));

            var shnFiles = _shnManager.LoadSHNFiles(shnsToHash.ToArray());
            foreach (var shnFile in shnFiles)
            {
                AddHash(shnFile.SHNType, shnFile.MD5Hash);
            }
        }

        public bool GetFullHash(out string hash)
        {
            hash = null;

            var requiredCount = _isNA ? SHNTypeExtensions.NAHashOrder.Count : SHNTypeExtensions.EUHashOrder.Count;

            var shnHashesTypes = _shnHashes.Keys;
            if (_shnHashes.Count != requiredCount) return false;

            hash = _shnHashes.Values.Aggregate<string, string>(null, (current, value) => current += value);
            return true;
        }
    }
}
