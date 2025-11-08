using System;

namespace HexagonalLib;

/// <summary>
/// 提供用于六边形网格和二维向量运算的静态数学辅助方法。
/// 这些方法以扩展方法形式定义，返回或比较二维向量（以 (X, Y) 元组表示）。
/// </summary>
public static class HexagonalMath
{
    extension(in (float X, float Y) source)
    {
        /// <summary>
        /// 绕 Z 轴顺时针旋转二维向量。
        /// </summary>
        /// <param name="vector">要旋转的二维向量，表示为 (X, Y)。</param>
        /// <param name="degrees">旋转角度（以度为单位），按顺时针方向旋转。</param>
        /// <returns>
        /// 返回旋转后的向量，类型为 (float X, float Y)。
        /// 注意：实现中对中间计算的结果进行了 6 位小数的四舍五入，且最终返回的 X 分量带有一次取反（实现上返回 (-x, y)）。
        /// </returns>
        /// <remarks>
        /// 该方法将角度转换为弧度（radians = PI / 180 * degrees），
        /// 使用 Math.Sin/Math.Cos 计算旋转矩阵，并对计算结果舍入到 6 位小数以减少浮点噪声。
        /// 方法本身不会抛出异常；对于极大或极小的输入可能会产生浮点精度误差。
        /// </remarks>
        public (float X, float Y) Rotate(float degrees)
        {
            var radians = Math.PI / 180.0 * degrees;
            var sin = Math.Sin(radians);
            var cos = Math.Cos(radians);
            var x = (float)Math.Round(cos * source.X - sin * source.Y, 6);
            var y = (float)Math.Round(sin * source.X + cos * source.Y, 6);
            return (-x, y);
        }

        /// <summary>
        /// 将给定二维向量缩放为单位向量（长度为 1）。
        /// </summary>
        /// <param name="vector">要归一化的二维向量，表示为 (X, Y)。</param>
        /// <returns>
        /// 返回长度为 1 的向量（单位向量），类型为 (float X, float Y)。
        /// </returns>
        /// <remarks>
        /// 计算向量的长度为 sqrt(X*X + Y*Y)，然后将每个分量除以该长度。
        /// 如果输入向量的长度为 0，则会发生除以零的情况，结果可能为 <see cref="double.NaN"/> 或 <see cref="double.PositiveInfinity"/>（在转换为 float 后为 NaN/Infinity）。
        /// 该方法不在内部处理零向量的特殊情况：调用方应在需要时先检查长度以避免不期望的值。
        /// </remarks>
        public (float X, float Y) Normalize()
        {
            var distance = Math.Sqrt(source.X * source.X + source.Y * source.Y);
            return ((float)(source.X / distance), (float)(source.Y / distance));
        }

        /// <summary>
        /// 比较两个二维向量在每个分量上是否相似（使用 <see cref="SimilarTo(float, float)"/>）。
        /// </summary>
        /// <param name="a">第一个二维向量，表示为 (X, Y)。</param>
        /// <param name="b">第二个二维向量，表示为 (X, Y)。</param>
        /// <returns>当且仅当两个向量的 X 分量和 Y 分量各自都与对应分量相似时返回 true。</returns>
        /// <remarks>
        /// 对向量比较使用分量级比较：分别比较 X 与 X、Y 与 Y，并且都满足 <see cref="SimilarTo(float, float)"/> 时才视为相似。
        /// 这种比较方式适合在存在浮点舍入误差时判断向量是否“足够接近”。
        /// </remarks>
        public bool SimilarTo(in (float X, float Y) b)
            => source.X.SimilarTo(b.X) && source.Y.SimilarTo(b.Y);
    }


    extension(float sourceA)
    {
        /// <summary>
        /// 比较两个浮点数，判断它们在容差范围内是否相似。
        /// </summary>
        /// <param name="sourceB">要比较的第二个浮点数。</param>
        /// <returns>如果两者在容差范围内则返回 true，否则返回 false。</returns>
        /// <remarks>
        /// 使用复合容差：取相对容差 1e-6 * max(|a|, |b|) 与绝对容差 float.Epsilon * 8f 中的较大值作为阈值，
        /// 然后比较 |b - a| 与该阈值。此实现适用于大多数常见浮点比较场景，兼顾相对误差和绝对误差。
        /// </remarks>
        public bool SimilarTo(float sourceB)
            => Math.Abs(sourceB - sourceA) < (double)Math.Max(1E-06f * Math.Max(Math.Abs(sourceA), Math.Abs(sourceB)), float.Epsilon * 8f);
    }
}