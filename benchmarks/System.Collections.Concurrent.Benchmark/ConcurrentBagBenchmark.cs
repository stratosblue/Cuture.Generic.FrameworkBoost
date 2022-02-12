using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmark
{
    [SimpleJob(RuntimeMoniker.Net60)]
    [MemoryDiagnoser]
    public class ConcurrentBagBenchmark
    {
        [Params(1_000_000)]
        public int ItemCount { get; set; }

        #region Public 方法

        [Benchmark]
        public void ParallelAppend_Int()
        {
            var collection = new ConcurrentBag<int>();

            Parallel.For(0, ItemCount, GetParallelOptions(), i =>
            {
                collection.Add(i);
            });
        }

        [Benchmark]
        public void ParallelAppend_Object()
        {
            var collection = new ConcurrentBag<object>();

            var obj = new object();

            Parallel.For(0, ItemCount, GetParallelOptions(), i =>
            {
                collection.Add(obj);
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
}