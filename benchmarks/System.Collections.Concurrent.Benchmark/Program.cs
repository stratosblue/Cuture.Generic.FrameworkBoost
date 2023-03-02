using BenchmarkDotNet.Running;

namespace Benchmark;

class Program
{
    static void Main(string[] args)
    {
        BenchmarkRunner.Run<GeneralBenchmark>();
        BenchmarkRunner.Run<ConcurrentBagBenchmark>();
        BenchmarkRunner.Run<ConcurrentDictionaryBenchmark>();
    }
}
