namespace System.Threading;

/// <summary>
/// 可变的 <inheritdoc cref="AsyncLocal{T}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class VolatileAsyncLocal<T>
{
    private readonly AsyncLocal<VolatileAsyncLocalValueHolder<T>> _innerAsyncLocal;

    /// <inheritdoc cref="AsyncLocal{T}.Value"/>
    public T? Value
    {
        get => _innerAsyncLocal.Value!.Value;
        set => _innerAsyncLocal.Value!.Value = value;
    }

    /// <inheritdoc cref="VolatileAsyncLocal{T}"/>
    public VolatileAsyncLocal()
    {
        _innerAsyncLocal = new()
        {
            Value = new()
        };
    }

    /// <summary>
    /// <inheritdoc cref="VolatileAsyncLocal{T}"/>
    /// </summary>
    /// <param name="valueChangedHandler">值变更回调委托</param>
    public VolatileAsyncLocal(Action<VolatileAsyncLocalValueChangedArgs<T>>? valueChangedHandler)
    {
        _innerAsyncLocal = valueChangedHandler is null
                           ? new()
                           : new(m => valueChangedHandler(new(m)));

        _innerAsyncLocal.Value = new();
    }

    internal class VolatileAsyncLocalValueHolder<TValue>
    {
        public TValue? Value { get; set; }
    }
}

/// <inheritdoc cref="AsyncLocalValueChangedArgs{T}"/>
public readonly struct VolatileAsyncLocalValueChangedArgs<T>
{
    /// <inheritdoc cref="AsyncLocalValueChangedArgs{T}.CurrentValue"/>
    public T? CurrentValue { get; }

    /// <inheritdoc cref="AsyncLocalValueChangedArgs{T}.PreviousValue"/>
    public T? PreviousValue { get; }

    /// <inheritdoc cref="AsyncLocalValueChangedArgs{T}.ThreadContextChanged"/>
    public bool ThreadContextChanged { get; }

    internal VolatileAsyncLocalValueChangedArgs(AsyncLocalValueChangedArgs<VolatileAsyncLocal<T>.VolatileAsyncLocalValueHolder<T>> args)
    {
        CurrentValue = args.CurrentValue!.Value;
        PreviousValue = args.PreviousValue!.Value;
        ThreadContextChanged = args.ThreadContextChanged;
    }
}
