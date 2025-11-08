namespace HexagonalLib;

/// <summary>
/// 六边形网格类型的枚举定义
/// 用于标识不同的六边形网格布局方式
/// </summary>
public enum HexagonalGridType : byte
{
    /// <summary>
    /// 水平布局，奇数行向右偏移
    /// </summary>
    PointyOdd,

    /// <summary>
    /// 水平布局，偶数行向右偏移
    /// </summary>
    PointyEven,

    /// <summary>
    /// 垂直布局，奇数列向下偏移
    /// </summary>
    FlatOdd,

    /// <summary>
    /// 垂直布局，偶数列向下偏移
    /// </summary>
    FlatEven,
}