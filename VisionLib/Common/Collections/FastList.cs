using System;
using System.Collections.Generic;
using System.Linq;
using VisionLib.Common.Utils;

namespace VisionLib.Common.Collections
{
    public class FastList<T> : List<T>
    {
        private static readonly object _lock = new object();
        public new volatile int Count;

        public int UpperBound
        {
            get
            {
                return Count - 1;
            }
        }

        public FastList()
        {
        }

        public FastList(IEnumerable<T> Source)
        {
            lock (_lock)
            {
                if (Source == null)
                    return;
                AddRange(Source);
            }
        }

        public int GetUpperBound()
        {
            return Count <= 0 ? -1 : Count - 1;
        }

        public FastList<T> Filter(Func<T, bool> Predicate)
        {
            lock (_lock)
                return new FastList<T>(this.Where<T>(Predicate));
        }

        public T First(Func<T, bool> predicate)
        {
            lock (_lock)
                return Filter(predicate)[0];
        }

        public FastList<TOutput> Filter<TOutput>(Func<T, bool> Predicate)
        {
            lock (_lock)
                return new FastList<TOutput>(this.Where<T>(Predicate).Cast<TOutput>());
        }

        public new T this[int index]
        {
            get
            {
                lock (_lock)
                    return Count < index + 1 ? default(T) : base[index];
            }
            set
            {
                lock (_lock)
                    base[index] = value;
            }
        }

        public new void Add(T value)
        {
            lock (_lock)
            {
                base.Add(value);
                ++Count;
            }
        }

        public void Add(params T[] values)
        {
            lock (_lock)
            {
                foreach (T obj in values)
                    Add(obj);
            }
        }

        public void AddSafe(T value)
        {
            lock (_lock)
            {
                if (Contains(value))
                    return;
                Add(value);
                ++Count;
            }
        }

        public new bool Remove(T value)
        {
            lock (_lock)
            {
                if (!base.Remove(value))
                    return false;
                --Count;
                return true;
            }
        }

        public new void AddRange(IEnumerable<T> collection)
        {
            lock (_lock)
            {
                T[] array = collection.ToArray<T>();
                base.AddRange(array);
                Count += array.Length;
            }
        }

        public new void Clear()
        {
            lock (_lock)
            {
                base.Clear();
                Count = 0;
            }
        }

        public FastList<T> Shuffle()
        {
            int count = Count;
            while (count > 1)
            {
                --count;
                int index = MathUtils.Random(count + 1);
                T obj = this[index];
                this[index] = this[count];
                this[count] = obj;
            }
            return this;
        }

        public FastList<T> Take(int count)
        {
            return new FastList<T>(this.Take<T>(count));
        }

        public void Copy(out FastList<T> destination)
        {
            destination = new FastList<T>(this);
        }
    }
}
