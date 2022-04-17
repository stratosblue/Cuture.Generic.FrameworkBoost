using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace System.Collections.Tests.Concurrent
{
    internal class DefaultDeferFlushCollection : DeferFlushCollection<int>
    {
        #region Public 字段

        public ConcurrentBag<IEnumerable<int>> All = new ConcurrentBag<IEnumerable<int>>();
        public int FlushCount = 0;
        public int ItemCount = 0;

        #endregion Public 字段

        #region Public 构造函数

        public DefaultDeferFlushCollection(int flushThreshold, TimeSpan flushInterval) : base(flushThreshold, flushInterval)
        {
        }

        #endregion Public 构造函数

        #region Protected 方法

        protected override Task FlushAsync(ReadOnlyMemory<int> items, CancellationToken token)
        {
            All.Add(items.Span.ToArray());
            var count = items.Length;

            Interlocked.Add(ref ItemCount, count);
            Interlocked.Increment(ref FlushCount);
            return Task.CompletedTask;
        }

        #endregion Protected 方法
    }
}