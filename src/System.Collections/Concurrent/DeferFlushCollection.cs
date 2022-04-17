using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.Collections.Concurrent
{
    /// <summary>
    /// 延时冲洗集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DeferFlushCollection<T> : IDisposable
    {
        #region Private 字段

        private readonly int _flushThreshold;

        private readonly object _syncRoot = new();

        /// <summary>
        /// 自动冲洗TokenSource
        /// </summary>
        private CancellationTokenSource _autoFlushCTS;

        private bool _disposedValue;

        /// <summary>
        /// 数组索引
        /// </summary>
        private int _index = 0;

        private T[] _items;

        /// <summary>
        /// 最后Flush的时间
        /// </summary>
        private DateTime _lastFlushTime;

        #endregion Private 字段

        #region Public 属性

        /// <summary>
        /// 元素总数
        /// </summary>
        public int Count => _index;

        /// <summary>
        /// 定时触发间隔
        /// </summary>
        public TimeSpan FlushInterval { get; }

        /// <summary>
        /// 触发阈值
        /// </summary>
        public int FlushThreshold => _flushThreshold;

        #endregion Public 属性

        #region Public 构造函数

        /// <summary>
        /// <inheritdoc cref="DeferFlushCollection{T}"/>
        /// </summary>
        /// <param name="flushThreshold">触发阈值</param>
        /// <param name="flushInterval">定时触发间隔</param>
        public DeferFlushCollection(int flushThreshold, TimeSpan flushInterval)
        {
            if (flushThreshold < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(flushThreshold));
            }

            if (flushInterval < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(flushInterval));
            }

            _flushThreshold = flushThreshold;
            _items = new T[flushThreshold];

            FlushInterval = flushInterval;

            _autoFlushCTS = new CancellationTokenSource();

            Task.Run(AutoFlushAsync, _autoFlushCTS.Token);
        }

        #endregion Public 构造函数

        #region Private 析构函数

        /// <summary>
        ///
        /// </summary>
        ~DeferFlushCollection()
        {
            Dispose(disposing: false);
        }

        #endregion Private 析构函数

        #region Public 方法

        /// <summary>
        /// 添加数据到集合
        /// </summary>
        /// <param name="item"></param>
        public void Append(in T item)
        {
            CheckDisposed();

            lock (_syncRoot)
            {
                _items[_index++] = item;
                if (_index < _flushThreshold)
                {
                    return;
                }
                InternalFlush();
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 处理集合中的数据
        /// </summary>
        public void Flush()
        {
            InternalFlush();
        }

        #endregion Public 方法

        #region Protected 方法

        /// <summary>
        /// 释放集合
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                _disposedValue = true;

                try
                {
                    _autoFlushCTS.Cancel();
                }
                catch { }
                finally
                {
                    _autoFlushCTS.Dispose();
                }

                _autoFlushCTS = null!;
            }
        }

        /// <summary>
        /// 处理已追加到集合的所有数据
        /// </summary>
        /// <param name="items">项目列表</param>
        /// <param name="token"></param>
        /// <returns></returns>
        protected abstract Task FlushAsync(ReadOnlyMemory<T> items, CancellationToken token);

        #endregion Protected 方法

        #region Private 方法

        private async Task AutoFlushAsync()
        {
            var token = _autoFlushCTS.Token;
            _lastFlushTime = DateTime.UtcNow;

            while (!token.IsCancellationRequested)
            {
                var interval = DateTime.UtcNow - _lastFlushTime;
                if (interval < FlushInterval)
                {
                    await Task.Delay(FlushInterval - interval, token).ConfigureAwait(false);
                    continue;
                }

                try
                {
                    T[] items;
                    int count;

                    lock (_syncRoot)
                    {
                        items = SwitchBag(out count);
                    }

                    if (count > 0)
                    {
                        await FlushAsync(new Memory<T>(items, 0, count), token).ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException)
                {
                    if (!token.IsCancellationRequested)
                    {
                        throw;
                    }
                }

                await Task.Delay(FlushInterval, token).ConfigureAwait(false);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CheckDisposed()
        {
            if (_disposedValue)
            {
                throw new ObjectDisposedException(nameof(DeferFlushCollection<T>));
            }
        }

        private void InternalFlush()
        {
            T[] items;
            int count;

            lock (_syncRoot)
            {
                items = SwitchBag(out count);
            }

            if (count > 0)
            {
                FlushAsync(new Memory<T>(items, 0, count), _autoFlushCTS.Token);
            }
        }

        /// <summary>
        /// 切换数据容器
        /// <para/>
        /// 确保在 <see cref="_syncRoot"/> lock 块中调用此方法
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private T[] SwitchBag(out int count)
        {
            _lastFlushTime = DateTime.UtcNow;
            if (_index > 0)
            {
                count = _index;
                _index = 0;
                return Interlocked.Exchange(ref _items, new T[_flushThreshold]);
            }
            count = 0;
            return Array.Empty<T>();
        }

        #endregion Private 方法
    }
}