using BenchmarkDotNet.Running;

namespace Benchmark;

internal class Program
{
    #region Private 方法

    private static void Main(string[] args)
    {
        BenchmarkRunner.Run<SpinLockGeneralBenchmark>();
    }

    #endregion Private 方法
}