using System.Collections.Generic;

namespace Vision.Core.Common.Collections
{
    public class FastDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public new int Count { get; private set; }

        public FastDictionary() { }

        public FastDictionary(int capacity) : base(capacity) { }

        public new void Add(TKey key, TValue value)
        {
            base.Add(key, value);
            ++Count;
        }

        public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> values)
        {
            foreach (var val in values)
            {
                Add(val.Key, val.Value);
            }
        }

        public new bool Remove(TKey key)
        {
            if (!base.Remove(key)) return false;

            --Count;
            return true;
        }

        public new TValue this[TKey index]
        {
            get => ContainsKey(index) ? base[index] : default(TValue);
            set
            {
                if (!ContainsKey(index))
                    Add(index, value);
                else
                    base[index] = value;
            }
        }

        public new void Clear()
        {
            base.Clear();
            Count = 0;
        }
    }
}
