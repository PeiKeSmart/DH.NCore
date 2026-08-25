using System.Runtime.CompilerServices;
using System.Security.Cryptography;
#if NET5_0_OR_GREATER
using System.Buffers.Binary;
#endif

namespace NewLife.Security;

/// <summary>高性能低碰撞Murmur128哈希算法</summary>
/// <remarks>
/// Redis等大量使用，比MD5要好
/// </remarks>
public class Murmur128 : HashAlgorithm
{
    #region 属性
    const UInt64 C1 = 0x87c37b91114253d5;
    const UInt64 C2 = 0x4cf5ad432745937f;

    private readonly UInt32 _Seed;
    /// <summary>种子</summary>
    public UInt32 Seed => _Seed;

    /// <summary>哈希大小</summary>
    public override Int32 HashSize => 128;

    private Int32 _Length;
    private UInt64 _H1;
    private UInt64 _H2;

    // 跨块尾部缓冲：分块流式哈希时缓存不足 16 字节的数据，凑满后按完整块处理
    private Byte[]? _tail;
    private Int32 _tailLen;
    #endregion

    #region 构造
    /// <summary>实例化</summary>
    /// <param name="seed"></param>
    public Murmur128(UInt32 seed = 0)
    {
        _Seed = seed;
        // 必须设置基类 HashSizeValue（位）。基类 TryComputeHash/TryHashFinal 依赖该字段做一致性校验，
        // 仅重写 HashSize 属性不会更新该字段，导致 TryComputeHash 抛"The algorithm's implementation is incorrect"
        HashSizeValue = 128;
        Reset();
    }
    #endregion

    #region 方法
    private void Reset()
    {
        // 初始化哈希值到种子
        _H1 = _H2 = Seed;

        // 重置长度为0
        _Length = 0;

        // 清空跨块尾部缓冲
        _tailLen = 0;
    }

    /// <summary>初始化</summary>
    public override void Initialize() => Reset();

