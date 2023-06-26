using LruCacheNs;

namespace LruCacheTests
{
    [TestClass]
    public class LruCacheTest
    {
        [TestMethod]
        public void TestEmptyCache()
        {
            var cache = new LruCache<string, int>();

            Assert.AreEqual(0, cache.Count);
            Assert.IsFalse(cache.TryGetValue("missing key", out var miss));
            Assert.AreEqual(0, miss);
        }

        [TestMethod]
        public void TestSingleEntry()
        {
            var cache = new LruCache<int, string>();

            var addResult = cache.TryAdd(19, "nineteen");

            Assert.IsTrue(addResult);
            Assert.AreEqual(1, cache.Count);
            Assert.IsTrue(cache.TryGetValue(19, out var value));
            Assert.AreEqual("nineteen", value);
            Assert.IsFalse(cache.TryGetValue(22, out var miss));
            Assert.IsNull(miss);
        }

        [TestMethod]
        public void TestSingleEntrySetTwice()
        {
            var cache = new LruCache<int, int>();

            var addResult1 = cache.TryAdd(34, 34);
            var addResult2 = cache.TryAdd(34, 35);

            Assert.IsTrue(addResult1);
            Assert.IsFalse(addResult2);
            Assert.AreEqual(1, cache.Count);
            Assert.IsTrue(cache.TryGetValue(34, out var value));
            Assert.AreEqual(34, value);
            Assert.IsFalse(cache.TryGetValue(42, out var miss));
            Assert.AreEqual(0, miss);
        }

        [TestMethod]
        public void TestTwoEntries()
        {
            var cache = new LruCache<string, string>();

            var addResult1 = cache.TryAdd("key one", "value one");
            var addResult2 = cache.TryAdd("key two", "value two");

            Assert.IsTrue(addResult1);
            Assert.IsTrue(addResult2);
            Assert.AreEqual(2, cache.Count);
            Assert.IsTrue(cache.TryGetValue("key one", out var value1));
            Assert.AreEqual("value one", value1);
            Assert.IsTrue(cache.TryGetValue("key two", out var value2));
            Assert.AreEqual("value two", value2);
            Assert.IsFalse(cache.TryGetValue("unknown key", out var miss));
            Assert.IsNull(miss);
        }

        [TestMethod]
        public void TestExpulsionWithTwoEntries()
        {
            var cache = new LruCache<string, int>(1);

            var addResult1 = cache.TryAdd("key one", 74);
            var addResult2 = cache.TryAdd("key two", 75);

            Assert.IsTrue(addResult1);
            Assert.IsTrue(addResult2);
            Assert.AreEqual(1, cache.Count);
            Assert.IsFalse(cache.TryGetValue("key one", out var value1));
            Assert.AreEqual(0, value1);
            Assert.IsTrue(cache.TryGetValue("key two", out var value2));
            Assert.AreEqual(75, value2);
        }

        [TestMethod]
        public void TestExpulsionWithThreeEntriesAndCapacity2()
        {
            var cache = new LruCache<string, int>(2);

            var addResult1 = cache.TryAdd("key one", 1);
            var addResult2 = cache.TryAdd("key two", 2);
            var addResult3 = cache.TryAdd("key three", 3);

            Assert.IsTrue(addResult1);
            Assert.IsTrue(addResult2);
            Assert.IsTrue(addResult3);
            Assert.AreEqual(2, cache.Count);
            Assert.IsFalse(cache.TryGetValue("key one", out var value1));
            Assert.AreEqual(0, value1);
            Assert.IsTrue(cache.TryGetValue("key two", out var value2));
            Assert.AreEqual(2, value2);
            Assert.IsTrue(cache.TryGetValue("key three", out var value3));
            Assert.AreEqual(3, value3);
        }

        [TestMethod]
        public void TestTwoExpulsions()
        {
            var cache = new LruCache<string, int>(1);

            var addResult1 = cache.TryAdd("key one", 1);
            var addResult2 = cache.TryAdd("key two", 2);
            var addResult3 = cache.TryAdd("key three", 3);

            Assert.IsTrue(addResult1);
            Assert.IsTrue(addResult2);
            Assert.IsTrue(addResult3);
            Assert.AreEqual(1, cache.Count);
            Assert.IsFalse(cache.TryGetValue("key one", out var value1));
            Assert.AreEqual(0, value1);
            Assert.IsFalse(cache.TryGetValue("key two", out var value2));
            Assert.AreEqual(0, value2);
            Assert.IsTrue(cache.TryGetValue("key three", out var value3));
            Assert.AreEqual(3, value3);
        }

