using BenchmarkDotNet.Attributes;

namespace Benchmark
{
    [SimpleJob]
    [MemoryDiagnoser]
    public class InParameterModifierBenchmark
    {
        #region Public 方法

        [Benchmark]
        public void InParameterModifier_With_Struct_Fields_10()
        {
            new GenericBenchmark<Struct_Fields_10>().WithInParameterModifier();
        }

        [Benchmark]
        public void InParameterModifier_With_Struct_Fields_2()
        {
            new GenericBenchmark<Struct_Fields_2>().WithInParameterModifier();
        }

        [Benchmark]
        public void InParameterModifier_With_Struct_Fields_20()
        {
            new GenericBenchmark<Struct_Fields_20>().WithInParameterModifier();
        }

        [Benchmark]
        public void InParameterModifier_With_Struct_Fields_40()
        {
            new GenericBenchmark<Struct_Fields_40>().WithInParameterModifier();
        }

        [Benchmark]
        public void Nothing_With_Struct_Fields_10()
        {
            new GenericBenchmark<Struct_Fields_10>().WithNothing();
        }

        [Benchmark]
        public void Nothing_With_Struct_Fields_2()
        {
            new GenericBenchmark<Struct_Fields_2>().WithNothing();
        }

        [Benchmark]
        public void Nothing_With_Struct_Fields_20()
        {
            new GenericBenchmark<Struct_Fields_20>().WithNothing();
        }

        [Benchmark]
        public void Nothing_With_Struct_Fields_40()
        {
            new GenericBenchmark<Struct_Fields_40>().WithNothing();
        }

        [Benchmark]
        public void RefParameterModifier_With_Struct_Fields_10()
        {
            new GenericBenchmark<Struct_Fields_10>().WithRefParameterModifier();
        }

        [Benchmark]
        public void RefParameterModifier_With_Struct_Fields_2()
        {
            new GenericBenchmark<Struct_Fields_2>().WithRefParameterModifier();
        }

        [Benchmark]
        public void RefParameterModifier_With_Struct_Fields_20()
        {
            new GenericBenchmark<Struct_Fields_20>().WithRefParameterModifier();
        }

        [Benchmark]
        public void RefParameterModifier_With_Struct_Fields_40()
        {
            new GenericBenchmark<Struct_Fields_40>().WithRefParameterModifier();
        }

        #endregion Public 方法
    }
}