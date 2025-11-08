using System;
using System.Collections.Generic;

namespace HexagonalLib.Coordinates;

/// <summary>
/// 表示一个立方体坐标系统中的坐标点，使用整数表示 X、Y、Z 坐标。
/// 该结构体是只读的，并实现了 IEquatable 和 IEqualityComparer 接口以支持相等性比较。
/// </summary>
[Serializable]
public readonly partial struct Cubic : IEquatable<Cubic>, IEqualityComparer<Cubic>
{
    /// <summary>
    /// 获取表示零坐标的 Cubic 实例（X=0, Y=0, Z=0）。
    /// </summary>
    public static Cubic Zero => new(0, 0, 0);

    /// <summary>
    /// 获取 X 坐标值。
    /// </summary>
    public readonly int X;

    /// <summary>
    /// 获取 Y 坐标值。
    /// </summary>
    public readonly int Y;

    /// <summary>
    /// 获取 Z 坐标值。
    /// </summary>
    public readonly int Z;

    /// <summary>
    /// 使用指定的整数坐标初始化一个新的 Cubic 实例。
    /// </summary>
    /// <param name="x">X 坐标。</param>
    /// <param name="y">Y 坐标。</param>
    /// <param name="z">Z 坐标。</param>
    public Cubic(int x, int y, int z)
        : this()
    {
        X = x;
        Y = y;
        Z = z;
    }

    /// <summary>
    /// 使用浮点坐标初始化一个新的 Cubic 实例，并将其四舍五入到最近的有效整数坐标。
    /// 在立方体坐标系统中，有效坐标必须满足 X + Y + Z = 0 的约束条件。
    /// </summary>
    /// <param name="x">X 坐标的浮点值。</param>
    /// <param name="y">Y 坐标的浮点值。</param>
    /// <param name="z">Z 坐标的浮点值。</param>
    public Cubic(float x, float y, float z)
    {
        var rx = (int)Math.Round(x);
        var ry = (int)Math.Round(y);
        var rz = (int)Math.Round(z);

        var xDiff = Math.Abs(rx - x);
        var yDiff = Math.Abs(ry - y);
        var zDiff = Math.Abs(rz - z);

        // 根据最大差异调整坐标以满足 X + Y + Z = 0 的约束
        if (xDiff > yDiff && xDiff > zDiff)
        {
            rx = -ry - rz;
        }
        else if (yDiff > zDiff)
        {
            ry = -rx - rz;
        }
        else
        {
            rz = -rx - ry;
        }

        X = rx;
        Y = ry;
        Z = rz;
    }

    /// <summary>
    /// 判断两个 Cubic 实例是否相等。
    /// </summary>
    /// <param name="coord1">要比较的第一个 Cubic 实例。</param>
    /// <param name="coord2">要比较的第二个 Cubic 实例。</param>
    /// <returns>如果两个实例相等，则返回 true；否则返回 false。</returns>
    public static bool operator ==(Cubic coord1, Cubic coord2)
        => (coord1.X, coord1.Y, coord1.Z) == (coord2.X, coord2.Y, coord2.Z);

    /// <summary>
    /// 判断两个 Cubic 实例是否不相等。
    /// </summary>
    /// <param name="coord1">要比较的第一个 Cubic 实例。</param>
    /// <param name="coord2">要比较的第二个 Cubic 实例。</param>
    /// <returns>如果不相等，则返回 true；否则返回 false。</returns>
    public static bool operator !=(Cubic coord1, Cubic coord2)
        => (coord1.X, coord1.Y, coord1.Z) != (coord2.X, coord2.Y, coord2.Z);

    /// <summary>
    /// 对两个 Cubic 实例进行加法运算。
    /// </summary>
    /// <param name="coord1">第一个 Cubic 实例。</param>
    /// <param name="coord2">第二个 Cubic 实例。</param>
    /// <returns>两个坐标相加后的新 Cubic 实例。</returns>
    public static Cubic operator +(Cubic coord1, Cubic coord2)
        => new(coord1.X + coord2.X, coord1.Y + coord2.Y, coord1.Z + coord2.Z);

    /// <summary>
    /// 将 Cubic 实例的所有坐标加上一个偏移量。
    /// </summary>
    /// <param name="coord">原始 Cubic 实例。</param>
    /// <param name="offset">要添加的整数偏移量。</param>
    /// <returns>加上偏移量后的新 Cubic 实例。</returns>
    public static Cubic operator +(Cubic coord, int offset)
        => new(coord.X + offset, coord.Y + offset, coord.Z + offset);

    /// <summary>
    /// 对两个 Cubic 实例进行减法运算。
    /// </summary>
    /// <param name="coord1">第一个 Cubic 实例。</param>
    /// <param name="coord2">第二个 Cubic 实例。</param>
    /// <returns>两个坐标相减后的新 Cubic 实例。</returns>
    public static Cubic operator -(Cubic coord1, Cubic coord2)
        => new(coord1.X - coord2.X, coord1.Y - coord2.Y, coord1.Z - coord2.Z);

    /// <summary>
    /// 将 Cubic 实例的所有坐标减去一个偏移量。
    /// </summary>
    /// <param name="coord">原始 Cubic 实例。</param>
    /// <param name="offset">要减去的整数偏移量。</param>
    /// <returns>减去偏移量后的新 Cubic 实例。</returns>
    public static Cubic operator -(Cubic coord, int offset)
        => new(coord.X - offset, coord.Y - offset, coord.Z - offset);

    /// <summary>
    /// 将 Cubic 实例的所有坐标乘以一个整数倍数。
    /// </summary>
    /// <param name="coord">原始 Cubic 实例。</param>
    /// <param name="offset">用于缩放的整数倍数。</param>
    /// <returns>乘以倍数后的新 Cubic 实例。</returns>
    public static Cubic operator *(Cubic coord, int offset)
        => new(coord.X * offset, coord.Y * offset, coord.Z * offset);

    /// <summary>
    /// 将 Cubic 实例的所有坐标乘以一个浮点数倍数。
    /// </summary>
    /// <param name="coord">原始 Cubic 实例。</param>
    /// <param name="delta">用于缩放的浮点数倍数。</param>
    /// <returns>乘以倍数后的新 Cubic 实例。</returns>
    public static Cubic operator *(Cubic coord, float delta)
        => new(coord.X * delta, coord.Y * delta, coord.Z * delta);

    /// <summary>
    /// 检查当前坐标是否为有效的立方体坐标（即满足 X + Y + Z == 0）。
    /// </summary>
    /// <returns>如果坐标有效，则返回 true；否则返回 false。</returns>
    public bool IsValid() => X + Y + Z == 0;

    /// <summary>
    /// 将当前坐标向右旋转一次（在立方体坐标系统中，相当于交换并取反某些坐标）。
    /// </summary>
    /// <returns>旋转后的新 Cubic 实例。</returns>
    public Cubic RotateToRight()
    {
        var x = -Y;
        var y = -Z;
        var z = -X;
        return new Cubic(x, y, z);
    }

    /// <summary>
    /// 将当前坐标向右旋转指定次数。
    /// </summary>
    /// <param name="times">旋转的次数。</param>
    /// <returns>旋转后的新 Cubic 实例。</returns>
    public Cubic RotateToRight(int times)
    {
        var cur = this;
        for (var i = 0; i < times; i++)
        {
            cur = cur.RotateToRight();
        }

        return cur;
    }

    /// <summary>
    /// 确定当前实例是否等于指定对象。
    /// </summary>
    /// <param name="obj">要与当前实例比较的对象。</param>
    /// <returns>如果对象是 Cubic 类型且与当前实例相等，则返回 true；否则返回 false。</returns>
    public override bool Equals(object obj) => obj is Cubic other && Equals(other);

    /// <summary>
    /// 确定当前实例是否等于另一个 Cubic 实例。
    /// </summary>
    /// <param name="other">要与当前实例比较的 Cubic 实例。</param>
    /// <returns>如果两个实例相等，则返回 true；否则返回 false。</returns>
    public bool Equals(Cubic other) => (X, Y, Z) == (other.X, other.Y, other.Z);

    /// <summary>
    /// 确定两个 Cubic 实例是否相等。
    /// </summary>
    /// <param name="coord1">要比较的第一个 Cubic 实例。</param>
    /// <param name="coord2">要比较的第二个 Cubic 实例。</param>
    /// <returns>如果两个实例相等，则返回 true；否则返回 false。</returns>
    public bool Equals(Cubic coord1, Cubic coord2) => coord1.Equals(coord2);

    /// <summary>
    /// 返回当前实例的哈希代码。
    /// </summary>
    /// <returns>当前实例的哈希代码。</returns>
    public override int GetHashCode() => (X, Y, Z).GetHashCode();

    /// <summary>
    /// 返回指定 Cubic 实例的哈希代码。
    /// </summary>
    /// <param name="coord">要获取哈希代码的 Cubic 实例。</param>
    /// <returns>指定实例的哈希代码。</returns>
    public int GetHashCode(Cubic coord) => coord.GetHashCode();

    /// <summary>
    /// 返回当前实例的字符串表示形式。
    /// 如果坐标无效，则显示 "C-[Invalid]"；否则显示格式为 "C-[X:Y:Z]"。
    /// </summary>
    /// <returns>当前实例的字符串表示。</returns>
    public override string ToString() => !IsValid() ? "C-[Invalid]" : $"C-[{X}:{Y}:{Z}]";
}