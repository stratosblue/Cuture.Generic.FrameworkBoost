using System;
using System.Threading;
using System.Threading.Tasks;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmark
{
    [SimpleJob(RuntimeMoniker.Net60)]
    [MemoryDiagnoser]
    public class SpinLockGeneralBenchmark
    {
        #region Private 字段

        private const int Count = 1_000_000;
        private const int Sleep = 1;
        private const int SleepCount = 1000;

        #endregion Private 字段

        #region Public 方法

        [Benchmark]
        public void ParallelLock()
        {
            object syncRoot = new();
            int count = 0;

            Parallel.For(0, Count, i =>
            {
                lock (syncRoot)
                {
                    count++;
                }
            });

            if (count != Count)
            {
                throw new Exception();
            }
        }

        [Benchmark]
        public void ParallelLockWithSleep()
        {
            object syncRoot = new();
            int count = 0;

            Parallel.For(0, SleepCount, i =>
            {
                lock (syncRoot)
                {
                    count += 1;
                    Thread.Sleep(Sleep);
                }
            });

            if (count != SleepCount)
            {
                throw new Exception();
            }
        }

        [Benchmark]
        public void ParallelLockWithWorkLoad()
        {
            object syncRoot = new();
            int count = 0;

            Parallel.For(0, Count, i =>
            {
                lock (syncRoot)
                {
                    count += WorkLoad(i);
                }
            });

            if (count != Count)
            {
                throw new Exception();
            }
        }

        [Benchmark]
        public void ParallelSpinLock()
        {
            SpinLock spinLock = new(false);

            int count = 0;

            Parallel.For(0, Count, i =>
            {
                bool lockTaken = false;
                try
                {
                    spinLock.Enter(ref lockTaken);
                    count++;
                }
                finally
                {
                    if (lockTaken)
                    {
                        spinLock.Exit(false);
                    }
                }
            });

            if (count != Count)
            {
                throw new Exception();
            }
        }

        [Benchmark]
        public void ParallelSpinLockWithSleep()
        {
            SpinLock spinLock = new(false);

            int count = 0;

            Parallel.For(0, SleepCount, i =>
            {
                bool lockTaken = false;
                try
                {
                    spinLock.Enter(ref lockTaken);
                    count += 1;
                    Thread.Sleep(Sleep);
                }
                finally
                {
                    if (lockTaken)
                    {
                        spinLock.Exit(false);
                    }
                }
            });

            if (count != SleepCount)
            {
                throw new Exception();
            }
        }

        [Benchmark]
        public void ParallelSpinLockWithWorkLoad()
        {
            SpinLock spinLock = new(false);

            int count = 0;

            Parallel.For(0, Count, i =>
            {
                bool lockTaken = false;
                try
                {
                    spinLock.Enter(ref lockTaken);
                    count += WorkLoad(i);
                }
                finally
                {
                    if (lockTaken)
                    {
                        spinLock.Exit(false);
                    }
                }
            });

            if (count != Count)
            {
                throw new Exception();
            }
        }

        [Benchmark]
        public void SequentialLock()
        {
            object syncRoot = new();
            int count = 0;

            for (int i = 0; i < Count; i++)
            {
                lock (syncRoot)
                {
                    count++;
                }
            }

            if (count != Count)
            {
                throw new Exception();
            }
        }

        [Benchmark]
        public void SequentialLockWithSleep()
        {
            object syncRoot = new();
            int count = 0;

            for (int i = 0; i < SleepCount; i++)
            {
                lock (syncRoot)
                {
                    count++;
                    Thread.Sleep(Sleep);
                }
            }

            if (count != SleepCount)
            {
                throw new Exception();
            }
        }

        [Benchmark]
        public void SequentialLockWithWorkLoad()
        {
            object syncRoot = new();
            int count = 0;

            for (int i = 0; i < Count; i++)
            {
                lock (syncRoot)
                {
                    count += WorkLoad(i);
                }
            }

            if (count != Count)
            {
                throw new Exception();
            }
        }

        [Benchmark]
        public void SequentialSpinLock()
        {
            SpinLock spinLock = new(false);

            int count = 0;

            for (int i = 0; i < Count; i++)
            {
                bool lockTaken = false;
                try
                {
                    spinLock.Enter(ref lockTaken);
                    count++;
                }
                finally
                {
                    if (lockTaken)
                    {
                        spinLock.Exit(false);
                    }
                }
            };

            if (count != Count)
            {
                throw new Exception();
            }
        }

        [Benchmark]
        public void SequentialSpinLockWithSleep()
        {
            SpinLock spinLock = new(false);

            int count = 0;

            for (int i = 0; i < SleepCount; i++)
            {
                bool lockTaken = false;
                try
                {
                    spinLock.Enter(ref lockTaken);
                    count++;
                    Thread.Sleep(Sleep);
                }
                finally
                {
                    if (lockTaken)
                    {
                        spinLock.Exit(false);
                    }
                }
            };

            if (count != SleepCount)
            {
                throw new Exception();
            }
        }

        [Benchmark]
        public void SequentialSpinLockWithWorkLoad()
        {
            SpinLock spinLock = new(false);

            int count = 0;

            for (int i = 0; i < Count; i++)
            {
                bool lockTaken = false;
                try
                {
                    spinLock.Enter(ref lockTaken);
                    count += WorkLoad(i);
                }
                finally
                {
                    if (lockTaken)
                    {
                        spinLock.Exit(false);
                    }
                }
            };

            if (count != Count)
            {
                throw new Exception();
            }
        }

        #endregion Public 方法

        #region Private 方法

        private static int WorkLoad(int i)
        {
            for (int l = 0; l < 100; l++)
            {
                Math.Sqrt(Math.Log(Math.Log2(Math.Sqrt(i * 313233343536373839.3311))));
            }

            return 1;
        }

        #endregion Private 方法
    }
}