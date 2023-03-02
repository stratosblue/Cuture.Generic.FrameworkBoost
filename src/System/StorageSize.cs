﻿// <Auto-Generated></Auto-Generated>

using System.Runtime.InteropServices;

namespace System;

/// <summary>
/// 存储大小
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = sizeof(ulong))]
public readonly struct StorageSize : IEquatable<StorageSize>
{
    #region Public 属性

    /// <summary>
    /// Byte
    /// </summary>
    public readonly ulong Byte;

    /// <summary>
    /// KB
    /// </summary>
    public double Kilobyte => Byte / 1024.0;

    /// <summary>
    /// MB
    /// </summary>
    public double Megabyte => Byte / (1024.0 * 1024);

    /// <summary>
    /// GB
    /// </summary>
    public double Gigabyte => Byte / (1024.0 * 1024 * 1024);

    /// <summary>
    /// TB
    /// </summary>
    public double Terabyte => Byte / (1024.0 * 1024 * 1024 * 1024);

    /// <summary>
    /// PB
    /// </summary>
    public double Petabyte => Byte / (1024.0 * 1024 * 1024 * 1024 * 1024);

    #endregion Public 属性

    #region Public 构造函数

    /// <summary>
    /// <inheritdoc cref="StorageSize"/>
    /// </summary>
    /// <param name="byteSize">Byte 大小</param>
    public StorageSize(ulong byteSize)
    {
        Byte = byteSize;
    }

    /// <summary>
    /// <inheritdoc cref="StorageSize"/>
    /// </summary>
    /// <param name="byteSize">Byte 大小</param>
    public StorageSize(long byteSize)
    {
        if (byteSize < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(byteSize));
        }
        Byte = (ulong)byteSize;
    }

    #endregion Public 构造函数

    #region Public 方法

    /// <summary>
    /// + 运算符
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static StorageSize operator +(StorageSize a, StorageSize b) => new(a.Byte + b.Byte);

    /// <summary>
    /// - 运算符
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static StorageSize operator -(StorageSize a, StorageSize b) => new(a.Byte - b.Byte);

    /// <summary>
    /// * 运算符
    /// </summary>
    /// <param name="a"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static StorageSize operator *(StorageSize a, int value) => value >= 0 ? new(a.Byte * (uint)value) : throw new ArgumentOutOfRangeException(nameof(value));

    /// <summary>
    /// * 运算符
    /// </summary>
    /// <param name="a"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static StorageSize operator *(StorageSize a, uint value) => new(a.Byte * value);

    /// <summary>
    /// / 运算符
    /// </summary>
    /// <param name="a"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static StorageSize operator /(StorageSize a, int value) => value >= 0 ? new(a.Byte / (uint)value) : throw new ArgumentOutOfRangeException(nameof(value));

    /// <summary>
    /// / 运算符
    /// </summary>
    /// <param name="a"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static StorageSize operator /(StorageSize a, uint value) => new(a.Byte / value);

    /// <summary>
    /// 大于 运算符
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool operator >(StorageSize a, StorageSize b) => a.Byte > b.Byte;

    /// <summary>
    /// 小于 运算符
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool operator <(StorageSize a, StorageSize b) => a.Byte < b.Byte;

    /// <summary>
    /// 等于 运算符
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool operator ==(StorageSize a, StorageSize b) => a.Byte == b.Byte;

    /// <summary>
    /// != 运算符
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool operator !=(StorageSize a, StorageSize b) => a.Byte != b.Byte;

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is StorageSize storage && Equals(storage);
    }

    /// <inheritdoc/>
    public bool Equals(StorageSize other)
    {
        return Byte == other.Byte;
    }

    /// <inheritdoc/>
    public override int GetHashCode() => Byte.GetHashCode();

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Megabyte:F4}Mb";
    }

    #endregion Public 方法
}