using System.ComponentModel;
using NewLife.Net.Handlers;
using Xunit;

namespace XUnitTest.Net;

public class MessageCodecTests
{
    [Fact]
    [DisplayName("变长长度字段未收全时应返回数据不足")]
    public void GetLength_ShouldReturnZero_WhenEncodedLengthIncomplete()
    {
        ReadOnlySpan<Byte> data = [0x30, 0x80];

        var len = MessageCodec<Object>.GetLength(data, 1, 0);

        Assert.Equal(0, len);
    }
}