using System.Threading;
using System.Threading.Tasks;

namespace System.Collections.Concurrent
{
    /// <summary>
    /// 异步回调的延时冲洗集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AsyncCallbackDeferFlushCollection<T> : DeferFlushCollection<T>
    {
        #region Private 字段

        private readonly Func<ReadOnlyMemory<T>, CancellationToken, Task> _flushCallback;

        #endregion Private 字段

        #region Public 构造函数

        /// <inheritdoc cref="AsyncCallbackDeferFlushCollection{T}"/>
        public AsyncCallbackDeferFlushCollection(Func<ReadOnlyMemory<T>, CancellationToken, Task> flushCallback, int flushThreshold, TimeSpan flushInterval) : base(flushThreshold, flushInterval)
        {
            _flushCallback = flushCallback ?? throw new ArgumentNullException(nameof(flushCallback));
        }

        #endregion Public 构造函数

        #region Protected 方法

        /// <inheritdoc/>
        protected override async Task FlushAsync(ReadOnlyMemory<T> items, CancellationToken token) => await _flushCallback(items, token).ConfigureAwait(false);

        #endregion Protected 方法
    }
}