namespace NewLife.Caching;

/// <summary>分布式锁</summary>
public class CacheLock : DisposeBase
{
    private ICache Client { get; set; }

    private Boolean _hasLock;
    /// <summary>是否持有锁</summary>
    public Boolean HasLock => _hasLock;

    private String _token = "";
    /// <summary>锁令牌。用于释放时校验归属，避免误删超时后被其它进程接管的锁</summary>
    public String Token => _token;

    /// <summary>键</summary>
    public String Key { get; set; }

    /// <summary>实例化</summary>
    /// <param name="client">缓存客户端</param>
    /// <param name="key">锁键</param>
    public CacheLock(ICache client, String key)
    {
        if (client == null) throw new ArgumentNullException(nameof(client));
        if (key.IsNullOrEmpty()) throw new ArgumentNullException(nameof(key));

        Client = client;
        Key = key;
    }

    /// <summary>申请锁</summary>
    /// <param name="msTimeout">锁等待时间，申请加锁时如果遇到冲突则等待的最大时间，单位毫秒</param>
    /// <param name="msExpire">锁过期时间，超过该时间如果没有主动释放则自动释放锁，必须整数秒，单位毫秒</param>
    /// <returns>是否成功获取锁</returns>
    public Boolean Acquire(Int32 msTimeout, Int32 msExpire)
    {
        var ch = Client;
        var now = Runtime.TickCount64;

        // 生成唯一令牌，释放时校验归属，避免锁超时被接管后误删新持有者的锁
        var token = Guid.NewGuid().ToString("N");
        // 锁值格式：令牌|绝对过期毫秒。缓存TTL向上取整，避免小于1秒的锁因取整为0而立即失效
        var value = $"{token}|{now + msExpire}";
        var expire = msExpire > 0 ? (msExpire + 999) / 1000 : 0;

        // 循环等待。至少尝试一次，msTimeout=0 时也允许直接抢锁（否则空闲锁也永远拿不到）
        var end = now + msTimeout;
        while (true)
        {
            // 申请加锁。没有冲突时可以直接返回
            var rs = ch.Add(Key, value, expire);
            if (rs)
            {
                _token = token;
                return _hasLock = true;
            }

            // 死锁超期检测
            var dt = ParseExpire(ch.Get<String>(Key));
            if (dt <= now)
            {
                // 开抢死锁。所有竞争者都会修改该锁的时间戳，但是只有一个能拿到旧的超时的值
                var old = ParseExpire(ch.Replace(Key, value));
                // 如果拿到超时值，说明抢到了锁。其它线程会抢到一个为超时的值
                if (old <= dt)
                {
                    ch.SetExpire(Key, TimeSpan.FromMilliseconds(msExpire));
                    _token = token;
                    return _hasLock = true;
                }
            }

            // 超时退出
            now = Runtime.TickCount64;
            if (now >= end) break;

            // 没抢到，继续
            Thread.Sleep(200);
        }

        return false;
    }

    /// <summary>解析锁值中的过期时间戳。兼容旧格式（纯数字时间戳）</summary>
    /// <param name="value">锁值</param>
    /// <returns>过期时间戳（毫秒）</returns>
    private static Int64 ParseExpire(String? value)
    {
        if (value.IsNullOrEmpty()) return 0;

        var p = value.IndexOf('|');
        var str = p > 0 ? value[(p + 1)..] : value;

        return str.ToLong();
    }

    /// <summary>销毁</summary>
    /// <param name="disposing"></param>
    protected override void Dispose(Boolean disposing)
    {
        base.Dispose(disposing);

        // 如果客户端已释放，则不删除
        if (Client is DisposeBase db && db.Disposed)
        {
        }
        else
        {
            if (_hasLock && !_token.IsNullOrEmpty())
            {
                // 仅当锁仍由本实例持有时才删除，避免误删超时后被其它进程接管的锁
                var str = Client.Get<String>(Key);
                if (str != null && str.StartsWith(_token + "|")) Client.Remove(Key);
            }
        }
    }
}