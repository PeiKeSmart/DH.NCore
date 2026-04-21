using System;
using System.ComponentModel;
using NewLife.Data;
using NewLife.Messaging;
using Xunit;

namespace XUnitTest.Messaging;

public class PacketCodecTests
{
    [Fact]
    [DisplayName("异常残包到期后应丢弃并恢复后续新帧解析")]
    public void Parse_ShouldDropExpiredIncompleteCache()
    {
        var codec = new PacketCodec
        {
            GetLength2 = static span => span.Length > 0 ? span[0] : 0,
            Expire = 10,
            Last = DateTime.Now.AddSeconds(-1)
        };

        var rs1 = codec.Parse(new ArrayPacket([5, 1]));

        Assert.Empty(rs1);

        var rs2 = codec.Parse(new ArrayPacket([2, 9]));

        var pk = Assert.Single(rs2);
        Assert.Equal(2, pk.Total);
        Assert.Equal((Byte)2, pk[0]);
        Assert.Equal((Byte)9, pk[1]);
    }
}