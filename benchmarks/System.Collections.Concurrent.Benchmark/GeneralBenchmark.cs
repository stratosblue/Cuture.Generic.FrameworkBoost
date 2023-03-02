using System.Collections.Concurrent;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmark;

[SimpleJob(RuntimeMoniker.Net60)]
[MemoryDiagnoser]
public class GeneralBenchmark
{
    [Params(100, 1000)]
    public int FlushThreshold { get; set; }

    [Params(1_000_000)]
    public int ItemCount { get; set; }

    #region Public 方法

    [Benchmark]
    public void ParallelAppend_Int()
    {
        var collection = new AsyncCallbackDeferFlushCollection<int>((items, token) => Task.CompletedTask, FlushThreshold, TimeSpan.FromSeconds(2));

        Parallel.For(0, ItemCount, GetParallelOptions(), i =>
        {
            collection.Append(i);
        });
    }

    [Benchmark]
    public void ParallelAppend_Object()
    {
        var collection = new AsyncCallbackDeferFlushCollection<object>((items, token) => Task.CompletedTask, FlushThreshold, TimeSpan.FromSeconds(2));

        var obj = new object();

        Parallel.For(0, ItemCount, GetParallelOptions(), i =>
        {
            collection.Append(obj);
        });
    }

    #endregion Public 方法

    #region Private 方法

    private ParallelOptions GetParallelOptions()
    {
        return new ParallelOptions()
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount - 2
        };
    }

    #endregion Private 方法
}