        [TestMethod]
        public void TestAddingKeyTwiceDoesNotExpel()
        {
            var cache = new LruCache<string, int>(2);

            var addResult1 = cache.TryAdd("key one", 1);
            var addResult2 = cache.TryAdd("key two", 2);
            var addResult3 = cache.TryAdd("key two", 3);

            Assert.IsTrue(addResult1);
            Assert.IsTrue(addResult2);
            Assert.IsFalse(addResult3);
            Assert.AreEqual(2, cache.Count);
            Assert.IsTrue(cache.TryGetValue("key one", out var value1));
            Assert.AreEqual(1, value1);
            Assert.IsTrue(cache.TryGetValue("key two", out var value2));
            Assert.AreEqual(2, value2);
        }

        [TestMethod]
        public void TestAddingKeyPutsToFrontOfLru()
        {
            var cache = new LruCache<string, int>(2);

            var addResult1 = cache.TryAdd("key one", 1);
            var addResult2 = cache.TryAdd("key two", 2);
            var addResult3 = cache.TryAdd("key one", 154);
            var addResult4 = cache.TryAdd("key three", 3);

            Assert.IsTrue(addResult1);
            Assert.IsTrue(addResult2);
            Assert.IsFalse(addResult3);
            Assert.IsTrue(addResult4);
            Assert.AreEqual(2, cache.Count);
            Assert.IsTrue(cache.TryGetValue("key one", out var value1));
            Assert.AreEqual(1, value1);
            Assert.IsFalse(cache.TryGetValue("key two", out var value2));
            Assert.AreEqual(0, value2);
            Assert.IsTrue(cache.TryGetValue("key three", out var value3));
            Assert.AreEqual(3, value3);
        }

        [TestMethod]
        public void TestGettingKeyPutsToFrontOfLru()
        {
            var cache = new LruCache<string, int>(2);

            var addResult1 = cache.TryAdd("key one", 1);
            var addResult2 = cache.TryAdd("key two", 2);
            cache.TryGetValue("key one", out _);
            var addResult3 = cache.TryAdd("key three", 3);

            Assert.IsTrue(addResult1);
            Assert.IsTrue(addResult2);
            Assert.IsTrue(addResult3);
            Assert.AreEqual(2, cache.Count);
            Assert.IsTrue(cache.TryGetValue("key one", out var value1));
            Assert.AreEqual(1, value1);
            Assert.IsFalse(cache.TryGetValue("key two", out var value2));
            Assert.AreEqual(0, value2);
            Assert.IsTrue(cache.TryGetValue("key three", out var value3));
            Assert.AreEqual(3, value3);
        }

        [TestMethod]
        public void TestGettingNonExistentKeyDoesNotAffectLru()
        {
            var cache = new LruCache<string, int>(2);

            var addResult1 = cache.TryAdd("key one", 1);
            var addResult2 = cache.TryAdd("key two", 2);
            cache.TryGetValue("key three", out _);
            var addResult3 = cache.TryAdd("key three", 3);

            Assert.IsTrue(addResult1);
            Assert.IsTrue(addResult2);
            Assert.IsTrue(addResult3);
            Assert.AreEqual(2, cache.Count);
            Assert.IsFalse(cache.TryGetValue("key one", out var value1));
            Assert.AreEqual(0, value1);
            Assert.IsTrue(cache.TryGetValue("key two", out var value2));
            Assert.AreEqual(2, value2);
            Assert.IsTrue(cache.TryGetValue("key three", out var value3));
            Assert.AreEqual(3, value3);
        }

        [TestMethod]
        public void TestEvictionObserver()
        {
            var cache = new LruCache<string, int>(1);
            var evictionObserver = new MockObserver<string>();
            cache.CacheEvictionHandler += evictionObserver.OnNext;

            cache.TryAdd("key one", 1);
            Assert.IsNull(evictionObserver.LastKeyEvicted);
            cache.TryAdd("key two", 2);
            Assert.AreEqual("key one", evictionObserver.LastKeyEvicted);
            cache.TryAdd("key three", 3);
            Assert.AreEqual("key two", evictionObserver.LastKeyEvicted);
        }
    }
}
