using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Extensions.Tests.Caching.Memory.LRUMemoryCache
{
    [TestClass]
    public class LRUMemoryCacheTest
    {
        #region Public 方法

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(1)]
        public void Invalid_Capacity(int capacity)
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => BoundedMemoryCache.CreateLRU<string, TestCacheItem>(capacity));
        }

        [TestMethod]
        [DataRow(2)]
        [DataRow(1000)]
        [DataRow(100_000)]
        public void Parallel_Add(int capacity)
        {
            var memoryCache = BoundedMemoryCache.CreateLRU<string, TestCacheItem>(capacity);
            var keys = new ConcurrentBag<string>();

            FillCache(capacity * 4, memoryCache, keys);

            Assert.AreEqual(capacity, GetCacheCount(memoryCache, keys));
        }

        [TestMethod]
        [DataRow(2)]
        [DataRow(1000)]
        [DataRow(100_000)]
        public void Parallel_Remove(int capacity)
        {
            var memoryCache = BoundedMemoryCache.CreateLRU<string, TestCacheItem>(capacity);
            var keys = new ConcurrentBag<string>();

            FillCache(capacity, memoryCache, keys);

            var partitionSize = (capacity / 4) > 0 ? capacity / 4 : 1;
            var count = capacity;
            var allKeys = new ConcurrentBag<string>(keys.ToArray());

            while (keys.Count > 0)
            {
                Parallel.For(0, partitionSize, _ =>
                {
                    if (keys.TryTake(out var key))
                    {
                        memoryCache.Remove(key);
                    }
                    //移除不存在的
                    memoryCache.Remove(Guid.NewGuid().ToString());
                });
                count -= partitionSize;
                Assert.AreEqual(count > 0 ? count : 0, GetCacheCount(memoryCache, allKeys));
            }

            Assert.AreEqual(0, keys.Count);
            Assert.AreEqual(0, GetCacheCount(memoryCache, allKeys));

            //移除完后再添加
            count = 0;

            while (count < capacity)
            {
                FillCache(partitionSize, memoryCache, allKeys);
                count += partitionSize;
                if (count > capacity)
                {
                    count = capacity;
                }
                Assert.AreEqual(count, GetCacheCount(memoryCache, allKeys));
            }

            Assert.AreEqual(capacity, GetCacheCount(memoryCache, allKeys));

            //在移除时添加
            for (int i = 0; i < 5; i++)
            {
                Parallel.For(0, partitionSize, _ =>
                {
                    if (keys.TryTake(out var key))
                    {
                        memoryCache.Remove(key);
                    }
                    key = Guid.NewGuid().ToString();
                    memoryCache.Add(key, new());
                    allKeys.Add(key);
                });

                Assert.AreEqual(capacity, GetCacheCount(memoryCache, allKeys));
            }
        }

        [TestMethod]
        [DataRow(2)]
        [DataRow(1000)]
        [DataRow(100_000)]
        public void Parallel_Repeat_Add(int capacity)
        {
            var memoryCache = BoundedMemoryCache.CreateLRU<string, TestCacheItem>(capacity);
            var keys = new ConcurrentBag<string>();
            var keepCaches = new ConcurrentBag<(string Key, TestCacheItem Value)>();

            Enumerable.Range(0, capacity).Select(m => (Guid.NewGuid().ToString(), new TestCacheItem())).ToList().ForEach(m => { keepCaches.Add(m); memoryCache.Add(m.Item1, m.Item2); });

            var keepKeys = keepCaches.Select(m => m.Key).ToArray();

            for (int i = 0; i < 4; i++)
            {
                var addCount = capacity / 2;
                var remainCount = capacity - addCount;

                FillCache(addCount, memoryCache, keys);

                Assert.AreEqual(addCount, GetCacheCount(memoryCache, keys));

                Assert.AreEqual(remainCount, GetCacheCount(memoryCache, keepKeys));

                keepCaches.AsParallel().ForAll(m =>
                {
                    memoryCache.Add(m.Key, m.Value);
                });

                Assert.AreEqual(0, GetCacheCount(memoryCache, keys));

                keys.Clear();

                Assert.AreEqual(capacity, GetCacheCount(memoryCache, keepKeys));
            }
        }

        [TestMethod]
        [DataRow(2)]
        [DataRow(1000)]
        [DataRow(100_000)]
        public void RemovingCallback(int capacity)
        {
            var memoryCache = BoundedMemoryCache.CreateLRU<string, TestCacheItem>(capacity);

            var keys = new ConcurrentBag<string>();
            var autoReAddKeys = new ConcurrentBag<string>();

            void AutoReAdd(string key, TestCacheItem value)
            {
                memoryCache.Add(key, value, AutoReAdd);
            }

            Parallel.For(0, capacity, _ =>
            {
                var key = Guid.NewGuid().ToString();
                memoryCache.Add(key, new(), AutoReAdd);
                autoReAddKeys.Add(key);
            });

            FillCache(capacity, memoryCache, keys);

            //回调可能未运行完
            Thread.Sleep(1_000);

            Assert.AreEqual(0, GetCacheCount(memoryCache, keys));
            Assert.AreEqual(capacity, GetCacheCount(memoryCache, autoReAddKeys));
        }

        #endregion Public 方法

        #region Private 方法

        private static void FillCache(int count, IBoundedMemoryCache<string, TestCacheItem> memoryCache, ConcurrentBag<string> keys)
        {
            Parallel.For(0, count, _ =>
            {
                var key = Guid.NewGuid().ToString();
                memoryCache.Add(key, new());
                keys.Add(key);
            });
        }

        private static int GetCacheCount(IBoundedMemoryCache<string, TestCacheItem> memoryCache, IEnumerable<string> allKeys)
        {
            return allKeys.Count(m => memoryCache.TryGet(m, out var item) && item is not null);
        }

        #endregion Private 方法
    }
}