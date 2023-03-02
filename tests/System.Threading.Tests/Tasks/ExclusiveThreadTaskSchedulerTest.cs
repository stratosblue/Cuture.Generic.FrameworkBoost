using System.Runtime.CompilerServices;

namespace System.Threading.Tests.Tasks;

[TestClass]
public class ExclusiveThreadTaskSchedulerTest
{
    #region Public 方法

    [TestMethod]
    public async Task AsyncExecuteExceptionFlow()
    {
        using var taskScheduler = new ExclusiveThreadTaskScheduler();
        var initThreadId = taskScheduler.ManagedThreadId;

        await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
        {
            return Task.Factory.StartNew(function: async () =>
            {
                await Task.Delay(1);
                SameThreadIdAssert(initThreadId, $"Task.Factory.StartNew async continue");
                throw new ArgumentException();
            },
            cancellationToken: CancellationToken.None,
            creationOptions: TaskCreationOptions.None,
            scheduler: taskScheduler).Unwrap();
        });
    }

    [TestMethod]
    public async Task AsyncRecursionExecute()
    {
        using var taskScheduler = new ExclusiveThreadTaskScheduler();
        var initThreadId = taskScheduler.ManagedThreadId;

        await RecursionExecuteAsync(taskScheduler, initThreadId, 100);

        static async Task RecursionExecuteAsync(ExclusiveThreadTaskScheduler scheduler, int threadId, int count)
        {
            if (count == 0)
            {
                return;
            }
            await Task.Factory.StartNew(function: async () =>
            {
                await Task.Delay(1);
                SameThreadIdAssert(threadId, $"Task.Factory.StartNew async RecursionExecute【{count}】");
                if (count % 2 == 0)
                {
                    scheduler.Run(() =>
                    {
                        SameThreadIdAssert(threadId, $"ExclusiveThreadTaskScheduler.Run in async【{count}】");
                    });
                }
                await RecursionExecuteAsync(scheduler, threadId, count - 1);
            },
            cancellationToken: CancellationToken.None,
            creationOptions: TaskCreationOptions.None,
            scheduler: scheduler).Unwrap();
        }
    }

    [TestMethod]
    public async Task CommonUsage()
    {
        using var taskScheduler = new ExclusiveThreadTaskScheduler();

        Assert.AreEqual(1, taskScheduler.MaximumConcurrencyLevel);

        var initThreadId = taskScheduler.ManagedThreadId;

        Console.WriteLine($"ExclusiveThreadTaskScheduler ManagedThreadId: {initThreadId}");

        for (int i = 1; i < 3; i++)
        {
            await Task.Factory.StartNew(action: () => SameThreadIdAssert(initThreadId, $"Task.Factory.StartNew sync【{i}】"),
                                        cancellationToken: CancellationToken.None,
                                        creationOptions: TaskCreationOptions.None,
                                        scheduler: taskScheduler);

            taskScheduler.Run(() => SameThreadIdAssert(initThreadId, $"ExclusiveThreadTaskScheduler.Run【{i}】"));

            await Task.Factory.StartNew(async () => SameThreadIdAssert(initThreadId, $"Task.Factory.StartNew fasync【{i}】"),
                                        cancellationToken: CancellationToken.None,
                                        creationOptions: TaskCreationOptions.None,
                                        scheduler: taskScheduler).Unwrap();

            await Task.Factory.StartNew(function: async () =>
            {
                SameThreadIdAssert(initThreadId, $"Task.Factory.StartNew async start【{i}】");
                await Task.Delay(1);
                SameThreadIdAssert(initThreadId, $"Task.Factory.StartNew async【{i}】");
                await Task.Delay(1);
                SameThreadIdAssert(initThreadId, $"Task.Factory.StartNew async continue【{i}】");
                await Task.Delay(1);
                SameThreadIdAssert(initThreadId, $"Task.Factory.StartNew async continue2【{i}】");
            },
            cancellationToken: CancellationToken.None,
            creationOptions: TaskCreationOptions.None,
            scheduler: taskScheduler).Unwrap();

            var task = new Task(() => SameThreadIdAssert(initThreadId, $"Task.RunSynchronously【{i}】"));
            task.RunSynchronously(taskScheduler);
        }
    }

    [TestMethod]
    public void DestructorWithGC()
    {
        var (WeakReference, DelayRunAction) = CreateScheduler();

        GC.Collect();
        GC.WaitForPendingFinalizers();

        CheckWeakReference(WeakReference, true);

        DelayRunAction();

        GC.Collect();
        GC.WaitForPendingFinalizers();

        CheckWeakReference(WeakReference, false);

        static (WeakReference<ExclusiveThreadTaskScheduler> WeakReference, Action DelayRunAction) CreateScheduler()
        {
            var taskScheduler = new ExclusiveThreadTaskScheduler();
            var initThreadId = taskScheduler.ManagedThreadId;
            Console.WriteLine($"ExclusiveThreadTaskScheduler Created ManagedThreadId: {initThreadId}");
            taskScheduler.Run(() => SameThreadIdAssert(initThreadId, $"ExclusiveThreadTaskScheduler.Run"));

            //避免直接引用 taskScheduler
            var holder = new ObjectHolder<ExclusiveThreadTaskScheduler>(taskScheduler);

            return (new(taskScheduler),
                    () =>
                    {
                        //执行并取消引用
                        var scheduler = holder.Value;
                        scheduler.Run(() => SameThreadIdAssert(initThreadId, $"ExclusiveThreadTaskScheduler.Run Delay"));
                        holder.Value = null;
                        scheduler = null;
                    }
            );
        }

        static void CheckWeakReference(WeakReference<ExclusiveThreadTaskScheduler> WeakReference, bool shouldGet)
        {
            var gotResult = WeakReference.TryGetTarget(out var _);

            Assert.AreEqual(shouldGet, gotResult);
        }
    }

    [TestMethod]
    public async Task ParallelExecute()
    {
        using var taskScheduler = new ExclusiveThreadTaskScheduler();
        var initThreadId = taskScheduler.ManagedThreadId;

        var tasks = Enumerable.Range(0, 500).Select(m => Task.Factory.StartNew(function: async () =>
        {
            await Task.Delay(1);
            SameThreadIdAssert(initThreadId, $"Task.Factory.StartNew async ParallelRun");
            await Task.Delay(1);
            SameThreadIdAssert(initThreadId, $"Task.Factory.StartNew async ParallelRun2");
        }, cancellationToken: CancellationToken.None, creationOptions: TaskCreationOptions.None, scheduler: taskScheduler).Unwrap());

        Parallel.For(0, 500, i =>
        {
            taskScheduler.Run(() => SameThreadIdAssert(initThreadId, $"ExclusiveThreadTaskScheduler.Run ParallelRun"));
        });

        await Task.WhenAll(tasks);
    }

    [TestMethod]
    public void SyncExecuteExceptionFlow()
    {
        using var taskScheduler = new ExclusiveThreadTaskScheduler();
        var initThreadId = taskScheduler.ManagedThreadId;

        Assert.ThrowsException<ArgumentException>(() => taskScheduler.Run(() =>
        {
            SameThreadIdAssert(initThreadId, $"ExclusiveThreadTaskScheduler.Run");
            throw new ArgumentException();
        }));
    }

    [TestMethod]
    public void SyncRecursionExecute()
    {
        using var taskScheduler = new ExclusiveThreadTaskScheduler();
        var initThreadId = taskScheduler.ManagedThreadId;

        RecursionExecute(taskScheduler, initThreadId, 100);

        static void RecursionExecute(ExclusiveThreadTaskScheduler scheduler, int threadId, int count)
        {
            if (count == 0)
            {
                return;
            }
            scheduler.Run(() =>
            {
                SameThreadIdAssert(threadId, $"ExclusiveThreadTaskScheduler.Run RecursionExecute【{count}】");
                RecursionExecute(scheduler, threadId, count - 1);
            });
        }
    }

    #endregion Public 方法

    #region Private 方法

    private static void SameThreadIdAssert(int threadId, [CallerMemberName] string callerName = null)
    {
        var currentThreadId = Thread.CurrentThread.ManagedThreadId;
        Console.WriteLine($"{callerName} ManagedThreadId: {currentThreadId}");
        Assert.AreEqual(threadId, currentThreadId, $"执行 {callerName} 时，线程ID不同");
    }

    #endregion Private 方法

    #region Internal 类

    private class ObjectHolder<T>
    {
        #region Public 属性

        public T Value { get; set; }

        #endregion Public 属性

        #region Public 构造函数

        public ObjectHolder(T value)
        {
            Value = value;
        }

        #endregion Public 构造函数
    }

    #endregion Internal 类
}