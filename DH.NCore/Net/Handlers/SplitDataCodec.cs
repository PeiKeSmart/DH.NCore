using NewLife.Data;
using NewLife.Messaging;
using NewLife.Model;

namespace NewLife.Net.Handlers;

/// <summary>按指定分割字节来处理粘包的处理器</summary>
/// <remarks>
/// 默认以"0x0D 0x0A"即换行来分割，分割的包包含分割字节本身，使用时请注意。
/// 使用方式：
/// <code>
/// // 默认分割方式（\r\n）
/// ISocket.Add&lt;SplitDataCodec&gt;();
///
/// // 自定义分割字节
/// ISocket.Add(new SplitDataCodec { SplitData = [0x01, 0x02] });
///
/// // 自定义最大缓存大小
/// ISocket.Add(new SplitDataCodec { MaxCacheDataLength = 2048 });
/// </code>
/// </remarks>
public class SplitDataCodec : Handler
{
    #region 属性
    /// <summary>粘包分割字节数据（默认0x0D,0x0A）</summary>
    public Byte[] SplitData { get; set; } = [0x0D, 0x0A];

    /// <summary>最大缓存待处理数据，默认1024字节</summary>
    public Int32 MaxCacheDataLength { get; set; } = 1024;
    #endregion

    /// <summary>写入数据，发送时在末尾追加分割字节</summary>
    /// <param name="context">处理器上下文</param>
    /// <param name="message">待发送的消息或数据包</param>
    /// <returns>追加分割字节后的数据包</returns>
    public override Object? Write(IHandlerContext context, Object message)
    {
        if (message is IPacket pk)
            message = pk.Append(SplitData);

        return base.Write(context, message);
    }

    /// <summary>读取数据，按分割字节拆包后逐个发送给后续处理器</summary>
    /// <param name="context">处理器上下文</param>
    /// <param name="message">接收到的数据包</param>
    /// <returns>拆包后的消息已逐个分发，返回null</returns>
    public override Object? Read(IHandlerContext context, Object message)
    {
        if (message is not IPacket pk) return base.Read(context, message);

        // 解码得到多个消息
        var list = Decode(context, pk);
        if (list == null) return null;

        foreach (var msg in list)
        {
            // 把数据发送给后续处理器
            //var rs = base.Read(context, msg);

            // 匹配输入回调，让上层事件收到分包信息
            //context.FireRead(rs);
            base.Read(context, msg);
        }

        return null;
    }

    /// <summary>连接关闭时，清空粘包编码器</summary>
    /// <param name="context">处理器上下文</param>
    /// <param name="reason">关闭原因</param>
    /// <returns>是否继续向下传递关闭通知</returns>
    public override Boolean Close(IHandlerContext context, String reason)
    {
        if (context.Owner is IExtend ss) ss["Codec"] = null;

        return base.Close(context, reason);
    }

    #region 粘包处理
    /// <summary>解码，从数据包中拆出多个完整消息</summary>
    /// <param name="context">处理器上下文</param>
    /// <param name="pk">待解码的数据包</param>
    /// <returns>拆分后的消息列表，无法解码时返回null</returns>
    protected IList<IPacket>? Decode(IHandlerContext context, IPacket pk)
    {
        if (context.Owner is not IExtend ss) return null;

        if (ss["Codec"] is not PacketCodec pc)
        {
#pragma warning disable CS0618 // 类型或成员已过时
            ss["Codec"] = pc = new PacketCodec
            {
                MaxCache = MaxCacheDataLength,
                // 主路径使用 GetLength2（span 版，性能更优），GetLength 仅链式包场景回退
                GetLength = GetLineLength,
                GetLength2 = GetLineLength,
                Tracer = (context.Owner as ISocket)?.Tracer
            };
#pragma warning restore CS0618 // 类型或成员已过时
        }

        return pc.Parse(pk);
    }

    /// <summary>获取包含分割字节在内的数据长度（匹配 GetLength 委托，供链式包场景回退）</summary>
    /// <param name="pk">数据包</param>
    /// <returns>包含分割字节在内的数据长度，未找到分割字节时返回0</returns>
    protected Int32 GetLineLength(IPacket pk)
    {
        var idx = pk.GetSpan().IndexOf(SplitData);
        if (idx < 0) return 0;

        return idx + SplitData.Length;
    }

    /// <summary>获取包含分割字节在内的数据长度（匹配 GetLength2 委托，性能更优）</summary>
    /// <param name="span">数据片段</param>
    /// <returns>包含分割字节在内的数据长度，未找到分割字节时返回0</returns>
    protected Int32 GetLineLength(ReadOnlySpan<Byte> span)
    {
        var idx = span.IndexOf(SplitData);
        if (idx < 0) return 0;

        return idx + SplitData.Length;
    }
    #endregion
}