    /// <summary>哈希核心</summary>
    /// <param name="array"></param>
    /// <param name="ibStart"></param>
    /// <param name="cbSize"></param>
    protected override void HashCore(Byte[] array, Int32 ibStart, Int32 cbSize)
    {
#if NET5_0_OR_GREATER
        // NET5+ 委托 Span 路径，复用 Span 版 Body/Tail，避免双份实现
        HashCore(array.AsSpan(ibStart, cbSize));
#else
        Core(array, ibStart, cbSize);
#endif
    }

#if !NET5_0_OR_GREATER
    // 数组版哈希核心（低版本 TFM）。跨块缓存不足 16 字节的数据，凑满后按完整块处理
    private void Core(Byte[] data, Int32 offset, Int32 count)
    {
        // 增加长度
        _Length += count;

        // 拼上之前缓存的尾部，凑满 16 字节块
        if (_tailLen > 0)
        {
            var need = 16 - _tailLen;
            var take = Math.Min(need, count);
            Buffer.BlockCopy(data, offset, _tail!, _tailLen, take);
            _tailLen += take;
            offset += take;
            count -= take;
            if (_tailLen == 16)
            {
                Body(_tail!, 0, 16);
                _tailLen = 0;
            }
        }

        // 处理完整 16 字节块
        var blocks = count / 16;
        if (blocks > 0)
        {
            Body(data, offset, blocks * 16);
            offset += blocks * 16;
            count -= blocks * 16;
        }

        // 剩余不足 16 字节，缓存
        if (count > 0)
        {
            _tail ??= new Byte[16];
            Buffer.BlockCopy(data, offset, _tail, 0, count);
            _tailLen = count;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Body(Byte[] data, Int32 start, Int32 length)
    {
        var remainder = length & 15;
        var alignedLength = start + (length - remainder);
        for (var i = start; i < alignedLength; i += 16)
        {
            _H1 ^= RotateLeft(data.ToUInt64(i) * C1, 31) * C2;
            _H1 = (RotateLeft(_H1, 27) + _H2) * 5 + 0x52dce729;

            _H2 ^= RotateLeft(data.ToUInt64(i + 8) * C2, 33) * C1;
            _H2 = (RotateLeft(_H2, 31) + _H1) * 5 + 0x38495ab5;
        }

        if (remainder > 0)
            Tail(data, alignedLength, remainder);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Tail(Byte[] tail, Int32 start, Int32 remaining)
    {
        // create our keys and initialize to 0
        UInt64 k1 = 0, k2 = 0;

        // determine how many bytes we have left to work with based on length
        switch (remaining)
        {
            case 15: k2 ^= (UInt64)tail[start + 14] << 48; goto case 14;
            case 14: k2 ^= (UInt64)tail[start + 13] << 40; goto case 13;
            case 13: k2 ^= (UInt64)tail[start + 12] << 32; goto case 12;
            case 12: k2 ^= (UInt64)tail[start + 11] << 24; goto case 11;
            case 11: k2 ^= (UInt64)tail[start + 10] << 16; goto case 10;
            case 10: k2 ^= (UInt64)tail[start + 9] << 8; goto case 9;
            case 9: k2 ^= (UInt64)tail[start + 8] << 0; goto case 8;
            case 8: k1 ^= (UInt64)tail[start + 7] << 56; goto case 7;
            case 7: k1 ^= (UInt64)tail[start + 6] << 48; goto case 6;
            case 6: k1 ^= (UInt64)tail[start + 5] << 40; goto case 5;
            case 5: k1 ^= (UInt64)tail[start + 4] << 32; goto case 4;
            case 4: k1 ^= (UInt64)tail[start + 3] << 24; goto case 3;
            case 3: k1 ^= (UInt64)tail[start + 2] << 16; goto case 2;
            case 2: k1 ^= (UInt64)tail[start + 1] << 8; goto case 1;
            case 1: k1 ^= (UInt64)tail[start] << 0; break;
        }

        _H2 ^= RotateLeft(k2 * C2, 33) * C1;
        _H1 ^= RotateLeft(k1 * C1, 31) * C2;
    }
#endif

    /// <summary>哈希结束</summary>
    /// <returns></returns>
    protected override Byte[] HashFinal()
    {
        // 处理跨块缓存的尾部
        if (_tailLen > 0)
        {
#if NET5_0_OR_GREATER
            Tail(_tail.AsSpan(0, _tailLen), 0, _tailLen);
#else
            Tail(_tail!, 0, _tailLen);
#endif
            _tailLen = 0;
        }

        var result = new Byte[16];
        FinalizeHash(result);

        return result;
    }

    /// <summary>计算最终哈希值写入目标缓冲区（小端序）。零分配</summary>
    /// <param name="destination">目标缓冲区，长度至少 16</param>
    private void FinalizeHash(Span<Byte> destination)
    {
        var len = (UInt64)_Length;
        _H1 ^= len; _H2 ^= len;

        _H1 += _H2;
        _H2 += _H1;

        _H1 = FMix(_H1);
        _H2 = FMix(_H2);

        _H1 += _H2;
        _H2 += _H1;

        // 直接写入字节，避免 BitConverter.GetBytes 分配
        for (var i = 0; i < 8; i++)
        {
            destination[i] = (Byte)(_H1 >> (8 * i));
            destination[8 + i] = (Byte)(_H2 >> (8 * i));
        }
    }

#if NET5_0_OR_GREATER
    /// <summary>Span 哈希核心。避免 ArrayPool 拷贝，支撑零分配 TryComputeHash</summary>
    /// <param name="source">源数据</param>
    protected override void HashCore(ReadOnlySpan<Byte> source)
    {
        Core(source);
    }

    // Span 版哈希核心。跨块缓存不足 16 字节的数据，凑满后按完整块处理
    private void Core(ReadOnlySpan<Byte> data)
    {
        // 增加长度
        _Length += data.Length;

        // 拼上之前缓存的尾部，凑满 16 字节块
        if (_tailLen > 0)
        {
            var need = 16 - _tailLen;
            var take = Math.Min(need, data.Length);
            data[..take].CopyTo(_tail.AsSpan(_tailLen));
            _tailLen += take;
            data = data[take..];
            if (_tailLen == 16)
            {
                Body(_tail);
                _tailLen = 0;
            }
        }

        // 处理完整 16 字节块
        var blocks = data.Length / 16;
        if (blocks > 0)
        {
            Body(data[..(blocks * 16)]);
            data = data[(blocks * 16)..];
        }

        // 剩余不足 16 字节，缓存
        if (data.Length > 0)
        {
            _tail ??= new Byte[16];
            data.CopyTo(_tail);
            _tailLen = data.Length;
        }
    }

    /// <summary>Span 哈希结束。零分配写入目标缓冲区</summary>
    /// <param name="destination">目标缓冲区，长度至少 16</param>
    /// <param name="bytesWritten">写入字节数</param>
    /// <returns>成功返回 true</returns>
    protected override Boolean TryHashFinal(Span<Byte> destination, out Int32 bytesWritten)
    {
        if (destination.Length < 16)
        {
            bytesWritten = 0;
            return false;
        }

        // 处理跨块缓存的尾部
        if (_tailLen > 0)
        {
            Tail(_tail.AsSpan(0, _tailLen), 0, _tailLen);
            _tailLen = 0;
        }

        FinalizeHash(destination);
        bytesWritten = 16;

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Body(ReadOnlySpan<Byte> data)
    {
        var remainder = data.Length & 15;
        var alignedLength = data.Length - remainder;
        for (var i = 0; i < alignedLength; i += 16)
        {
            _H1 ^= RotateLeft(BinaryPrimitives.ReadUInt64LittleEndian(data.Slice(i)) * C1, 31) * C2;
            _H1 = (RotateLeft(_H1, 27) + _H2) * 5 + 0x52dce729;

            _H2 ^= RotateLeft(BinaryPrimitives.ReadUInt64LittleEndian(data.Slice(i + 8)) * C2, 33) * C1;
            _H2 = (RotateLeft(_H2, 31) + _H1) * 5 + 0x38495ab5;
        }

        if (remainder > 0)
            Tail(data, alignedLength, remainder);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Tail(ReadOnlySpan<Byte> tail, Int32 start, Int32 remaining)
    {
        // create our keys and initialize to 0
        UInt64 k1 = 0, k2 = 0;

        // determine how many bytes we have left to work with based on length
        switch (remaining)
        {
            case 15: k2 ^= (UInt64)tail[start + 14] << 48; goto case 14;
            case 14: k2 ^= (UInt64)tail[start + 13] << 40; goto case 13;
            case 13: k2 ^= (UInt64)tail[start + 12] << 32; goto case 12;
            case 12: k2 ^= (UInt64)tail[start + 11] << 24; goto case 11;
            case 11: k2 ^= (UInt64)tail[start + 10] << 16; goto case 10;
            case 10: k2 ^= (UInt64)tail[start + 9] << 8; goto case 9;
            case 9: k2 ^= (UInt64)tail[start + 8] << 0; goto case 8;
            case 8: k1 ^= (UInt64)tail[start + 7] << 56; goto case 7;
            case 7: k1 ^= (UInt64)tail[start + 6] << 48; goto case 6;
            case 6: k1 ^= (UInt64)tail[start + 5] << 40; goto case 5;
            case 5: k1 ^= (UInt64)tail[start + 4] << 32; goto case 4;
            case 4: k1 ^= (UInt64)tail[start + 3] << 24; goto case 3;
            case 3: k1 ^= (UInt64)tail[start + 2] << 16; goto case 2;
            case 2: k1 ^= (UInt64)tail[start + 1] << 8; goto case 1;
            case 1: k1 ^= (UInt64)tail[start] << 0; break;
        }

        _H2 ^= RotateLeft(k2 * C2, 33) * C1;
        _H1 ^= RotateLeft(k1 * C1, 31) * C2;
    }
#endif
    #endregion

    #region 辅助
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //private static UInt32 RotateLeft(UInt32 x, Byte r) => (x << r) | (x >> (32 - r));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static UInt64 RotateLeft(UInt64 x, Byte r) => (x << r) | (x >> (64 - r));

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //private static UInt32 FMix(UInt32 h)
    //{
    //    h = (h ^ (h >> 16)) * 0x85ebca6b;
    //    h = (h ^ (h >> 13)) * 0xc2b2ae35;
    //    return h ^ (h >> 16);
    //}

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static UInt64 FMix(UInt64 h)
    {
        h = (h ^ (h >> 33)) * 0xff51afd7ed558ccd;
        h = (h ^ (h >> 33)) * 0xc4ceb9fe1a85ec53;

        return (h ^ (h >> 33));
    }
    #endregion
}