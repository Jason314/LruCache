using LruCacheProj;
using System.Diagnostics;

namespace LruCacheNs
{
    public class LruCache<TKey, TValue> where TKey : notnull
    {
        private readonly Dictionary<TKey, CacheEntry<TKey, TValue>> values = new();
        private readonly LinkedList<TKey> lruList = new(); // First is first to be ejected
        private readonly int capacity;
        private readonly object lockObject = new();

        public delegate void EvictionHandler(object sender, TKey key);
        public event EvictionHandler CacheEvictionHandler;

        public LruCache(int capacity = 10)
        {
            Debug.Assert(capacity > 0);

            this.capacity = capacity;

            CheckInvariants();
        }

        public bool TryAdd(TKey key, TValue value)
        {
            lock(lockObject)
            {
                var lruNode = MakeLastInLru(key);
                if (lruList.Count > capacity) RemoveAnEntry();
                var result = values.TryAdd(key, new CacheEntry<TKey, TValue>(value, lruNode));

                CheckInvariants();
                return result;
            }
        }

        private void RemoveAnEntry()
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var key = lruList.First.Value;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            lruList.RemoveFirst();
            values.Remove(key, out _);
            CacheEvictionHandler?.Invoke(this, key);
        }

        public int Count
        {
            get
            {
                lock (lockObject)
                {
                    return values.Count;
                }
            }
        }

        public bool TryGetValue(TKey key, out TValue? value)
        {
            lock (lockObject)
            {
                var inCache = values.TryGetValue(key, out var cacheEntry);
#pragma warning disable CS8604 // Possible null reference argument.
                value = inCache ? GetValueForEntry(key, cacheEntry) : default;
#pragma warning restore CS8604 // Possible null reference argument.

                CheckInvariants();
                return inCache;
            }
        }

        private TValue? GetValueForEntry(TKey key, CacheEntry<TKey, TValue> cacheEntry)
        {
            MakeLastInLru(key);
            return cacheEntry.Value;
        }

        private LinkedListNode<TKey> MakeLastInLru(TKey key)
        {
            if (!values.TryGetValue(key, out var cacheEntry))
                return lruList.AddLast(key);
            var lruNode = cacheEntry.LruNode;
            lruList.Remove(lruNode);
            lruList.AddLast(lruNode);
            return lruNode;
        }

        private void CheckInvariants()
        {
            Debug.Assert(lruList.Count == values.Count);
            Debug.Assert(lruList.Count <= capacity);
        }
    }
}
