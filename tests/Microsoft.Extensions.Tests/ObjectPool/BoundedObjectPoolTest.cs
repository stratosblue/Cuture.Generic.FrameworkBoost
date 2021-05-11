using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.ObjectPool;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Extensions.Tests.ObjectPool
{
    [TestClass]
    public class BoundedObjectPoolTest
    {
        #region Private 字段

        private IBoundedObjectPool<BoundedObjectPoolTestClass> _boundedObjectPool;

        #endregion Private 字段

        #region Public 方法

        [TestMethod]
        public async Task AutomaticReductionPoolSize()
        {
            var maximumPooled = 10000;
            var minimumRetained = 3000;
            var recycleIntervalSeconds = 1;
            InitPool(maximumPooled, minimumRetained, recycleIntervalSeconds);

            HashSet<Guid> objHash = new();

            var owners = Enumerable.Range(0, maximumPooled).AsParallel().Select(m => _boundedObjectPool.Rent()).ToList();
            int lastPoolSize = _boundedObjectPool.PoolSize;
            Assert.AreEqual(maximumPooled, lastPoolSize);

            owners.ForEach(m => objHash.Add(m.Item.Guid));
            Assert.AreEqual(maximumPooled, objHash.Count);

            owners.ForEach(m => m.Dispose());

            for (int i = 1; i < 6; i++)
            {
                for (int tryCheck = 0; tryCheck <= 25; tryCheck++)
                {
                    await Task.Delay(TimeSpan.FromSeconds(recycleIntervalSeconds * 0.1));
                    if (_boundedObjectPool.PoolSize < lastPoolSize)
                    {
                        Console.WriteLine($"tryCheckTime: {tryCheck} - PoolSize: {_boundedObjectPool.PoolSize} - lastPoolSize: {lastPoolSize}");
                        break;
                    }
                    if (tryCheck == 25)
                    {
                        Assert.Fail("在指定时间内没有正确进行回收");
                    }
                }

                Console.WriteLine($"CheckTime: {i} - PoolSize: {_boundedObjectPool.PoolSize} - lastPoolSize: {lastPoolSize}");

                if (_boundedObjectPool.PoolSize == minimumRetained)
                {
                    Console.WriteLine("自动回收到了最小保留值");
                    break;
                }
                Assert.IsTrue(_boundedObjectPool.PoolSize < lastPoolSize);
                lastPoolSize = _boundedObjectPool.PoolSize;

                owners = Enumerable.Range(0, minimumRetained).AsParallel().Select(m => _boundedObjectPool.Rent()).ToList();
                owners.ForEach(m => objHash.Add(m.Item.Guid));
                Assert.AreEqual(maximumPooled, objHash.Count);

                owners.ForEach(m => m.Dispose());
            }

            await Task.Delay(TimeSpan.FromSeconds(recycleIntervalSeconds * 1.2));
            Assert.AreEqual(minimumRetained, _boundedObjectPool.PoolSize);

            owners = Enumerable.Range(0, minimumRetained + 200).AsParallel().Select(m => _boundedObjectPool.Rent()).ToList();
            owners.ForEach(m => objHash.Add(m.Item.Guid));
            Assert.AreEqual(maximumPooled + 200, objHash.Count);
        }

        [TestMethod]
        public void GeneralUsage()
        {
            var maximumPooled = 10000;
            InitPool(maximumPooled, 2000);

            HashSet<Guid> objHash = new();

            var owners = Enumerable.Range(0, maximumPooled).AsParallel().Select(m => _boundedObjectPool.Rent()).ToList();
            Assert.IsFalse(owners.Any(m => m?.Item is null));

            owners.ForEach(m => objHash.Add(m.Item.Guid));
            Assert.AreEqual(maximumPooled, objHash.Count);

            Assert.AreEqual(maximumPooled, _boundedObjectPool.PoolSize);
            Assert.AreEqual(0, _boundedObjectPool.AvailableCount);

            var otherOwners = Enumerable.Range(0, maximumPooled).AsParallel().Select(m => _boundedObjectPool.Rent()).ToList();
            Assert.IsFalse(otherOwners.Any(m => m is not null));
            Assert.AreEqual(maximumPooled, _boundedObjectPool.PoolSize);
            Assert.AreEqual(0, _boundedObjectPool.AvailableCount);

            owners.ForEach(m => m.Dispose());
            Assert.AreEqual(maximumPooled, _boundedObjectPool.AvailableCount);

            owners = Enumerable.Range(0, maximumPooled).AsParallel().Select(m => _boundedObjectPool.Rent()).ToList();
            Assert.IsFalse(owners.Any(m => m?.Item is null));

            owners.ForEach(m => objHash.Add(m.Item.Guid));
            Assert.AreEqual(maximumPooled, objHash.Count);

            Assert.AreEqual(maximumPooled, _boundedObjectPool.PoolSize);
            Assert.AreEqual(0, _boundedObjectPool.AvailableCount);
        }

        [TestMethod]
        public void NotInvokeDispose()
        {
            var maximumPooled = 10000;
            InitPool(maximumPooled, 2000);

            HashSet<Guid> objHash = new();

            var owners = Enumerable.Range(0, maximumPooled).AsParallel().Select(m => _boundedObjectPool.Rent()).ToList();
            Assert.IsFalse(owners.Any(m => m?.Item is null));

            owners.ForEach(m => objHash.Add(m.Item.Guid));
            Assert.AreEqual(maximumPooled, objHash.Count);

            Assert.AreEqual(maximumPooled, _boundedObjectPool.PoolSize);
            Assert.AreEqual(0, _boundedObjectPool.AvailableCount);

            var otherOwners = Enumerable.Range(0, maximumPooled).AsParallel().Select(m => _boundedObjectPool.Rent()).ToList();
            Assert.IsFalse(otherOwners.Any(m => m is not null));
            Assert.AreEqual(maximumPooled, _boundedObjectPool.PoolSize);
            Assert.AreEqual(0, _boundedObjectPool.AvailableCount);

            owners.Clear();
            otherOwners.Clear();

            GC.Collect();
            GC.WaitForFullGCComplete();
            GC.WaitForPendingFinalizers();

            Assert.AreEqual(maximumPooled, _boundedObjectPool.PoolSize);
            Assert.AreEqual(maximumPooled, _boundedObjectPool.AvailableCount);

            owners = Enumerable.Range(0, maximumPooled).AsParallel().Select(m => _boundedObjectPool.Rent()).ToList();
            Assert.IsFalse(owners.Any(m => m?.Item is null));

            owners.ForEach(m => objHash.Add(m.Item.Guid));
            Assert.AreEqual(maximumPooled, objHash.Count);

            Assert.AreEqual(maximumPooled, _boundedObjectPool.PoolSize);
            Assert.AreEqual(0, _boundedObjectPool.AvailableCount);
        }

        [TestMethod]
        public async Task SuppressAutomaticRecycle()
        {
            InitPool(100, 0, 1);

            _boundedObjectPool.Rent().Dispose();
            var lastPoolSize = _boundedObjectPool.PoolSize;

            for (int i = 1; i < 15; i++)
            {
                Console.WriteLine($"CheckTime: {i} - PoolSize: {_boundedObjectPool.PoolSize} - lastPoolSize: {lastPoolSize}");

                Assert.IsTrue(lastPoolSize > 0);
                Assert.IsTrue(_boundedObjectPool.PoolSize >= lastPoolSize);
                lastPoolSize = _boundedObjectPool.PoolSize;

                Enumerable.Range(0, i).Select(m => _boundedObjectPool.Rent()).ToList().ForEach(m => m.Dispose());

                await Task.Delay(TimeSpan.FromSeconds(0.5));
            }

            await Task.Delay(TimeSpan.FromSeconds(1.5));
            Console.WriteLine($"PoolSize at end: {_boundedObjectPool.PoolSize}");
            Assert.IsTrue(_boundedObjectPool.PoolSize < lastPoolSize);
        }

        #region init

        [TestCleanup]
        public void TestCleanup()
        {
            _boundedObjectPool.Dispose();
        }

        #endregion init

        #endregion Public 方法

        #region Private 方法

        private void InitPool(int maximumPooled, int minimumRetained, int recycleIntervalSeconds = 60)
        {
            _boundedObjectPool = BoundedObjectPool.Create<BoundedObjectPoolTestClass>(maximumPooled, minimumRetained, recycleIntervalSeconds);
        }

        #endregion Private 方法
    }

    internal class BoundedObjectPoolTestClass
    {
        #region Public 属性

        public Guid Guid { get; } = Guid.NewGuid();

        #endregion Public 属性
    }
}