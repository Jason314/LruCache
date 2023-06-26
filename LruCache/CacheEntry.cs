namespace LruCacheProj
{
    internal class CacheEntry<TKey, TValue>
    {
        public CacheEntry(TValue value, LinkedListNode<TKey> lruNode)
        {
            Value = value;
            LruNode = lruNode;
        }

        public TValue Value { get; }
        public LinkedListNode<TKey> LruNode { get; }
    }
}
