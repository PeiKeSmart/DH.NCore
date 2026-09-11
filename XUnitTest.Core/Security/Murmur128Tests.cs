using System.Security.Cryptography;
using System.Text;
using NewLife.Security;
using Xunit;

namespace XUnitTest.Security;

/// <summary>Murmur128哈希算法测试</summary>
public class Murmur128Tests
{
    [Fact(DisplayName = "相同输入产生相同哈希")]
    public void SameInputSameHash()
    {
        var data = Encoding.UTF8.GetBytes("Hello, Murmur128!");
        using var hasher1 = new Murmur128();
        using var hasher2 = new Murmur128();

        var hash1 = hasher1.ComputeHash(data);
        var hash2 = hasher2.ComputeHash(data);

        Assert.Equal(hash1, hash2);
    }

    [Fact(DisplayName = "哈希输出为16字节")]
    public void HashSizeIs128Bits()
    {
        var data = Encoding.UTF8.GetBytes("test");
        using var hasher = new Murmur128();

        var hash = hasher.ComputeHash(data);

        Assert.Equal(16, hash.Length);
        Assert.Equal(128, hasher.HashSize);
    }

    [Fact(DisplayName = "不同输入产生不同哈希")]
    public void DifferentInputDifferentHash()
    {
        using var hasher = new Murmur128();
        var hash1 = hasher.ComputeHash(Encoding.UTF8.GetBytes("input1"));
        hasher.Initialize();
        var hash2 = hasher.ComputeHash(Encoding.UTF8.GetBytes("input2"));

        Assert.NotEqual(hash1, hash2);
    }

    [Fact(DisplayName = "不同种子产生不同哈希")]
    public void DifferentSeedDifferentHash()
    {
        var data = Encoding.UTF8.GetBytes("same data");
        using var hasher1 = new Murmur128(0);
        using var hasher2 = new Murmur128(42);

        var hash1 = hasher1.ComputeHash(data);
        var hash2 = hasher2.ComputeHash(data);

        Assert.NotEqual(hash1, hash2);
    }

    [Fact(DisplayName = "空数据可以哈希")]
    public void EmptyDataHash()
    {
        using var hasher = new Murmur128();
        var hash = hasher.ComputeHash([]);

        Assert.Equal(16, hash.Length);
    }

    [Fact(DisplayName = "种子属性正确")]
    public void SeedProperty()
    {
        using var hasher = new Murmur128(12345);
        Assert.Equal((UInt32)12345, hasher.Seed);
    }

    [Fact(DisplayName = "Initialize重置状态")]
    public void InitializeResetsState()
    {
        var data = Encoding.UTF8.GetBytes("test data for reset");
        using var hasher = new Murmur128();

        var hash1 = hasher.ComputeHash(data);
        hasher.Initialize();
        var hash2 = hasher.ComputeHash(data);

        Assert.Equal(hash1, hash2);
    }

    [Fact(DisplayName = "大数据块哈希")]
    public void LargeDataHash()
    {
        // 大于16字节触发多块处理
        var data = new Byte[1024];
        new Random(42).NextBytes(data);
        using var hasher = new Murmur128();

        var hash = hasher.ComputeHash(data);

        Assert.Equal(16, hash.Length);
    }

