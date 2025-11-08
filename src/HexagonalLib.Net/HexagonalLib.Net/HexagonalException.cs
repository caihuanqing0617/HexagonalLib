using System;
using System.Text;

namespace HexagonalLib;

/// <summary>
/// 表示六边形网格相关的异常类
/// </summary>
public class HexagonalException : Exception
{
    /// <summary>
    /// 用于构建异常消息的内部类
    /// </summary>
    private class MessageBuilder
    {
        private readonly StringBuilder _Message = new();

        /// <summary>
        /// 向消息构建器中追加一行消息
        /// </summary>
        /// <param name="message">要追加的消息内容</param>
        /// <returns>当前消息构建器实例，用于链式调用</returns>
        public MessageBuilder Append(string message)
        {
            _Message.AppendLine(message);
            return this;
        }

        /// <summary>
        /// 向消息构建器中追加六边形网格的信息
        /// </summary>
        /// <param name="grid">要追加信息的六边形网格对象</param>
        /// <returns>当前消息构建器实例，用于链式调用</returns>
        public MessageBuilder Append(HexagonalGrid grid)
        {
            Append(nameof(grid.Type), grid.Type);
            Append(nameof(grid.InscribedRadius), grid.InscribedRadius);
            Append(nameof(grid.DescribedRadius), grid.DescribedRadius);
            return this;
        }

        /// <summary>
        /// 向消息构建器中追加字段信息
        /// </summary>
        /// <param name="fields">要追加的字段元组数组，每个元组包含字段名和字段值</param>
        /// <returns>当前消息构建器实例，用于链式调用</returns>
        public MessageBuilder Append(params (string, object)[] fields)
        {
            foreach (var (paramName, paramValue) in fields)
                Append(paramName, paramValue);

            return this;
        }

        /// <summary>
        /// 向消息构建器中追加单个字段信息
        /// </summary>
        /// <param name="paramName">字段名称</param>
        /// <param name="paramValue">字段值</param>
        private void Append(string paramName, object paramValue) => _Message.Append($"{paramName}={paramValue}; ");

        /// <summary>
        /// 将构建的消息转换为字符串
        /// </summary>
        /// <returns>构建完成的消息字符串</returns>
        public override string ToString() => _Message.ToString();
    }

    /// <summary>
    /// 初始化 HexagonalException 类的新实例
    /// </summary>
    /// <param name="message">描述错误的消息</param>
    public HexagonalException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// 初始化 HexagonalException 类的新实例，包含指定的错误消息和字段信息
    /// </summary>
    /// <param name="message">描述错误的消息</param>
    /// <param name="fields">要包含在错误消息中的字段信息数组</param>
    public HexagonalException(string message, params (string, object)[] fields)
        : base(CreateBuilder(message).Append(fields).ToString())
    {
    }

    /// <summary>
    /// 初始化 HexagonalException 类的新实例，包含指定的错误消息和六边形网格信息
    /// </summary>
    /// <param name="message">描述错误的消息</param>
    /// <param name="grid">要包含在错误消息中的六边形网格对象</param>
    public HexagonalException(string message, HexagonalGrid grid)
        : base(CreateBuilder(message).Append(grid).ToString())
    {
    }

    /// <summary>
    /// 初始化 HexagonalException 类的新实例，包含指定的错误消息、六边形网格信息和字段信息
    /// </summary>
    /// <param name="message">描述错误的消息</param>
    /// <param name="grid">要包含在错误消息中的六边形网格对象</param>
    /// <param name="fields">要包含在错误消息中的额外字段信息数组</param>
    public HexagonalException(string message, HexagonalGrid grid, params (string, object)[] fields)
        : base(CreateBuilder(message).Append(grid).Append(fields).ToString())
    {
    }

    /// <summary>
    /// 创建一个新的消息构建器实例并添加初始消息
    /// </summary>
    /// <param name="message">初始消息内容</param>
    /// <returns>配置了初始消息的消息构建器实例</returns>
    private static MessageBuilder CreateBuilder(string message) => new MessageBuilder().Append(message);
}