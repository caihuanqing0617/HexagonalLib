namespace HexagonalLib;

/// <summary>
/// 六边形网格类型的枚举定义
/// 用于标识不同的六边形网格布局方式
/// </summary>
public enum HexGridType : byte
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

/// <summary>
/// 用于描述尖顶六边形各面的方向性，
/// 其中每个方向均从六边形的中心出发，
/// 垂直于其所代表的面（将该面二等分，并从其中心直接向外延伸）。
/// 这里使用基本方向来使这些区分更加容易。
/// 表示六边形网格中的六个方向枚举
/// </summary>
public enum HexDirectionType
{
    /// <summary>
    /// 东方向
    /// </summary>
    East = 0,

    /// <summary>
    /// 东南方向
    /// </summary>
    SouthEast = 1,

    /// <summary>
    /// 西南方向
    /// </summary>
    SouthWest = 2,

    /// <summary>
    /// 西方向
    /// </summary>
    West = 3,

    /// <summary>
    /// 西北方向
    /// </summary>
    NorthWest = 4,

    /// <summary>
    /// 东北方向
    /// </summary>
    NorthEast = 5
}