    [Theory(DisplayName = "不同长度数据哈希")]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(15)]
    [InlineData(16)]
    [InlineData(17)]
    [InlineData(31)]
    [InlineData(32)]
    [InlineData(33)]
    [InlineData(255)]
    public void VariousLengthHash(Int32 length)
    {
        var data = new Byte[length];
        new Random(42).NextBytes(data);
        using var hasher = new Murmur128();

        var hash = hasher.ComputeHash(data);

        Assert.Equal(16, hash.Length);
    }

    [Fact(DisplayName = "通过CryptoStream使用")]
    public void UseThroughCryptoStream()
    {
        var data = Encoding.UTF8.GetBytes("Stream hashing test data that is longer than 16 bytes");
        using var hasher = new Murmur128();
        using var ms = new MemoryStream(data);
        var hash = hasher.ComputeHash(ms);

        Assert.Equal(16, hash.Length);
    }

    [Fact(DisplayName = "TryComputeHash与ComputeHash结果一致")]
    public void TryComputeHashMatchesComputeHash()
    {
        var data = Encoding.UTF8.GetBytes("Hello, Murmur128! 中文测试");
        using var hasher = new Murmur128();

        var hash = hasher.ComputeHash(data);
        var destination = new Byte[16];
        var ok = hasher.TryComputeHash(data, destination, out var written);

        Assert.True(ok);
        Assert.Equal(16, written);
        Assert.Equal(hash, destination);
    }

    [Theory(DisplayName = "不同长度TryComputeHash")]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(15)]
    [InlineData(16)]
    [InlineData(17)]
    [InlineData(31)]
    [InlineData(255)]
    public void TryComputeHashVariousLength(Int32 length)
    {
        var data = new Byte[length];
        new Random(42).NextBytes(data);
        using var hasher = new Murmur128();

        var hash = hasher.ComputeHash(data);
        var destination = new Byte[16];

        Assert.True(hasher.TryComputeHash(data, destination, out var written));
        Assert.Equal(16, written);
        Assert.Equal(hash, destination);
    }

    [Fact(DisplayName = "TryComputeHash目标缓冲区过短返回false")]
    public void TryComputeHashShortDestination()
    {
        var data = Encoding.UTF8.GetBytes("short");
        using var hasher = new Murmur128();

        var destination = new Byte[15];
        Assert.False(hasher.TryComputeHash(data, destination, out var written));
        Assert.Equal(0, written);
    }

    [Theory(DisplayName = "黄金值锁定：ComputeHash与TryComputeHash逐位一致")]
    [InlineData("Hello, Murmur128!", "73FB55DF3C6DC70907DC9511AAE43133")]
    [InlineData("test", "9DE1BD74CC287DAC824DBDF93182129A")]
    [InlineData("The quick brown fox jumps over the lazy dog", "6C1B07BC7BBC4BE347939AC4A93C437A")]
    [InlineData("", "00000000000000000000000000000000")]
    public void GoldenValue(String text, String expected)
    {
        var data = Encoding.UTF8.GetBytes(text);
        using var hasher = new Murmur128();

        var hash = hasher.ComputeHash(data);
        Assert.Equal(expected, BitConverter.ToString(hash).Replace("-", ""));

        var destination = new Byte[16];
        Assert.True(hasher.TryComputeHash(data, destination, out _));
        Assert.Equal(expected, BitConverter.ToString(destination).Replace("-", ""));
    }

    [Theory(DisplayName = "黄金值锁定：不同种子")]
    [InlineData(42, "abc", "D6F7CFB39F08850D303D35422B711075")]
    [InlineData(42, "The quick brown fox jumps over the lazy dog", "D7D50BFE93CF0D748F5C70ECF46C54C4")]
    public void GoldenValueWithSeed(UInt32 seed, String text, String expected)
    {
        var data = Encoding.UTF8.GetBytes(text);
        using var hasher = new Murmur128(seed);

        var hash = hasher.ComputeHash(data);
        Assert.Equal(expected, BitConverter.ToString(hash).Replace("-", ""));

        var destination = new Byte[16];
        Assert.True(hasher.TryComputeHash(data, destination, out _));
        Assert.Equal(expected, BitConverter.ToString(destination).Replace("-", ""));
    }

    [Theory(DisplayName = "黄金值锁定：边界长度（整块与多块）")]
    [InlineData(16, 0, "2AEC68238A535C02E747611F7F7FB619")]
    [InlineData(32, 0, "FBB9B47C18F61A89EC1B6A68E20F4797")]
    [InlineData(1000, 0, "69582F2DA1B8F3C3657689592067AF81")]
    [InlineData(1000, 42, "7EF3E7D5493B89DA40F1B9D41247FF6D")]
    public void GoldenValueByPattern(Int32 length, UInt32 seed, String expected)
    {
        var data = new Byte[length];
        for (var i = 0; i < data.Length; i++) data[i] = (Byte)(i * 131 + 17);

        using var hasher = new Murmur128(seed);

        var hash = hasher.ComputeHash(data);
        Assert.Equal(expected, BitConverter.ToString(hash).Replace("-", ""));

        var destination = new Byte[16];
        Assert.True(hasher.TryComputeHash(data, destination, out _));
        Assert.Equal(expected, BitConverter.ToString(destination).Replace("-", ""));
    }

    [Fact(DisplayName = "分块流式哈希与一次性哈希结果一致")]
    public void ChunkedHashMatchesOneShot()
    {
        var data = new Byte[100];
        for (var i = 0; i < data.Length; i++) data[i] = (Byte)(i * 131 + 17);

        using var one = new Murmur128();
        var hash1 = one.ComputeHash(data);

        // 用 TransformBlock 分块喂入，覆盖跨 16 字节边界的流式路径
        using var chunked = new Murmur128();
        var buf = new Byte[7];
        var offset = 0;
        while (offset < data.Length)
        {
            var len = Math.Min(buf.Length, data.Length - offset);
            Array.Copy(data, offset, buf, 0, len);
            chunked.TransformBlock(buf, 0, len, null, 0);
            offset += len;
        }
        chunked.TransformFinalBlock([], 0, 0);
        var hash2 = chunked.Hash!;

        Assert.Equal(hash1, hash2);
    }

    [Fact(DisplayName = "ComputeHash(Stream)与ComputeHash(数组)结果一致")]
    public void StreamHashMatchesArrayHash()
    {
        var data = new Byte[1000];
        for (var i = 0; i < data.Length; i++) data[i] = (Byte)(i * 131 + 17);

        using var one = new Murmur128();
        var hash1 = one.ComputeHash(data);

        using var ms = new MemoryStream(data);
        using var stream = new Murmur128();
        var hash2 = stream.ComputeHash(ms);

        Assert.Equal(hash1, hash2);
    }

    [Fact(DisplayName = "TryComputeHash短缓冲失败后重算正确")]
    public void TryComputeHashFailureThenRecover()
    {
        var data = Encoding.UTF8.GetBytes("failure recovery test data");

        using var hasher = new Murmur128();
        var shortBuf = new Byte[15];
        Assert.False(hasher.TryComputeHash(data, shortBuf, out var written));
        Assert.Equal(0, written);

        // 失败后重新初始化再算，结果应正确
        hasher.Initialize();
        var dest = new Byte[16];
        Assert.True(hasher.TryComputeHash(data, dest, out written));
        Assert.Equal(16, written);

        using var once = new Murmur128();
        var expected = once.ComputeHash(data);
        Assert.Equal(expected, dest);
    }
}
