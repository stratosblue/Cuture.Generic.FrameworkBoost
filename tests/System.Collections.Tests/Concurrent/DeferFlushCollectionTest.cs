using System.Linq;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Collections.Tests.Concurrent
{
    [TestClass]
    public class DeferFlushCollectionTest
    {
        #region Public 方法

        [TestMethod]
        public async Task FastWriteTestAsync()
        {
            var flushThreshold = 1000;
            var count = 10_000_000;
            var interval = TimeSpan.FromSeconds(10);

            var deferFlushCollection = new DefaultDeferFlushCollection(flushThreshold, interval);

            Parallel.For(0, count, index =>
            {
                deferFlushCollection.Append(index);
            });

            deferFlushCollection.Flush();

            await Task.Delay(400);

            Console.WriteLine($"Collection Count: {deferFlushCollection.All.Sum(m => m.Count())}");
            Console.WriteLine($"Flush Count: {deferFlushCollection.FlushCount}");

            Assert.AreEqual(count, deferFlushCollection.ItemCount);
        }

        [TestMethod]
        public async Task SlowWriteTestAsync()
        {
            var flushThreshold = 300;
            var count = 1_000;
            var interval = TimeSpan.FromSeconds(1);

            var deferFlushCollection = new DefaultDeferFlushCollection(flushThreshold, interval);

            for (int i = 0; i < count; i++)
            {
                deferFlushCollection.Append(i);
                await Task.Delay(10);
            }

            deferFlushCollection.Flush();

            await Task.Delay(400);

            Console.WriteLine($"Collection Count: {deferFlushCollection.All.Sum(m => m.Count())}");
            Console.WriteLine($"Flush Count: {deferFlushCollection.FlushCount}");

            Assert.IsTrue(deferFlushCollection.FlushCount > count / flushThreshold + 1);
            Assert.AreEqual(count, deferFlushCollection.ItemCount);
        }

        #endregion Public 方法
    }
}