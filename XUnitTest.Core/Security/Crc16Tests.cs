using NewLife.Security;
using Xunit;

namespace XUnitTest.Security;

public class Crc16Tests
{
    [Fact]
    public void TestComputeWithByteArray()
    {
        var data = "123456789"u8.ToArray();
        var crc = Crc16.Compute(data);
        Assert.Equal(0x31C3, crc);
    }

    [Fact]
    public void TestComputeWithStream()
    {
        var data = "123456789"u8.ToArray();
        using var stream = new MemoryStream(data);
        var crc = Crc16.Compute(stream, -1);
        Assert.Equal(0x31C3, crc);
    }

    [Fact]
    public void TestComputeWithReadOnlySpan()
    {
        var data = "123456789"u8.ToArray();
        var crc = Crc16.Compute(new ReadOnlySpan<Byte>(data));
        Assert.Equal(0x31C3, crc);
    }

    [Fact]
    public void TestComputeModbusWithByteArray()
    {
        var data = "123456789"u8.ToArray();
        var crc = Crc16.ComputeModbus(data, 0);
        Assert.Equal(0x4B37, crc);
    }

    [Fact]
    public void TestComputeModbusWithStream()
    {
        var data = "123456789"u8.ToArray();
        using var stream = new MemoryStream(data);
        var crc = Crc16.ComputeModbus(stream);
        Assert.Equal(0x4B37, crc);
    }

    [Fact(DisplayName = "Update(Int16)只取低8位不越界")]
    public void UpdateInt16UsesLower8Bits()
    {
        // value 超出 0~255 时只取低 8 位参与查表，不应越界
        var crc1 = new Crc16();
        crc1.Update((Int16)0x1234);
        var crc2 = new Crc16();
        crc2.Update((Int16)0x34);
        Assert.Equal(crc2.Value, crc1.Value);

        var crc3 = new Crc16();
        crc3.Update(unchecked((Int16)0xFFFF));
        var crc4 = new Crc16();
        crc4.Update((Int16)0xFF);
        Assert.Equal(crc4.Value, crc3.Value);
    }

    [Theory(DisplayName = "ComputeModbus指定偏移与数量")]
    [InlineData(1, -1)]
    [InlineData(2, 3)]
    [InlineData(0, 5)]
    [InlineData(4, 0)]
    public void ComputeModbusWithOffset(Int32 offset, Int32 count)
    {
        var data = "123456789"u8.ToArray();
        var crc = Crc16.ComputeModbus(data, offset, count);

        // 与切子数组从 0 计算的结果一致
        var end = count > 0 ? offset + count : data.Length;
        var sub = data[offset..end];
        var expected = Crc16.ComputeModbus(sub, 0);
        Assert.Equal(expected, crc);
    }
}
