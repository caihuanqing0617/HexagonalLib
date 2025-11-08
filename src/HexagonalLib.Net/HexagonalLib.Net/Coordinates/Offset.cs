using System;
using System.Collections.Generic;

namespace HexagonalLib.Coordinates;

/// <summary>
/// 表示一个二维偏移量（X, Y），用于表示坐标或位置的相对变化。
/// 支持基本的数学运算、比较操作以及序列化功能。
/// </summary>
[Serializable]
public readonly partial struct Offset : IEquatable<Offset>, IEqualityComparer<Offset>
{
    /// <summary>
    /// 获取零偏移量（X=0, Y=0）。
    /// </summary>
    public static Offset Zero => new(0, 0);

    /// <summary>
    /// 偏移量在 X 轴上的分量。
    /// </summary>
    public readonly int X;

    /// <summary>
    /// 偏移量在 Y 轴上的分量。
    /// </summary>
    public readonly int Y;

    /// <summary>
    /// 初始化一个新的 <see cref="Offset"/> 实例。
    /// </summary>
    /// <param name="x">X 轴方向的偏移量。</param>
    /// <param name="y">Y 轴方向的偏移量。</param>
    public Offset(int x, int y)
        : this()
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// 返回当前偏移量加上指定增量后的新偏移量。
    /// </summary>
    /// <param name="xOffset">要加到 X 分量上的值。</param>
    /// <param name="yOffset">要加到 Y 分量上的值。</param>
    /// <returns>新的偏移量实例。</returns>
    public Offset Add(int xOffset, int yOffset) => new(X + xOffset, Y + yOffset);

    /// <summary>
    /// 将给定的偏移量限制在一个最小和最大范围之间。
    /// </summary>
    /// <param name="coord">需要被限制的偏移量。</param>
    /// <param name="min">允许的最小偏移量。</param>
    /// <param name="max">允许的最大偏移量。</param>
    /// <returns>经过裁剪后的偏移量。</returns>
    public static Offset Clamp(Offset coord, Offset min, Offset max)
    {
        var x = Clamp(coord.X, min.X, max.X);
        var y = Clamp(coord.Y, min.Y, max.Y);

        return new Offset(x, y);
    }

    /// <summary>
    /// 将整数值限制在指定范围内。
    /// </summary>
    /// <param name="value">待处理的值。</param>
    /// <param name="min">最小边界。</param>
    /// <param name="max">最大边界。</param>
    /// <returns>如果超出范围则返回边界值，否则返回原值。</returns>
    private static int Clamp(int value, int min, int max)
    {
        if (value < min)
        {
            value = min;
        }
        else if (value > max)
        {
            value = max;
        }

        return value;
    }

    /// <summary>
    /// 判断两个偏移量是否相等。
    /// </summary>
    /// <param name="coord1">第一个偏移量。</param>
    /// <param name="coord2">第二个偏移量。</param>
    /// <returns>若两个偏移量完全相同则返回 true；否则返回 false。</returns>
    public static bool operator ==(Offset coord1, Offset coord2)
        => (coord1.X, coord1.Y) == (coord2.X, coord2.Y);

    /// <summary>
    /// 判断两个偏移量是否不相等。
    /// </summary>
    /// <param name="coord1">第一个偏移量。</param>
    /// <param name="coord2">第二个偏移量。</param>
    /// <returns>若两个偏移量不同则返回 true；否则返回 false。</returns>
    public static bool operator !=(Offset coord1, Offset coord2)
        => (coord1.X, coord1.Y) != (coord2.X, coord2.Y);

    /// <summary>
    /// 对两个偏移量执行向量加法。
    /// </summary>
    /// <param name="coord1">第一个偏移量。</param>
    /// <param name="coord2">第二个偏移量。</param>
    /// <returns>两者的和组成的新的偏移量。</returns>
    public static Offset operator +(Offset coord1, Offset coord2)
        => new(coord1.X + coord2.X, coord1.Y + coord2.Y);

    /// <summary>
    /// 给偏移量的每个维度增加相同的标量值。
    /// </summary>
    /// <param name="coord">原始偏移量。</param>
    /// <param name="offset">要添加的标量值。</param>
    /// <returns>新计算出的偏移量。</returns>
    public static Offset operator +(Offset coord, int offset)
        => new(coord.X + offset, coord.Y + offset);

    /// <summary>
    /// 执行两个偏移量之间的减法操作。
    /// </summary>
    /// <param name="coord">被减数偏移量。</param>
    /// <param name="index2">减数偏移量。</param>
    /// <returns>差值组成的新偏移量。</returns>
    public static Offset operator -(Offset coord, Offset index2)
        => new(coord.X - index2.X, coord.Y - index2.Y);

    /// <summary>
    /// 将偏移量的各个分量除以一个整数。
    /// </summary>
    /// <param name="coord">原始偏移量。</param>
    /// <param name="value">除数。</param>
    /// <returns>结果偏移量。</returns>
    public static Offset operator /(Offset coord, int value)
        => new(coord.X / value, coord.Y / value);

    /// <summary>
    /// 将偏移量的各分量乘上一个整数因子。
    /// </summary>
    /// <param name="coord">原始偏移量。</param>
    /// <param name="offset">缩放因子。</param>
    /// <returns>缩放后的新偏移量。</returns>
    public static Offset operator *(Offset coord, int offset)
        => new(coord.X * offset, coord.Y * offset);

    /// <summary>
    /// 检查对象是否与当前偏移量相等。
    /// </summary>
    /// <param name="obj">要比较的对象。</param>
    /// <returns>如果是 Offset 类型且等于当前实例，则返回 true；否则返回 false。</returns>
    public override bool Equals(object obj)
        => obj is Offset other && Equals(other);

    /// <summary>
    /// 比较另一个 Offset 是否与当前实例相等。
    /// </summary>
    /// <param name="other">要比较的其他偏移量。</param>
    /// <returns>如果两者相等则返回 true；否则返回 false。</returns>
    public bool Equals(Offset other) => (X, Y) == (other.X, other.Y);

    /// <summary>
    /// 使用此比较器判断两个 Offset 是否相等。
    /// </summary>
    /// <param name="coord1">第一个偏移量。</param>
    /// <param name="coord2">第二个偏移量。</param>
    /// <returns>如果两个偏移量相等则返回 true；否则返回 false。</returns>
    public bool Equals(Offset coord1, Offset coord2) => coord1.Equals(coord2);

    /// <summary>
    /// 计算当前偏移量的哈希码。
    /// </summary>
    /// <returns>基于 X 和 Y 的元组生成的哈希码。</returns>
    public override int GetHashCode() => (X, Y).GetHashCode();

    /// <summary>
    /// 根据提供的偏移量获取其哈希码。
    /// </summary>
    /// <param name="coord">目标偏移量。</param>
    /// <returns>该偏移量的哈希码。</returns>
    public int GetHashCode(Offset coord) => coord.GetHashCode();

    /// <summary>
    /// 返回当前偏移量的字符串表示形式。
    /// </summary>
    /// <returns>格式为 "O-[X:Y]" 的字符串。</returns>
    public override string ToString() => $"O-[{X}:{Y}]";
}