using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmark
{
    [SimpleJob(RuntimeMoniker.NetCoreApp50)]
    [MemoryDiagnoser]
    public class ConcurrentDictionaryBenchmark
    {
        #region Private 字段

        private readonly Guid[] _allKeys;
        private readonly object _data = new();

        #endregion Private 字段

        #region Public 属性

        [Params(4, 8)]
        public int MaxDegreeOfParallelism { get; set; }

        #endregion Public 属性

        #region Public 构造函数

        public ConcurrentDictionaryBenchmark()
        {
            _allKeys = Enumerable.Range(0, 1_000_000).Select(m => Guid.NewGuid()).ToArray();
        }

        #endregion Public 构造函数

        #region Public 方法

        [Benchmark]
        public void ConcurrentDictionary_Parallel_GetOrAdd()
        {
            var dic = new ConcurrentDictionary<Guid, object>();

            Parallel.For(0, _allKeys.Length, GetParallelOptions(), i =>
            {
                dic.GetOrAdd(_allKeys[i], _data);
            });
        }

        [Benchmark]
        public void ConcurrentDictionary_Parallel_TryAdd()
        {
            var dic = new ConcurrentDictionary<Guid, object>();

            Parallel.For(0, _allKeys.Length, GetParallelOptions(), i =>
            {
                dic.TryAdd(_allKeys[i], _data);
            });
        }

        [Benchmark]
        public void ConcurrentDictionary_Parallel_TryAdd_Conflict()
        {
            var dic = new ConcurrentDictionary<Guid, object>();

            Parallel.For(0, _allKeys.Length - 1, GetParallelOptions(), i =>
            {
                dic.TryAdd(_allKeys[i], _data);
                dic.TryAdd(_allKeys[i + 1], _data);
            });
        }

        [Benchmark]
        public void Dictionary_Parallel_TryAdd_WithLock()
        {
            var dic = new Dictionary<Guid, object>();

            Parallel.For(0, _allKeys.Length, GetParallelOptions(), i =>
            {
                lock (_data)
                {
                    dic.TryAdd(_allKeys[i], _data);
                }
            });
        }

        [Benchmark]
        public void Dictionary_Parallel_TryAdd_WithLock_Conflict()
        {
            var dic = new Dictionary<Guid, object>();

            Parallel.For(0, _allKeys.Length - 1, GetParallelOptions(), i =>
            {
                lock (_data)
                {
                    dic.TryAdd(_allKeys[i], _data);
                    dic.TryAdd(_allKeys[i + 1], _data);
                }
            });
        }

        [Benchmark]
        public void Dictionary_Parallel_TryAdd_WithLock_GetOrAdd()
        {
            var dic = new Dictionary<Guid, object>();

            Parallel.For(0, _allKeys.Length, GetParallelOptions(), i =>
            {
                var key = _allKeys[i];
                lock (_data)
                {
                    if (!dic.TryGetValue(key, out var data))
                    {
                        data = _data;
                        dic.Add(key, data);
                    }
                }
            });
        }

        #endregion Public 方法

        #region Private 方法

        private ParallelOptions GetParallelOptions()
        {
            return new ParallelOptions()
            {
                MaxDegreeOfParallelism = MaxDegreeOfParallelism
            };
        }

        #endregion Private 方法
    }
}