using System.Collections.Concurrent;
using System.Diagnostics;

namespace System.Threading.Tasks;

/// <summary>
/// 独占线程任务调度器
/// <para/>
/// 内部将会独占一个线程
/// <para/>
/// 通过该调度器执行的所有操作都在此独占线程上执行
/// </summary>
public class ExclusiveThreadTaskScheduler : TaskScheduler, IDisposable
{
    #region Private 字段

    private readonly Queue<Func<Task?>> _actionQueue = new(Environment.ProcessorCount * 2);
    private readonly AutoResetEvent _actionQueueWaitEvent = new(false);
    private readonly CancellationTokenSource _runningCancellationTokenSource;
    private readonly CancellationToken _runningToken;
    private readonly Thread _thread;
    private readonly int _threadId;
    private bool _disposedValue;

    #endregion Private 字段

    #region Public 属性

    /// <summary>
    /// 是否跨线程调用
    /// </summary>
    public bool InvokeRequired => Thread.CurrentThread.ManagedThreadId != _threadId;

    /// <summary>
    /// 独占线程的托管线程ID
    /// </summary>
    public int ManagedThreadId => _threadId;

    #endregion Public 属性

    #region Public 构造函数

    /// <inheritdoc cref="ExclusiveThreadTaskScheduler"/>
    public ExclusiveThreadTaskScheduler()
    {
        _runningCancellationTokenSource = new CancellationTokenSource();
        _runningToken = _runningCancellationTokenSource.Token;

        _thread = new Thread(InternalWork)
        {
            IsBackground = true,
        };

        //传递弱引用到工作线程，避免强引用，使析构函数能够触发
        _thread.Start(new WeakReference<ExclusiveThreadTaskScheduler>(this));
        _threadId = _thread.ManagedThreadId;
    }

    #endregion Public 构造函数

    #region Public 方法

    /// <summary>
    /// 在此调度器的独占线程上同步执行委托
    /// </summary>
    /// <param name="action"></param>
    public void Run(Action action)
    {
        ThrowIfDisposed();

        if (InvokeRequired)
        {
            using ManualResetEventSlim eventSlim = new(false);
            Exception? exception = null;
            lock (_actionQueue)
            {
                _actionQueue.Enqueue(() =>
                {
                    try
                    {
                        action();
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                    }
                    finally
                    {
                        eventSlim.Set();
                    }
                    return null;
                });
                _actionQueueWaitEvent.Set();
            }
            eventSlim.Wait();

            if (exception != null)
            {
                throw exception;
            }
        }
        else
        {
            action();
        }
    }

    #endregion Public 方法

    #region Private 方法

    private static void InternalWork(object? state)
    {
        var (RunningToken, WaitEvent, TaskQueue, TryExecuteTaskAction) = DeconstructionState(state);

        while (!RunningToken.IsCancellationRequested)
        {
            WaitEvent.WaitOne();
            if (RunningToken.IsCancellationRequested)
            {
                return;
            }

            var shouldLoop = true;
            Func<Task?>? function = null;

            while (shouldLoop
                   && !RunningToken.IsCancellationRequested)
            {
                lock (TaskQueue)
                {
                    shouldLoop = TaskQueue.TryDequeue(out function);
                }

                if (!shouldLoop
                    || function is null)
                {
                    break;
                }

                //HACK  此处是否会出现异常，导致工作线程终止
                var task = function();
                if (task != null)
                {
                    TryExecuteTaskAction(task);
                }
            }
        }

        static (CancellationToken RunningToken, AutoResetEvent WaitEvent, Queue<Func<Task?>> TaskQueue, Action<Task> TryExecuteTaskAction) DeconstructionState(object? state)
        {
            if (state is WeakReference<ExclusiveThreadTaskScheduler> schedulerWeakReference
                && schedulerWeakReference.TryGetTarget(out var taskScheduler)
                && taskScheduler is not null)
            {
                Action<Task> tryExecuteTaskAction = task =>
                {
                    if (schedulerWeakReference.TryGetTarget(out var scheduler))
                    {
                        scheduler.TryExecuteTask(task);
                    }
                    else
                    {
                        throw new ObjectDisposedException(nameof(taskScheduler));
                    }
                };
                return (taskScheduler._runningToken, taskScheduler._actionQueueWaitEvent, taskScheduler._actionQueue, tryExecuteTaskAction);
            }
            else
            {
                throw new ArgumentException("必须将 Scheduler 弱引用传递给工作线程。");
            }
        }
    }

    #endregion Private 方法

    #region TaskScheduler

    // 作为 ConcurrentHashSet<Task> 的替代方案
    private readonly ConcurrentDictionary<Task, byte> _tasks = new();

    /// <inheritdoc/>
    public override int MaximumConcurrencyLevel => 1;

    /// <inheritdoc/>
    protected override IEnumerable<Task>? GetScheduledTasks() => _tasks.Keys;

    /// <inheritdoc/>
    protected override void QueueTask(Task task)
    {
        ThrowIfDisposed();

        _tasks.TryAdd(task, 0);

        task.ContinueWith(innerTask =>
        {
            _tasks.TryRemove(innerTask, out var _);
        });

        lock (_actionQueue)
        {
            _actionQueue.Enqueue(() => task);
            _actionQueueWaitEvent.Set();
        }
    }

    /// <inheritdoc/>
    protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued) => false;

    #endregion TaskScheduler

    #region Dispose

    /// <summary>
    /// 析构函数
    /// </summary>
    ~ExclusiveThreadTaskScheduler()
    {
        Dispose(disposing: false);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            Debug.WriteLine("ExclusiveThreadTaskScheduler Disposed");

            _runningCancellationTokenSource.Cancel();
            _actionQueueWaitEvent.Set();

            lock (_actionQueue)
            {
                _actionQueue.Clear();
            }

            _actionQueueWaitEvent.Dispose();
            _runningCancellationTokenSource.Dispose();

            _disposedValue = true;
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposedValue)
        {
            throw new ObjectDisposedException(nameof(_thread));
        }
    }

    #endregion Dispose
}