using System;
using System.Collections.Generic;
using HexagonalLib.Coordinates;
using static System.Math;

namespace HexagonalLib;

/// <summary>
/// 表示一个六边形网格结构体
/// </summary>
/// <remarks>
/// 这是一个只读的部分结构体，用于表示六边形网格数据结构
/// </remarks>
public readonly partial struct HexagonalGrid
{
    /// <summary>
    /// Total count of edges in one Hex
    /// </summary>
    public const int EDGES_COUNT = 6;

    /// <summary>
    /// 根号3的常量值，用于六边形几何计算
    /// </summary>
    public static readonly float Sqrt3 = (float)Sqrt(3);

    /// <summary>
    /// 内切圆半径，即从六边形中心到边的垂直距离
    /// </summary>
    public readonly float InscribedRadius;

    /// <summary>
    /// 外接圆半径，即从六边形中心到顶点的距离
    /// </summary>
    public readonly float DescribedRadius;

    /// <summary>
    /// 获取六边形的边长，等于外接圆半径
    /// </summary>
    public float Side => DescribedRadius;

    /// <summary>
    /// 获取六边形的内切圆直径，即内切圆半径的两倍
    /// </summary>
    public float InscribedDiameter => InscribedRadius * 2;

    /// <summary>
    /// 获取六边形的外接圆直径，即外接圆半径的两倍
    /// </summary>
    public float DescribedDiameter => DescribedRadius * 2;

    /// <summary>
    /// 网格的方向和布局类型
    /// </summary>
    public readonly HexagonalGridType Type;

    /// <summary>
    /// 获取当前六边形网格中，一个六边形与其右侧邻居在X轴上的偏移量。
    /// 对于点朝上（Pointy）类型的网格，该值等于内切圆直径；
    /// 对于面朝上（Flat）类型的网格，该值等于外接圆半径的1.5倍。
    /// </summary>
    /// <exception cref="HexagonalException">当网格类型未定义时抛出异常。</exception>
    public float HorizontalOffset
    {
        get
        {
            switch (Type)
            {
                case HexagonalGridType.PointyOdd:
                case HexagonalGridType.PointyEven:
                    return InscribedRadius * 2.0f;
                case HexagonalGridType.FlatOdd:
                case HexagonalGridType.FlatEven:
                    return DescribedRadius * 1.5f;
                default:
                    throw new HexagonalException($"Can't get {nameof(HorizontalOffset)} with unexpected {nameof(Type)}", this);
            }
        }
    }

    /// <summary>
    /// 获取当前六边形网格中，一个六边形与其上方邻居在Y轴上的偏移量。
    /// 对于点朝上（Pointy）类型的网格，该值等于外接圆半径的1.5倍；
    /// 对于面朝上（Flat）类型的网格，该值等于内切圆直径。
    /// </summary>
    /// <exception cref="HexagonalException">当网格类型未定义时抛出异常。</exception>
    public float VerticalOffset
    {
        get
        {
            switch (Type)
            {
                case HexagonalGridType.PointyOdd:
                case HexagonalGridType.PointyEven:
                    return DescribedRadius * 1.5f;
                case HexagonalGridType.FlatOdd:
                case HexagonalGridType.FlatEven:
                    return InscribedRadius * 2.0f;
                default:
                    throw new HexagonalException($"Can't get {nameof(VerticalOffset)} with unexpected {nameof(Type)}", this);
            }
        }
    }

    /// <summary>
    /// 获取任意一个六边形中心与其第一个邻居中心之间的角度，
    /// 相对于向量(0, 1)按顺时针方向计算。
    /// 点朝上的网格返回0度；面朝上的网格返回30度。
    /// </summary>
    /// <exception cref="HexagonalException">当网格类型未定义时抛出异常。</exception>
    public float AngleToFirstNeighbor
    {
        get
        {
            switch (Type)
            {
                case HexagonalGridType.PointyOdd:
                case HexagonalGridType.PointyEven:
                    return 0.0f;
                case HexagonalGridType.FlatOdd:
                case HexagonalGridType.FlatEven:
                    return 30.0f;
                default:
                    throw new HexagonalException($"Can't get {nameof(AngleToFirstNeighbor)} with unexpected {nameof(Type)}", this);
            }
        }
    }

    /// <summary>
    /// 初始化一个新的六边形网格实例。
    /// 根据给定的网格类型和内切圆半径计算并设置相关属性。
    /// </summary>
    /// <param name="type">指定六边形网格的方向与布局方式。</param>
    /// <param name="radius">六边形的内切圆半径。</param>
    public HexagonalGrid(HexagonalGridType type, float radius)
    {
        Type = type;
        InscribedRadius = radius;
        DescribedRadius = (float)(radius / Cos(PI / EDGES_COUNT));
    }

    #region ToOffset

    /// <summary>
    /// 将立方坐标转换为偏移坐标。
    /// 转换规则根据当前六边形网格的类型（点朝上或面朝上、奇数行优先或偶数行优先）进行相应处理。
    /// </summary>
    /// <param name="coord">要转换的立方坐标。</param>
    /// <returns>对应的偏移坐标。</returns>
    /// <exception cref="HexagonalException">当网格类型未定义时抛出异常。</exception>
    public Offset ToOffset(Cubic coord)
    {
        switch (Type)
        {
            case HexagonalGridType.PointyOdd:
            {
                var col = coord.X + (coord.Z - (coord.Z & 1)) / 2;
                var row = coord.Z;
                return new Offset(col, row);
            }
            case HexagonalGridType.PointyEven:
            {
                var col = coord.X + (coord.Z + (coord.Z & 1)) / 2;
                var row = coord.Z;
                return new Offset(col, row);
            }
            case HexagonalGridType.FlatOdd:
            {
                var col = coord.X;
                var row = coord.Z + (coord.X - (coord.X & 1)) / 2;
                return new Offset(col, row);
            }
            case HexagonalGridType.FlatEven:
            {
                var col = coord.X;
                var row = coord.Z + (coord.X + (coord.X & 1)) / 2;
                return new Offset(col, row);
            }
            default:
                throw new HexagonalException($"{nameof(ToOffset)} failed with unexpected {nameof(Type)}", this, (nameof(coord), coord));
        }
    }

    /// <summary>
    /// 将轴向坐标转换为偏移坐标。
    /// 先将轴向坐标转为立方坐标，再调用立方坐标到偏移坐标的转换方法。
    /// </summary>
    /// <param name="axial">要转换的轴向坐标。</param>
    /// <returns>对应的偏移坐标。</returns>
    public Offset ToOffset(Axial axial) => ToOffset(ToCubic(axial));

    /// <summary>
    /// 计算包含指定平面坐标的六边形单元格，并返回其偏移坐标。
    /// 先将平面坐标转为立方坐标，再进一步转换为偏移坐标。
    /// </summary>
    /// <param name="x">点的X坐标。</param>
    /// <param name="y">点的Y坐标。</param>
    /// <returns>包含该点的六边形单元格的偏移坐标。</returns>
    public Offset ToOffset(float x, float y) => ToOffset(ToCubic(x, y));

    /// <summary>
    /// 计算包含指定平面坐标的六边形单元格，并返回其偏移坐标。
    /// 使用元组形式传入坐标参数。
    /// </summary>
    /// <param name="point">表示点坐标的元组(X,Y)。</param>
    /// <returns>包含该点的六边形单元格的偏移坐标。</returns>
    public Offset ToOffset((float X, float Y) point) => ToOffset(ToCubic(point.X, point.Y));

    #endregion

    #region ToAxial

    /// <summary>
    /// 将立方坐标转换为轴向坐标。
    /// 轴向坐标由立方坐标的X和Z分量组成。
    /// </summary>
    /// <param name="cubic">要转换的立方坐标。</param>
    /// <returns>对应的轴向坐标。</returns>
    public Axial ToAxial(Cubic cubic) => new(cubic.X, cubic.Z);

    /// <summary>
    /// 将偏移坐标转换为轴向坐标。
    /// 先将偏移坐标转换为立方坐标，再将其转换为轴向坐标。
    /// </summary>
    /// <param name="offset">要转换的偏移坐标。</param>
    /// <returns>对应的轴向坐标。</returns>
    public Axial ToAxial(Offset offset) => ToAxial(ToCubic(offset));

    /// <summary>
    /// 根据给定的二维点坐标，计算其所在的六边形的轴向坐标。
    /// 先将点坐标转换为立方坐标，再将其转换为轴向坐标。
    /// </summary>
    /// <param name="x">点的X坐标。</param>
    /// <param name="y">点的Y坐标。</param>
    /// <returns>包含该点的六边形的轴向坐标。</returns>
    public Axial ToAxial(float x, float y) => ToAxial(ToCubic(x, y));

    /// <summary>
    /// 根据给定的二维点坐标元组，计算其所在的六边形的轴向坐标。
    /// 先将点坐标转换为立方坐标，再将其转换为轴向坐标。
    /// </summary>
    /// <param name="point">表示点坐标的元组(X, Y)。</param>
    /// <returns>包含该点的六边形的轴向坐标。</returns>
    public Axial ToAxial((float X, float Y) point) => ToAxial(ToCubic(point.X, point.Y));

    #endregion

    #region ToCubic

    /// <summary>
    /// 将偏移坐标转换为立方坐标。
    /// 不同类型的网格使用不同的转换公式。
    /// </summary>
    /// <param name="coord">要转换的偏移坐标。</param>
    /// <returns>对应的立方坐标。</returns>
    public Cubic ToCubic(Offset coord)
    {
        switch (Type)
        {
            // Pointy方向奇数行排列：X方向偏移需要根据Y坐标的奇偶性调整
            case HexagonalGridType.PointyOdd:
            {
                var x = coord.X - (coord.Y - (coord.Y & 1)) / 2;
                var z = coord.Y;
                var y = -x - z;
                return new Cubic(x, y, z);
            }
            // Pointy方向偶数行排列：X方向偏移需要根据Y坐标的奇偶性调整
            case HexagonalGridType.PointyEven:
            {
                var x = coord.X - (coord.Y + (coord.Y & 1)) / 2;
                var z = coord.Y;
                var y = -x - z;
                return new Cubic(x, y, z);
            }
            // Flat方向奇数列排列：Z方向偏移需要根据X坐标的奇偶性调整
            case HexagonalGridType.FlatOdd:
            {
                var x = coord.X;
                var z = coord.Y - (coord.X - (coord.X & 1)) / 2;
                var y = -x - z;
                return new Cubic(x, y, z);
            }
            // Flat方向偶数列排列：Z方向偏移需要根据X坐标的奇偶性调整
            case HexagonalGridType.FlatEven:
            {
                var x = coord.X;
                var z = coord.Y - (coord.X + (coord.X & 1)) / 2;
                var y = -x - z;
                return new Cubic(x, y, z);
            }
            default:
                throw new HexagonalException($"{nameof(ToCubic)} failed with unexpected {nameof(Type)}", this, (nameof(coord), coord));
        }
    }

    /// <summary>
    /// 将轴向坐标转换为立方坐标。
    /// 利用立方坐标三个分量之和为零的特性进行转换。
    /// </summary>
    /// <param name="axial">要转换的轴向坐标。</param>
    /// <returns>对应的立方坐标。</returns>
    public Cubic ToCubic(Axial axial) => new(axial.Q, -axial.Q - axial.R, axial.R);

    /// <summary>
    /// 根据给定的二维点坐标，计算其所在的六边形的立方坐标。
    /// 使用数学方法将平面直角坐标映射到六边形网格坐标系统中。
    /// </summary>
    /// <param name="x">点的X坐标。</param>
    /// <param name="y">点的Y坐标。</param>
    /// <returns>包含该点的六边形的立方坐标。</returns>
    public Cubic ToCubic(float x, float y)
    {
        switch (Type)
        {
            // Pointy方向（上下尖）布局处理
            case HexagonalGridType.PointyOdd:
            case HexagonalGridType.PointyEven:
            {
                var q = (x * Sqrt3 / 3.0f - y / 3.0f) / Side;
                var r = y * 2.0f / 3.0f / Side;
                return new Cubic(q, -q - r, r);
            }
            // Flat方向（左右尖）布局处理
            case HexagonalGridType.FlatOdd:
            case HexagonalGridType.FlatEven:
            {
                var q = x * 2.0f / 3.0f / Side;
                var r = (-x / 3.0f + Sqrt3 / 3.0f * y) / Side;
                return new Cubic(q, -q - r, r);
            }
            default:
                throw new HexagonalException($"{nameof(ToCubic)} failed with unexpected {nameof(Type)}", this, (nameof(x), x), (nameof(y), y));
        }
    }

    /// <summary>
    /// 根据给定的二维点坐标元组，计算其所在的六边形的立方坐标。
    /// 使用数学方法将平面直角坐标映射到六边形网格坐标系统中。
    /// </summary>
    /// <param name="point">表示点坐标的元组(X, Y)。</param>
    /// <returns>包含该点的六边形的立方坐标。</returns>
    public Cubic ToCubic((float X, float Y) point) => ToCubic(point.X, point.Y);

    #endregion

    #region ToPoint2

    /// <summary>
    /// 将基于偏移坐标的六边形转换为其在二维空间中的中心位置。
    /// 首先将偏移坐标转换为轴向坐标，然后获取对应的位置。
    /// </summary>
    /// <param name="coord">六边形的偏移坐标。</param>
    /// <returns>六边形中心点的二维坐标(X, Y)。</returns>
    public (float X, float Y) ToPoint2(Offset coord)
        => ToPoint2(ToAxial(coord));

    /// <summary>
    /// 将基于轴向坐标的六边形转换为其在二维空间中的中心位置
    /// </summary>
    /// <param name="coord">六边形的轴向坐标</param>
    /// <returns>二维空间中的中心位置，格式为(X, Y)</returns>
    public (float X, float Y) ToPoint2(Axial coord)
    {
        // 根据不同的网格类型计算六边形中心点的二维坐标
        switch (Type)
        {
            case HexagonalGridType.PointyOdd:
            case HexagonalGridType.PointyEven:
            {
                var x = Side * (Sqrt3 * coord.Q + Sqrt3 / 2 * coord.R);
                var y = Side * (3.0f / 2.0f * coord.R);
                return (x, y);
            }
            case HexagonalGridType.FlatOdd:
            case HexagonalGridType.FlatEven:
            {
                var x = Side * (3.0f / 2.0f * coord.Q);
                var y = Side * (Sqrt3 / 2 * coord.Q + Sqrt3 * coord.R);
                return (x, y);
            }
            default:
                throw new HexagonalException($"{nameof(ToPoint2)} failed with unexpected {nameof(Type)}", this, (nameof(coord), coord));
        }
    }

    /// <summary>
    /// 将基于立方坐标的六边形转换为其在二维空间中的中心位置
    /// </summary>
    /// <param name="coord">六边形的立方坐标</param>
    /// <returns>二维空间中的中心位置，格式为(X, Y)</returns>
    public (float X, float Y) ToPoint2(Cubic coord)
        => ToPoint2(ToAxial(coord));

    #endregion

    #region GetCornerPoint

    /// <summary>
    /// 获取偏移坐标下指定边缘编号的角点在二维空间中的坐标
    /// </summary>
    /// <param name="coord">六边形的偏移坐标</param>
    /// <param name="edge">边缘索引（0-5），表示六边形的六条边之一</param>
    /// <returns>角点在二维空间中的（X,Y）坐标</returns>
    public (float X, float Y) GetCornerPoint(Offset coord, int edge)
        => GetCornerPoint(coord, edge, ToPoint2);

    /// <summary>
    /// 获取轴向坐标下指定边缘编号的角点在二维空间中的坐标
    /// </summary>
    /// <param name="coord">六边形的轴向坐标</param>
    /// <param name="edge">边缘索引（0-5），表示六边形的六条边之一</param>
    /// <returns>角点在二维空间中的（X,Y）坐标</returns>
    public (float X, float Y) GetCornerPoint(Axial coord, int edge)
        => GetCornerPoint(coord, edge, ToPoint2);

    /// <summary>
    /// 获取立方坐标下指定边缘编号的角点在二维空间中的坐标
    /// </summary>
    /// <param name="coord">六边形的立方坐标</param>
    /// <param name="edge">边缘索引（0-5），表示六边形的六条边之一</param>
    /// <returns>角点在二维空间中的（X,Y）坐标</returns>
    public (float X, float Y) GetCornerPoint(Cubic coord, int edge)
        => GetCornerPoint(coord, edge, ToPoint2);

    /// <summary>
    /// 计算指定六边形坐标的指定边缘的角点在2D空间中的坐标
    /// </summary>
    /// <typeparam name="T">六边形坐标类型（如Offset、Axial或Cubic）</typeparam>
    /// <param name="coord">六边形的坐标</param>
    /// <param name="edge">边缘索引（0-5），表示六边形的六条边之一，会被自动归一化到有效范围</param>
    /// <param name="toPoint">将六边形坐标转换为其中心在2D空间中坐标的函数</param>
    /// <returns>角点在2D空间中的（X,Y）坐标</returns>
    /// <remarks>
    /// 方法会先将边缘索引归一化（确保在 0-5 范围内），然后根据网格类型（Pointy或Flat）调整角度，
    /// 最终结合六边形中心坐标、外接圆半径和三角函数计算出角点位置。
    /// </remarks>
    private (float X, float Y) GetCornerPoint<T>(T coord, int edge, Func<T, (float X, float Y)> toPoint)
        where T : struct
    {
        // 归一化边缘索引以保证其处于合法范围内
        edge = NormalizeIndex(edge);

        // 计算当前角点相对于正右方向的角度（单位：度）
        var angleDeg = 60 * edge;

        // 对于尖顶布局的网格，需要额外旋转30度来匹配图形方向
        if (Type is HexagonalGridType.PointyEven or HexagonalGridType.PointyOdd)
            angleDeg -= 30;

        // 获取六边形中心点坐标
        var center = toPoint(coord);

        // 将角度从度数转为弧度并使用三角函数计算角点坐标
        var angleRad = PI / 180 * angleDeg;
        var x = (float)(center.X + DescribedRadius * Cos(angleRad));
        var y = (float)(center.Y + DescribedRadius * Sin(angleRad));

        return (x, y);
    }

    #endregion

    #region GetNeighbor

    /// <summary>
    /// 获取指定偏移坐标的六边形在特定方向上的相邻六边形坐标
    /// </summary>
    /// <param name="coord">源六边形的偏移坐标</param>
    /// <param name="neighborIndex">邻居方向索引（0-5），表示六个可能的相邻方向</param>
    /// <returns>指定方向上相邻六边形的偏移坐标</returns>
    /// <remarks>
    /// 方法会先通过<see cref="GetNeighborsOffsets(Offset)"/>获取该坐标对应的邻居偏移量表，
    /// 然后将邻居索引通过<see cref="NormalizeIndex(int)"/>归一化（确保在0-5范围内），
    /// 最后将源坐标与对应方向的偏移量相加，得到相邻六边形的坐标。
    /// 六个方向按照顺时针顺序排列，方向定义取决于六边形网格的类型（Pointy或Flat）。
    /// </remarks>
    public Offset GetNeighbor(Offset coord, int neighborIndex)
        => coord + GetNeighborsOffsets(coord)[NormalizeIndex(neighborIndex)];

    /// <summary>
    /// 获取指定轴向坐标的六边形在特定方向上的相邻六边形坐标
    /// </summary>
    /// <param name="coord">源六边形的轴向坐标</param>
    /// <param name="neighborIndex">邻居方向索引（0-5），表示六个可能的相邻方向</param>
    /// <returns>指定方向上相邻六边形的轴向坐标</returns>
    /// <remarks>
    /// 方法会先将邻居索引归一化（确保在 0-5 范围内），然后通过从预定义的轴向邻居偏移量数组中获取对应方向的偏移量，
    /// 最后将该偏移量与源坐标相加，得到相邻六边形的坐标。
    /// 六个方向按照顺时针顺序排列，方向定义取决于六边形网格的类型（Pointy或Flat）。
    /// </remarks>
    public Axial GetNeighbor(Axial coord, int neighborIndex)
        => coord + _sAxialNeighbors[NormalizeIndex(neighborIndex)];

    /// <summary>
    /// 获取指定立体坐标的六边形在特定方向上的相邻六边形坐标
    /// </summary>
    /// <param name="coord">源六边形的立体坐标</param>
    /// <param name="neighborIndex">邻居方向索引（0-5），表示六个可能的相邻方向</param>
    /// <returns>指定方向上相邻六边形的立体坐标</returns>
    /// <remarks>
    /// 方法会先将邻居索引归一化（确保在 0-5 范围内），然后通过从预定义的立体坐标邻居偏移量数组中获取对应方向的偏移量，
    /// 最后将该偏移量与源坐标相加，得到相邻六边形的坐标。
    /// 六个方向按照顺时针顺序排列，方向定义取决于六边形网格的类型（Pointy或Flat）。
    /// </remarks>
    public Cubic GetNeighbor(Cubic coord, int neighborIndex)
        => coord + _sCubicNeighbors[NormalizeIndex(neighborIndex)];

    #endregion

    #region GetNeighbors

    /// <summary>
    /// 获取指定偏移坐标的六边形的所有相邻六边形坐标
    /// </summary>
    /// <param name="hex">源六边形的偏移坐标</param>
    /// <returns>包含六个相邻六边形偏移坐标的可枚举集合，按顺时针顺序排列</returns>
    /// <remarks>
    /// 方法通过调用<see cref="GetNeighborsOffsets(Offset)"/>获取适用于当前网格类型和坐标奇偶性的邻居偏移量表，
    /// 然后遍历每个偏移量并与源坐标相加，生成并返回所有相邻六边形的坐标。
    /// 
    /// 邻居偏移量的选择逻辑基于网格类型和坐标的奇偶性：
    /// <list type="bullet">
    ///   <item><b>PointyOdd网格类型</b>：根据Y坐标的奇偶性选择<see cref="_sPointyOddNeighbors"/>或<see cref="_sPointyEvenNeighbors"/></item>
    ///   <item><b>PointyEven网格类型</b>：根据Y坐标的奇偶性选择<see cref="_sPointyEvenNeighbors"/>或<see cref="_sPointyOddNeighbors"/></item>
    ///   <item><b>FlatOdd网格类型</b>：根据X坐标的奇偶性选择<see cref="_sFlatOddNeighbors"/>或<see cref="_sFlatEvenNeighbors"/></item>
    ///   <item><b>FlatEven网格类型</b>：根据X坐标的奇偶性选择<see cref="_sFlatEvenNeighbors"/>或<see cref="_sFlatOddNeighbors"/></item>
    /// </list>
    /// 
    /// 六个邻居按照顺时针顺序排列，具体方向定义取决于所使用的偏移量表。
    /// 该方法使用yield return实现懒加载，仅在需要时才计算邻居坐标。
    /// </remarks>
    public IEnumerable<Offset> GetNeighbors(Offset hex)
    {
        foreach (var offset in GetNeighborsOffsets(hex))
            yield return offset + hex;
    }

    /// <summary>
    /// 获取指定轴向坐标的六边形的所有相邻六边形坐标
    /// </summary>
    /// <param name="hex">源六边形的轴向坐标</param>
    /// <returns>包含六个相邻六边形轴向坐标的可枚举集合，按顺时针顺序排列</returns>
    /// <remarks>
    /// 方法通过遍历预定义的轴向邻居偏移量数组<see cref="_sAxialNeighbors"/>，
    /// 将每个偏移量与源坐标相加，生成并返回所有相邻六边形的坐标。
    /// 六个邻居按照顺时针顺序排列，方向定义为：
    /// <list type="bullet">
    ///   <item>索引0：东 (+1, 0)</item>
    ///   <item>索引1：东北 (+1, -1)</item>
    ///   <item>索引2：西北 (0, -1)</item>
    ///   <item>索引3：西 (-1, 0)</item>
    ///   <item>索引4：西南 (-1, +1)</item>
    ///   <item>索引5：东南 (0, +1)</item>
    /// </list>
    /// 这些方向与<see cref="_sAxialNeighbors"/>中定义的偏移量一一对应。
    /// </remarks>
    public IEnumerable<Axial> GetNeighbors(Axial hex)
    {
        foreach (var offset in _sAxialNeighbors)
            yield return offset + hex;
    }

    /// <summary>
    /// 获取指定六边形的邻居六边形集合
    /// </summary>
    /// <param name="hex">要获取邻居的六边形坐标</param>
    /// <returns>返回包含所有邻居六边形的可枚举集合</returns>
    public IEnumerable<Cubic> GetNeighbors(Cubic hex)
    {
        // 遍历所有邻居偏移量，计算并返回邻居六边形坐标
        foreach (var offset in _sCubicNeighbors)
            yield return offset + hex;
    }

    #endregion

    #region IsNeighbors

    /// <summary>
    /// 检查两个偏移坐标表示的六边形是否为邻居关系
    /// </summary>
    /// <param name="coord1">第一个六边形的偏移坐标</param>
    /// <param name="coord2">第二个六边形的偏移坐标</param>
    /// <returns>如果是邻居则返回true，否则返回false</returns>
    public bool IsNeighbors(Offset coord1, Offset coord2)
        => IsNeighbors(coord1, coord2, GetNeighbor);

    /// <summary>
    /// 检查两个轴向坐标表示的六边形是否为邻居关系
    /// </summary>
    /// <param name="coord1">第一个六边形的轴向坐标</param>
    /// <param name="coord2">第二个六边形的轴向坐标</param>
    /// <returns>如果是邻居则返回true，否则返回false</returns>
    public bool IsNeighbors(Axial coord1, Axial coord2)
    {
        Func<Axial, int, Axial> getNeighbor = GetNeighbor;
        return IsNeighbors(coord1, coord2, getNeighbor);
    }

    /// <summary>
    /// 检查两个立方体坐标表示的六边形是否为邻居关系
    /// </summary>
    /// <param name="coord1">第一个六边形的立方体坐标</param>
    /// <param name="coord2">第二个六边形的立方体坐标</param>
    /// <returns>如果是邻居则返回true，否则返回false</returns>
    public bool IsNeighbors(Cubic coord1, Cubic coord2)
        => IsNeighbors(coord1, coord2, GetNeighbor);


    /// <summary>
    /// 检查两个六边形坐标是否为相邻关系
    /// </summary>
    /// <typeparam name="T">六边形坐标类型（如Offset、Axial、Cubic等），需实现相等性比较接口</typeparam>
    /// <param name="coord1">第一个六边形坐标</param>
    /// <param name="coord2">第二个六边形坐标</param>
    /// <param name="getNeighbor">获取邻居坐标的函数，参数为源坐标和邻居索引（0-5），返回对应方向的邻居坐标</param>
    /// <returns>若两个坐标为相邻六边形则返回 true，否则返回 false</returns>
    /// <remarks>
    /// 方法通过循环检查第一个坐标的所有6个邻居（使用 <paramref name="getNeighbor"/> 函数获取），
    /// 若其中任何一个邻居与第二个坐标相等，则判定为相邻关系。
    /// </remarks>
    private bool IsNeighbors<T>(T coord1, T coord2, in Func<T, int, T> getNeighbor)
        where T : struct, IEqualityComparer<T>
    {
        for (var neighborIndex = 0; neighborIndex < EDGES_COUNT; neighborIndex++)
        {
            var neighbor = getNeighbor(coord1, neighborIndex);
            if (neighbor.Equals(coord2))
            {
                return true;
            }
        }

        return false;
    }

    #endregion

    #region GetNeighborsRing

    /// <summary>
    /// 获取指定中心点周围指定半径的邻居环形区域
    /// </summary>
    /// <param name="center">中心点坐标偏移量</param>
    /// <param name="radius">环形区域的半径</param>
    /// <returns>返回环形区域内的所有邻居点坐标偏移量集合</returns>
    public IEnumerable<Offset> GetNeighborsRing(Offset center, int radius)
        => GetNeighborsRing(center, radius, GetNeighbor);


    /// <summary>
    /// Returns a ring with a radius of <see cref="radius"/> hexes around the given <see cref="center"/>.
    /// </summary>
    /// <summary>
    /// 获取指定中心位置和半径的邻居环形区域中的所有轴向坐标点
    /// </summary>
    /// <param name="center">环形区域的中心轴向坐标</param>
    /// <param name="radius">环形区域的半径</param>
    /// <returns>返回指定环形区域中所有轴向坐标的可枚举集合</returns>
    public IEnumerable<Axial> GetNeighborsRing(Axial center, int radius)
        => GetNeighborsRing(center, radius, GetNeighbor);

    /// <summary>
    /// 获取指定中心位置周围指定半径范围内的所有立方体邻居
    /// </summary>
    /// <param name="center">中心立方体坐标</param>
    /// <param name="radius">搜索半径</param>
    /// <returns>返回半径范围内所有邻居立方体的枚举序列</returns>
    public IEnumerable<Cubic> GetNeighborsRing(Cubic center, int radius)
        => GetNeighborsRing(center, radius, GetNeighbor);


    /// <summary>
    /// 生成以指定中心为原点、指定半径的六边形环形邻居集合
    /// </summary>
    /// <typeparam name="T">六边形坐标类型（如Offset、Axial或Cubic），必须为值类型</typeparam>
    /// <param name="center">环形的中心六边形坐标</param>
    /// <param name="radius">环形半径（单位：六边形数量），值为0时仅返回中心坐标</param>
    /// <param name="getNeighbor">获取邻居坐标的函数，参数为源坐标和邻居方向索引（0-5），返回对应方向的邻居坐标</param>
    /// <returns>环形上所有六边形坐标的可枚举集合，顺序为顺时针方向</returns>
    /// <remarks>
    /// 算法核心步骤：
    /// 1. 若半径为0，直接返回中心坐标
    /// 2. 否则，将中心沿方向索引4移动<paramref name="radius"/>步，到达环形起点
    /// 3. 依次沿6个方向（0-5）各移动<paramref name="radius"/>步，收集途经的坐标形成闭合环形
    /// </remarks>
    private static IEnumerable<T> GetNeighborsRing<T>(T center, int radius, Func<T, int, T> getNeighbor)
        where T : struct
    {
        if (radius == 0)
        {
            yield return center;
            yield break;
        }

        for (var i = 0; i < radius; i++)
            center = getNeighbor(center, 4);

        for (var i = 0; i < 6; i++)
        {
            for (var j = 0; j < radius; j++)
            {
                yield return center;
                center = getNeighbor(center, i);
            }
        }
    }

    #endregion

    #region GetNeighborsAround

    /// <summary>
    /// 获取指定中心点周围指定半径范围内的所有邻居偏移量
    /// </summary>
    /// <param name="center">中心点的偏移量</param>
    /// <param name="radius">搜索半径</param>
    /// <returns>返回在指定半径范围内的所有邻居偏移量的可枚举集合</returns>
    public IEnumerable<Offset> GetNeighborsAround(Offset center, int radius)
        => GetNeighborsAround(center, radius, GetNeighborsRing);


    /// <summary>
    /// 获取指定中心点周围指定半径范围内的所有六边形邻居
    /// </summary>
    /// <param name="center">中心点的轴向坐标</param>
    /// <param name="radius">搜索半径</param>
    /// <returns>返回在指定半径范围内的所有六边形邻居的可枚举集合</returns>
    public IEnumerable<Axial> GetNeighborsAround(Axial center, int radius)
        => GetNeighborsAround(center, radius, GetNeighborsRing);

    /// <summary>
    /// 获取指定中心点周围指定半径范围内的所有六边形邻居
    /// </summary>
    /// <param name="center">中心点的立方体坐标</param>
    /// <param name="radius">搜索半径</param>
    /// <returns>返回在指定半径范围内的所有六边形邻居的可枚举集合</returns>
    public IEnumerable<Cubic> GetNeighborsAround(Cubic center, int radius)
        => GetNeighborsAround(center, radius, GetNeighborsRing);

    /// <summary>
    /// 生成以指定中心为原点、指定半径范围内的所有六边形区域邻居集合（包含多层环形）
    /// </summary>
    /// <typeparam name="T">六边形坐标类型（如Offset、Axial或Cubic），必须为值类型</typeparam>
    /// <param name="center">区域的中心六边形坐标</param>
    /// <param name="radius">区域半径（单位：六边形数量），定义包含的环形层数（0表示仅中心，1表示中心+1层环，以此类推）</param>
    /// <param name="getNeighborRing">获取指定半径环形邻居的函数，参数为中心坐标和环形半径，返回该环形上的所有坐标</param>
    /// <returns>区域内所有六边形坐标的可枚举集合，按半径从小到大顺序排列</returns>
    /// <remarks>
    /// 实现逻辑：通过循环获取从半径0到<paramref name="radius"/>-1的所有环形邻居集合，
    /// 并将这些环形坐标按半径顺序合并为一个连续的区域集合。例如，radius=2时将包含半径0（中心）、半径1（内环）的所有坐标。
    /// </remarks>
    private static IEnumerable<T> GetNeighborsAround<T>(T center, int radius, Func<T, int, IEnumerable<T>> getNeighborRing)
        where T : struct
    {
        for (var i = 0; i < radius; i++)
        {
            foreach (var hex in getNeighborRing(center, i))
                yield return hex;
        }
    }

    #endregion

    #region GetNeighborIndex

    /// <summary>
    /// 获取指定邻居位置在邻居列表中的索引
    /// </summary>
    /// <param name="center">中心位置的偏移量</param>
    /// <param name="neighbor">邻居位置的偏移量</param>
    /// <returns>邻居在邻居列表中的索引值</returns>
    public byte GetNeighborIndex(Offset center, Offset neighbor) => GetNeighborIndex(center, neighbor, GetNeighbors);

    /// <summary>
    /// 获取指定邻居位置在中心位置邻居数组中的索引
    /// </summary>
    /// <param name="center">中心位置的轴向坐标</param>
    /// <param name="neighbor">邻居位置的轴向坐标</param>
    /// <returns>邻居位置在中心位置邻居数组中的索引值</returns>
    public byte GetNeighborIndex(Axial center, Axial neighbor) => GetNeighborIndex(center, neighbor, GetNeighbors);


    /// <summary>
    /// 获取指定邻居位置在邻居数组中的索引
    /// </summary>
    /// <param name="center">中心位置的立方体坐标</param>
    /// <param name="neighbor">邻居位置的立方体坐标</param>
    /// <returns>邻居在邻居数组中的索引值</returns>
    public byte GetNeighborIndex(Cubic center, Cubic neighbor) => GetNeighborIndex(center, neighbor, GetNeighbors);

    /// <summary>
    /// 获取指定邻居相对于中心六边形的方向索引（0-5）
    /// </summary>
    /// <typeparam name="T">六边形坐标类型（如Offset、Axial或Cubic），需为值类型且实现相等性比较接口</typeparam>
    /// <param name="center">中心六边形坐标</param>
    /// <param name="neighbor">需要查找索引的邻居六边形坐标</param>
    /// <param name="getNeighbors">获取中心所有邻居坐标的函数，返回按方向索引顺序排列的邻居集合</param>
    /// <returns>邻居相对于中心的方向索引（0-5），对应六边形的六个方向</returns>
    /// <exception cref="HexagonalException">当<paramref name="neighbor"/>不是<paramref name="center"/>的邻居时抛出</exception>
    /// <remarks>
    /// 实现逻辑：通过<paramref name="getNeighbors"/>获取中心的所有邻居集合，按顺序遍历并与<paramref name="neighbor"/>比较，
    /// 返回第一个匹配项的索引。索引顺序与六边形的六个方向一一对应（0-5）。
    /// </remarks>
    private byte GetNeighborIndex<T>(T center, T neighbor, Func<T, IEnumerable<T>> getNeighbors)
        where T : struct, IEqualityComparer<T>
    {
        byte neighborIndex = 0;
        foreach (var current in getNeighbors(center))
        {
            if (current.Equals(neighbor))
                return neighborIndex;

            neighborIndex++;
        }

        throw new HexagonalException($"Can't find bypass index", this, (nameof(center), center), (nameof(neighbor), neighbor));
    }

    #endregion

    #region GetPointBetweenTwoNeighbours

    /// <summary>
    /// 获取两个相邻坐标之间的中点坐标
    /// </summary>
    /// <param name="coord1">第一个坐标偏移量</param>
    /// <param name="coord2">第二个坐标偏移量</param>
    /// <returns>返回两个相邻坐标之间的中点坐标，以(x, y)元组形式表示</returns>
    public (float x, float y) GetPointBetweenTwoNeighbours(Offset coord1, Offset coord2)
        => GetPointBetweenTwoNeighbours(coord1, coord2, IsNeighbors, ToPoint2);

    /// <summary>
    /// 获取两个相邻坐标点之间的中点坐标
    /// </summary>
    /// <param name="coord1">第一个轴向坐标</param>
    /// <param name="coord2">第二个轴向坐标</param>
    /// <returns>返回两个相邻坐标点之间的中点坐标，以(x, y)元组形式表示</returns>
    public (float x, float y) GetPointBetweenTwoNeighbours(Axial coord1, Axial coord2)
        => GetPointBetweenTwoNeighbours(coord1, coord2, IsNeighbors, ToPoint2);

    /// <summary>
    /// 获取两个相邻坐标点之间的中点坐标
    /// </summary>
    /// <param name="coord1">第一个立方体坐标</param>
    /// <param name="coord2">第二个立方体坐标</param>
    /// <returns>返回两个相邻坐标点之间的中点坐标，以(x, y)元组形式表示</returns>
    public (float x, float y) GetPointBetweenTwoNeighbours(Cubic coord1, Cubic coord2)
        => GetPointBetweenTwoNeighbours(coord1, coord2, IsNeighbors, ToPoint2);

    /// <summary>
    /// 计算两个相邻六边形边界线段的中点坐标
    /// </summary>
    /// <typeparam name="T">六边形坐标类型（如Offset、Axial或Cubic），必须为值类型</typeparam>
    /// <param name="coord1">第一个六边形坐标（邻居之一）</param>
    /// <param name="coord2">第二个六边形坐标（邻居之二）</param>
    /// <param name="isNeighbor">判断两个坐标是否为相邻关系的函数</param>
    /// <param name="toPoint">将六边形坐标转换为其中心在2D空间中坐标的函数</param>
    /// <returns>两个相邻六边形边界线段的中点（X,Y）坐标</returns>
    /// <exception cref="HexagonalException">当<paramref name="coord1"/>与<paramref name="coord2"/>不是相邻关系时抛出</exception>
    /// <remarks>
    /// 实现逻辑：
    /// 1. 首先通过<paramref name="isNeighbor"/>验证两个坐标是否为邻居
    /// 2. 若不相邻则抛出异常，否则通过<paramref name="toPoint"/>获取两个六边形的中心坐标
    /// 3. 计算两个中心坐标的平均值，得到边界线段的中点
    /// </remarks>
    private (float x, float y) GetPointBetweenTwoNeighbours<T>(T coord1, T coord2, Func<T, T, bool> isNeighbor, Func<T, (float X, float Y)> toPoint)
        where T : struct
    {
        if (!isNeighbor(coord1, coord2))
        {
            throw new HexagonalException($"Can't calculate point between not neighbors", this, (nameof(coord1), coord1), (nameof(coord2), coord2));
        }

        var c1 = toPoint(coord1);
        var c2 = toPoint(coord2);

        return ((c1.X + c2.X) / 2, (c1.Y + c2.Y) / 2);
    }

    #endregion

    #region CubeDistance

    /// <summary>
    /// 计算两个偏移坐标之间的立方体距离
    /// </summary>
    /// <param name="h1">第一个偏移坐标</param>
    /// <param name="h2">第二个偏移坐标</param>
    /// <returns>两个坐标之间的立方体距离</returns>
    public int CubeDistance(Offset h1, Offset h2)
    {
        var cubicFrom = ToCubic(h1);
        var cubicTo = ToCubic(h2);
        return CubeDistance(cubicFrom, cubicTo);
    }

    /// <summary>
    /// 计算两个轴向坐标之间的立方体距离
    /// </summary>
    /// <param name="h1">第一个轴向坐标</param>
    /// <param name="h2">第二个轴向坐标</param>
    /// <returns>两个坐标之间的立方体距离</returns>
    public int CubeDistance(Axial h1, Axial h2)
    {
        var cubicFrom = ToCubic(h1);
        var cubicTo = ToCubic(h2);
        return CubeDistance(cubicFrom, cubicTo);
    }

    /// <summary>
    /// 计算两个立方体坐标（Cubic）之间的曼哈顿距离（Manhattan distance）
    /// </summary>
    /// <param name="h1">第一个六边形的立方体坐标</param>
    /// <param name="h2">第二个六边形的立方体坐标</param>
    /// <returns>两个六边形坐标之间的曼哈顿距离值，范围为非负整数</returns>
    /// <remarks>
    /// 立方体坐标系统下的曼哈顿距离计算公式为：( |x1-x2| + |y1-y2| + |z1-z2| ) / 2
    /// 该公式利用了立方体坐标的性质，其中x + y + z = 0，因此总和必为偶数，结果总是整数
    /// 距离表示从一个六边形移动到另一个六边形所需的最少相邻六边形步数
    /// </remarks>
    public static int CubeDistance(Cubic h1, Cubic h2)
        => (Abs(h1.X - h2.X) + Abs(h1.Y - h2.Y) + Abs(h1.Z - h2.Z)) / 2;

    #endregion

    #region Neighbors

    /// <summary>
    /// 获取指定六边形坐标的所有邻居偏移量列表
    /// </summary>
    /// <param name="coord">六边形的Offset坐标</param>
    /// <returns>包含6个邻居相对偏移量的只读列表，偏移量顺序对应六边形的6个方向</returns>
    /// <remarks>
    /// 根据网格类型（<see cref="Type"/>）和坐标的奇偶性选择对应的邻居偏移量列表：
    /// <list type="bullet">
    ///   <item>PointyOdd/PointyEven：根据 <paramref name="coord"/> 的Y值奇偶性选择 <see cref="_sPointyOddNeighbors"/> 或 <see cref="_sPointyEvenNeighbors"/></item>
    ///   <item>FlatOdd/FlatEven：根据 <paramref name="coord"/> 的X值奇偶性选择 <see cref="_sFlatOddNeighbors"/> 或 <see cref="_sFlatEvenNeighbors"/></item>
    /// </list>
    /// </remarks>
    private IReadOnlyList<Offset> GetNeighborsOffsets(Offset coord)
    {
        switch (Type)
        {
            case HexagonalGridType.PointyOdd:
                return Abs(coord.Y % 2) == 0 ? _sPointyEvenNeighbors : _sPointyOddNeighbors;
            case HexagonalGridType.PointyEven:
                return Abs(coord.Y % 2) == 1 ? _sPointyEvenNeighbors : _sPointyOddNeighbors;
            case HexagonalGridType.FlatOdd:
                return Abs(coord.X % 2) == 0 ? _sFlatEvenNeighbors : _sFlatOddNeighbors;
            case HexagonalGridType.FlatEven:
                return Abs(coord.X % 2) == 1 ? _sFlatEvenNeighbors : _sFlatOddNeighbors;
            default:
                throw new HexagonalException($"{nameof(GetNeighborsOffsets)} failed with unexpected {nameof(Type)}", this, (nameof(coord), coord));
        }
    }

    /// <summary>
    /// 提供六边形网格中各个方向邻居偏移量的静态只读列表。
    /// 这些偏移量用于在不同坐标系和方向布局下计算相邻六边形的位置。
    /// </summary>
    private static readonly List<Offset> _sPointyOddNeighbors =
    [
        new Offset(+1, 0), new Offset(+1, -1), new Offset(0, -1),
        new Offset(-1, 0), new Offset(0, +1), new Offset(+1, +1)
    ];

    /// <summary>
    /// 在点朝上（pointy）且行号为偶数时，表示六边形六个邻居的相对坐标偏移。
    /// 每个 Offset 表示相对于当前六边形的一个邻居位置。
    /// </summary>
    private static readonly List<Offset> _sPointyEvenNeighbors =
    [
        new Offset(+1, 0), new Offset(0, -1), new Offset(-1, -1),
        new Offset(-1, 0), new Offset(-1, +1), new Offset(0, +1)
    ];

    /// <summary>
    /// 在平顶（flat）且行号为奇数时，表示六边形六个邻居的相对坐标偏移。
    /// 每个 Offset 表示相对于当前六边形的一个邻居位置。
    /// </summary>
    private static readonly List<Offset> _sFlatOddNeighbors =
    [
        new Offset(+1, +1), new Offset(+1, 0), new Offset(0, -1),
        new Offset(-1, 0), new Offset(-1, +1), new Offset(0, +1)
    ];

    /// <summary>
    /// 在平顶（flat）且行号为偶数时，表示六边形六个邻居的相对坐标偏移。
    /// 每个 Offset 表示相对于当前六边形的一个邻居位置。
    /// </summary>
    private static readonly List<Offset> _sFlatEvenNeighbors =
    [
        new Offset(+1, 0), new Offset(+1, -1), new Offset(0, -1),
        new Offset(-1, -1), new Offset(-1, 0), new Offset(0, +1)
    ];

    /// <summary>
    /// 使用轴向坐标（Axial Coordinate）表示的六边形邻居偏移量列表。
    /// 包含围绕中心六边形的六个直接邻居的方向向量。
    /// </summary>
    private static readonly List<Axial> _sAxialNeighbors =
    [
        new Axial(+1, 0), new Axial(+1, -1), new Axial(0, -1),
        new Axial(-1, 0), new Axial(-1, +1), new Axial(0, +1)
    ];

    /// <summary>
    /// 使用立方体坐标（Cubic Coordinate）表示的六边形邻居偏移量列表。
    /// 所有方向满足 x + y + z = 0 的约束条件，代表与中心六边形相邻的六个方向。
    /// </summary>
    private static readonly List<Cubic> _sCubicNeighbors =
    [
        new Cubic(+1, -1, 0), new Cubic(+1, 0, -1), new Cubic(0, +1, -1),
        new Cubic(-1, +1, 0), new Cubic(-1, 0, +1), new Cubic(0, -1, +1)
    ];

    #endregion

    /// <summary>
    /// 将索引标准化到有效范围内
    /// </summary>
    /// <param name="index">需要标准化的索引值</param>
    /// <returns>标准化后的索引值，确保在[0, EDGES_COUNT)范围内</returns>
    private static int NormalizeIndex(int index)
    {
        // 使用模运算将索引限制在EDGES_COUNT范围内
        index %= EDGES_COUNT;

        // 处理负数索引的情况，将其转换为正数索引
        if (index < 0)
            index += EDGES_COUNT;

        return index;
    }
}