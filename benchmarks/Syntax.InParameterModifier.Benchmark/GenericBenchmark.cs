using System.Runtime.CompilerServices;

namespace Benchmark
{
    public class GenericBenchmark<TStruct> where TStruct : struct
    {
        #region Public 字段

        public const int CallCount = 10_000_000;

        #endregion Public 字段

        #region Public 方法

        public void WithInParameterModifier()
        {
            var data = new TStruct();
            for (int i = 0; i < CallCount; i++)
            {
                WithInParameterModifier(in data);
            }
        }

        public void WithNothing()
        {
            var data = new TStruct();
            for (int i = 0; i < CallCount; i++)
            {
                WithNothing(data);
            }
        }

        public void WithRefParameterModifier()
        {
            var data = new TStruct();
            for (int i = 0; i < CallCount; i++)
            {
                WithRefParameterModifier(ref data);
            }
        }

        #endregion Public 方法

        #region Private 方法

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void WithInParameterModifier(in TStruct data)
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void WithNothing(TStruct data)
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void WithRefParameterModifier(ref TStruct data)
        {
        }

        #endregion Private 方法
    }
}