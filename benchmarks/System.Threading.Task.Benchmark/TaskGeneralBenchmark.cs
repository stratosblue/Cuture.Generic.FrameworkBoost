using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmark;

[SimpleJob(RuntimeMoniker.Net60)]
[MemoryDiagnoser]
public class TaskGeneralBenchmark
{
}