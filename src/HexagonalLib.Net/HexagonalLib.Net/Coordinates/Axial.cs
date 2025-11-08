using System;
using System.Collections.Generic;

namespace HexagonalLib.Coordinates;

/// <summary>
/// 表示一个轴向坐标系统中的坐标点，使用 Q 和 R 两个整数分量表示。
/// 该结构体是只读的，并支持序列化。
/// </summary>
/// <param name="q">Q 坐标分量</param>
/// <param name="r">R 坐标分量</param>
[Serializable]
public readonly partial struct Axial(int q, int r) : IEquatable<Axial>, IEqualityComparer<Axial>
{
    /// <summary>
    /// 获取坐标原点 (0, 0) 的 Axial 实例。
    /// </summary>
    public static Axial Zero => new(0, 0);

    /// <summary>
    /// Q 坐标分量（只读）。
    /// </summary>
    public readonly int Q = q;

    /// <summary>
    /// R 坐标分量（只读）。
    /// </summary>
    public readonly int R = r;

    /// <summary>
    /// 判断两个 Axial 坐标是否相等。
    /// </summary>
    /// <param name="coord1">第一个坐标</param>
    /// <param name="coord2">第二个坐标</param>
    /// <returns>如果两个坐标相等则返回 true，否则返回 false</returns>
    public static bool operator ==(Axial coord1, Axial coord2)
        => (coord1.Q, coord1.R) == (coord2.Q, coord2.R);

    /// <summary>
    /// 判断两个 Axial 坐标是否不相等。
    /// </summary>
    /// <param name="coord1">第一个坐标</param>
    /// <param name="coord2">第二个坐标</param>
    /// <returns>如果两个坐标不相等则返回 true，否则返回 false</returns>
    public static bool operator !=(Axial coord1, Axial coord2)
        => (coord1.Q, coord1.R) != (coord2.Q, coord2.R);

    /// <summary>
    /// 将两个 Axial 坐标相加。
    /// </summary>
    /// <param name="coord1">第一个坐标</param>
    /// <param name="coord2">第二个坐标</param>
    /// <returns>相加后的结果坐标</returns>
    public static Axial operator +(Axial coord1, Axial coord2)
        => new(coord1.Q + coord2.Q, coord1.R + coord2.R);

    /// <summary>
    /// 将 Axial 坐标的每个分量加上一个整数偏移量。
    /// </summary>
    /// <param name="coord">原始坐标</param>
    /// <param name="offset">要加上的整数偏移量</param>
    /// <returns>加上偏移后的结果坐标</returns>
    public static Axial operator +(Axial coord, int offset)
        => new(coord.Q + offset, coord.R + offset);

    /// <summary>
    /// 将两个 Axial 坐标相减。
    /// </summary>
    /// <param name="coord1">第一个坐标</param>
    /// <param name="coord2">第二个坐标</param>
    /// <returns>相减后的结果坐标</returns>
    public static Axial operator -(Axial coord1, Axial coord2)
        => new(coord1.Q - coord2.Q, coord1.R - coord2.R);

    /// <summary>
    /// 将 Axial 坐标的每个分量减去一个整数偏移量。
    /// </summary>
    /// <param name="coord">原始坐标</param>
    /// <param name="offset">要减去的整数偏移量</param>
    /// <returns>减去偏移后的结果坐标</returns>
    public static Axial operator -(Axial coord, int offset)
        => new(coord.Q - offset, coord.R - offset);

    /// <summary>
    /// 将 Axial 坐标的每个分量乘以一个整数倍数。
    /// </summary>
    /// <param name="coord">原始坐标</param>
    /// <param name="offset">要乘的整数倍数</param>
    /// <returns>乘以倍数后的结果坐标</returns>
    public static Axial operator *(Axial coord, int offset)
        => new(coord.Q * offset, coord.R * offset);

    /// <summary>
    /// 判断当前对象是否与另一个对象相等。
    /// </summary>
    /// <param name="other">要比较的对象</param>
    /// <returns>如果相等则返回 true，否则返回 false</returns>
    public override bool Equals(object other)
        => other is Axial axial && Equals(axial);

    /// <summary>
    /// 判断当前 Axial 坐标是否与另一个 Axial 坐标相等。
    /// </summary>
    /// <param name="other">要比较的另一个坐标</param>
    /// <returns>如果相等则返回 true，否则返回 false</returns>
    public bool Equals(Axial other) => (Q, R) == (other.Q, other.R);

    /// <summary>
    /// 比较两个 Axial 坐标是否相等。
    /// </summary>
    /// <param name="coord1">第一个坐标</param>
    /// <param name="coord2">第二个坐标</param>
    /// <returns>如果两个坐标相等则返回 true，否则返回 false</returns>
    public bool Equals(Axial coord1, Axial coord2) => coord1.Equals(coord2);

    /// <summary>
    /// 返回当前 Axial 坐标的哈希值。
    /// </summary>
    /// <returns>表示当前坐标的哈希码</returns>
    public override int GetHashCode() => (Q, R).GetHashCode();

    /// <summary>
    /// 获取指定 Axial 坐标的哈希值。
    /// </summary>
    /// <param name="axial">要获取哈希值的坐标</param>
    /// <returns>表示该坐标的哈希码</returns>
    public int GetHashCode(Axial axial) => axial.GetHashCode();

    /// <summary>
    /// 返回当前 Axial 坐标的字符串表示形式。
    /// 格式为 "A-[Q:R]"。
    /// </summary>
    /// <returns>格式化的字符串表示</returns>
    public override string ToString() => $"A-[{Q}:{R}]";
}