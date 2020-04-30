using System.Collections.Concurrent;

namespace Vision.Core.Common.Collections
{
    public class FastConcurrentDictionary<TKey, TValue> : ConcurrentDictionary<TKey, TValue>
    {
        public new int Count { get; private set; }

        public bool Add(TKey key, TValue value)
        {
            if (!TryAdd(key, value)) return false;
            ++Count;
            return true;
        }

        public bool Remove(TKey key, out TValue value)
        {
            if (!TryRemove(key, out value)) return false;
            --Count;
            return true;
        }

        public new TValue this[TKey index]
        {
            get => ContainsKey(index) ? base[index] : default;
            set
            {
                if (!ContainsKey(index))
                    Add(index, value);
                else
                    base[index] = value;
            }
        }
    }
}
