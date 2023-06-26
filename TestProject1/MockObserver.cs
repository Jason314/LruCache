namespace LruCacheTests
{
    internal class MockObserver<TKey>
    {
        public TKey? LastKeyEvicted { get; private set; }

        public virtual void OnNext(object sender, TKey key)
        {
            LastKeyEvicted = key;
        }
    }
}
