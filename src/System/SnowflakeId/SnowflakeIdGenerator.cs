using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.SnowflakeId;

/// <summary>
/// 雪花ID生成器选项
/// </summary>
public sealed class SnowflakeIdGeneratorOptions
{
    #region Public 属性

    /// <summary>
    /// 纪元UTC时间
    /// </summary>
    public DateTime EpochUTCDateTime { get; set; } = new DateTime(2022, 1, 1);

    /// <summary>
    /// 时间回退等待阈值 (Ticks)
    /// </summary>
    public long TimebackWaitThreshold { get; set; } = 10 * TimeSpan.TicksPerSecond;

    /// <summary>
    /// 大于 0 小于 <see cref="SnowflakeIdGenerator.MaxWorkerId"/> 的值
    /// </summary>
    public int WorkerId { get; set; }

    #endregion Public 属性
}

/// <summary>
/// 雪花ID生成器
/// </summary>
public sealed class SnowflakeIdGenerator
{
    #region const

    #region BitLengths

    /// <summary>
    /// 序列号位数
    /// </summary>
    internal const int SequenceBits = 12;

    /// <summary>
    /// 时间戳位数
    /// </summary>
    internal const int TimestampBits = 29;

    /// <summary>
    /// 工作Id位数
    /// </summary>
    internal const int WorkerIdBits = 10;

    #endregion BitLengths

    #region Thresholds

    /// <summary>
    /// WorkerId最大值
    /// </summary>
    public const int MaxWorkerId = ~(-1 << WorkerIdBits);

    /// <summary>
    /// 单位时间内最大序列
    /// </summary>
    internal const int MaxSequence = ~(-1 << SequenceBits);

    #endregion Thresholds

    #endregion const

    #region Private 字段

    private readonly long _epochTimestamp;

    private readonly object _syncRoot = new();

    private readonly long _timebackWaitThreshold;
    private readonly long _workerId;

    private long _currentIdWithoutSequence;

    private long _currentSequence;

    private long _currentTimestamp;

    private DateTime _lastTime;

    #endregion Private 字段

    #region Public 构造函数

    /// <summary>
    /// <inheritdoc cref="SnowflakeIdGenerator"/>
    /// </summary>
    /// <param name="workerId">大于 0 小于 <see cref="MaxWorkerId"/> 的值</param>
    /// <exception cref="ArgumentException"></exception>
    public SnowflakeIdGenerator(int workerId) : this(new SnowflakeIdGeneratorOptions() { WorkerId = workerId })
    {
    }

    /// <summary>
    /// <inheritdoc cref="SnowflakeIdGenerator"/>
    /// </summary>
    /// <param name="options">选项</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public SnowflakeIdGenerator(SnowflakeIdGeneratorOptions options)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        var workerId = options.WorkerId;
        if (workerId > MaxWorkerId
            || workerId < 0)
        {
            throw new ArgumentException($"Worker id can not be greater than '{MaxWorkerId}' or less than '0'.");
        }

        _workerId = workerId << SequenceBits;

        _timebackWaitThreshold = options.TimebackWaitThreshold;
        _epochTimestamp = GetTimestamp(options.EpochUTCDateTime);

        if (_epochTimestamp > DateTimeOffset.UtcNow.ToUnixTimeMilliseconds())
        {
            throw new ArgumentException($"EpochYear - {options.EpochUTCDateTime} is larger than current time.");
        }
    }

    #endregion Public 构造函数

    #region Public 方法

    /// <summary>
    /// 生成下一个ID
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public ValueTask<long> NextAsync(CancellationToken cancellationToken = default)
    {
        lock (_syncRoot)
        {
            var now = GetNewestTime();
            if (now > _lastTime)   //时间正确递增
            {
                var nowTimestamp = GetTimestamp(in now);
                if (nowTimestamp != _currentTimestamp)   //时间戳变更
                {
                    ResetCurrentSequence(in nowTimestamp);
                    _currentTimestamp = nowTimestamp;
                    _lastTime = now;
                }
            }
            else if (now < _lastTime)   //时间回退
            {
                return DelayRetryForTimeback(now, cancellationToken);
            }

            IncrementCurrentSequence();

            return new(_currentIdWithoutSequence | _currentSequence);
        }
    }

    #endregion Public 方法

    #region Private 方法

    /// <summary>
    /// 获取最新时间
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static DateTime GetNewestTime() => DateTime.UtcNow;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long GetTimestamp(in DateTime dateTime) => new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();

    /// <summary>
    /// 延时进行重试以应对时间回退
    /// </summary>
    /// <param name="now"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private async ValueTask<long> DelayRetryForTimeback(DateTime now, CancellationToken cancellationToken)
    {
        //TODO 此处存在递归

        var interval = now - _lastTime;
        if (interval.Ticks > _timebackWaitThreshold)
        {
            throw new InvalidOperationException($"Timeback recovery failed. Timeback value - '{interval.Ticks}' is bigger than wait threshold '{_timebackWaitThreshold}'.");
        }

        await Task.Delay(interval, cancellationToken);

        return await NextAsync(cancellationToken);
    }

    /// <summary>
    /// 当前序列自增
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void IncrementCurrentSequence()
    {
        if (++_currentSequence > MaxSequence)
        {
            throw new InvalidOperationException($"The sequence in unit time has been used up.");
        }
    }

    /// <summary>
    /// 重置当前序列为当前时间戳的起始序列
    /// </summary>
    /// <param name="currentTimestamp"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ResetCurrentSequence(in long currentTimestamp)
    {
        long idWithoutSequence = (currentTimestamp - _epochTimestamp) << (WorkerIdBits + SequenceBits);
        idWithoutSequence |= _workerId;

        _currentIdWithoutSequence = idWithoutSequence;
        _currentSequence = 0;
    }

    #endregion Private 方法
}
