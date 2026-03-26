# ����ϵͳ ICache

## ����

DH.NCore �ṩ��ͳһ�Ļ���ӿ� `ICache`��֧���ڴ滺�桢Redis ����ȶ���ʵ�֡�ͨ��ͳһ�ӿڣ������ڲ�ͬ�������޷��л�����ʵ�֣�ͬʱ֧�ֹ���ʱ�䡢ԭ�Ӳ��������������ȸ߼����ԡ�

**�����ռ�**��`NewLife.Caching`  
**文档地址**：历史文档已归档，当前请以仓库内 Doc 为准

## ��������

- **ͳһ�ӿ�**��`ICache` �ӿڶ����׼�������
- **������**��`MemoryCache` ���� `ConcurrentDictionary`����ֵ���ܴ� 10 �� ops
- **�Զ�����**��֧����Թ���ʱ�䣨TTL��
- **ԭ�Ӳ���**���������ݼ����滻��ԭ�Ӳ���
- **��������**��������д�������翪��
- **LRU ��̭**���ڴ滺�泬����ʱ�Զ�����

## ���ٿ�ʼ

### ����ʹ��

```csharp
using NewLife.Caching;

// ʹ��Ĭ���ڴ滺��
var cache = MemoryCache.Instance;

// ���û��棨60����ڣ�
cache.Set("name", "����", 60);

// ��ȡ����
var name = cache.Get<String>("name");

// ɾ������
cache.Remove("name");
```

### ���ò���

```csharp
var cache = MemoryCache.Instance;

// ����Ƿ����
if (cache.ContainsKey("user:1"))
{
    var user = cache.Get<User>("user:1");
}

// ��ȡ�����ӣ����洩͸������
var data = cache.GetOrAdd("data:key", k =>
{
    // ���治����ʱ��ִ�д˻ص���ȡ����
    return LoadFromDatabase(k);
}, 300);

// ԭ�ӵ���
var count = cache.Increment("visit:count", 1);
```

## API �ο�

### ICache �ӿ�

#### ��������

```csharp
/// <summary>��������</summary>
String Name { get; }

/// <summary>Ĭ�Ϲ���ʱ�䣨�룩</summary>
Int32 Expire { get; set; }

/// <summary>����������</summary>
Int32 Count { get; }

/// <summary>���л����</summary>
ICollection<String> Keys { get; }
```

#### ��������

```csharp
/// <summary>����Ƿ����</summary>
Boolean ContainsKey(String key);

/// <summary>���û���</summary>
/// <param name="expire">����������-1ʹ��Ĭ�ϣ�0��������</param>
Boolean Set<T>(String key, T value, Int32 expire = -1);

/// <summary>���û��棨TimeSpan��</summary>
Boolean Set<T>(String key, T value, TimeSpan expire);

/// <summary>��ȡ����</summary>
T Get<T>(String key);

/// <summary>���Ի�ȡ��������洩͸��</summary>
Boolean TryGetValue<T>(String key, out T value);

/// <summary>ɾ������</summary>
Int32 Remove(String key);

/// <summary>����ɾ��</summary>
Int32 Remove(params String[] keys);

/// <summary>������л���</summary>
void Clear();
```

#### ����ʱ�����

```csharp
/// <summary>���ù���ʱ��</summary>
Boolean SetExpire(String key, TimeSpan expire);

/// <summary>��ȡʣ�����ʱ��</summary>
TimeSpan GetExpire(String key);
```

#### ��������

```csharp
/// <summary>������ȡ</summary>
IDictionary<String, T?> GetAll<T>(IEnumerable<String> keys);

/// <summary>��������</summary>
void SetAll<T>(IDictionary<String, T> values, Int32 expire = -1);
```

#### �߼�����

```csharp
/// <summary>���ӣ��Ѵ���ʱ�����£�</summary>
Boolean Add<T>(String key, T value, Int32 expire = -1);

/// <summary>�滻�����ؾ�ֵ</summary>
T Replace<T>(String key, T value);

/// <summary>��ȡ������</summary>
T GetOrAdd<T>(String key, Func<String, T> callback, Int32 expire = -1);

/// <summary>ԭ�ӵ���</summary>
Int64 Increment(String key, Int64 value);
Double Increment(String key, Double value);

/// <summary>ԭ�ӵݼ�</summary>
Int64 Decrement(String key, Int64 value);
Double Decrement(String key, Double value);
```

### MemoryCache ��

```csharp
public class MemoryCache : Cache
{
    /// <summary>Ĭ��ʵ��</summary>
    public static MemoryCache Instance { get; set; }
    
    /// <summary>����������ʱLRU��̭��Ĭ��100000</summary>
    public Int32 Capacity { get; set; }
    
    /// <summary>��ʱ����������룩��Ĭ��60</summary>
    public Int32 Period { get; set; }
    
    /// <summary>����������¼�</summary>
    public event EventHandler<KeyEventArgs>? KeyExpired;
}
```

## ʹ�ó���

### 1. ���ݻ���

```csharp
public class UserService
{
    private readonly ICache _cache;
    
    public UserService(ICache cache)
    {
        _cache = cache;
    }
    
    public User? GetUser(Int32 id)
    {
        var key = $"user:{id}";
        
        // �Ȳ黺��
        if (_cache.TryGetValue<User>(key, out var user))
            return user;
        
        // ����δ���У������ݿ�
        user = LoadUserFromDb(id);
        
        if (user != null)
            _cache.Set(key, user, 300);  // ����5����
        
        return user;
    }
}
```

### 2. ��ֹ���洩͸

```csharp
// ʹ�� GetOrAdd ��ֹ���洩͸
var user = cache.GetOrAdd($"user:{id}", key =>
{
    // ��ʹ���ݿⷵ�� null��Ҳ�ᱻ����
    return LoadUserFromDb(id);
}, 60);

// ʹ�� TryGetValue ���ֿ�ֵ�Ͳ�����
if (cache.TryGetValue<User?>($"user:{id}", out var user))
{
    // �����ڣ�����Ϊ null��
    return user;
}
else
{
    // �������ڣ���Ҫ��ѯ���ݿ�
}
```

### 3. ������

```csharp
// ���ʼ���
var count = cache.Increment("page:home:views", 1);

// ��������
var requests = cache.Increment($"rate:{userId}", 1);
if (requests == 1)
{
    // �״η��ʣ����ù���ʱ��
    cache.SetExpire($"rate:{userId}", TimeSpan.FromMinutes(1));
}
if (requests > 100)
{
    throw new Exception("�������Ƶ��");
}
```

### 4. �ֲ�ʽ������ʵ��

```csharp
public class SimpleLock
{
    private readonly ICache _cache;
    
    public Boolean TryLock(String key, Int32 seconds = 30)
    {
        // Add ֻ�ڲ�����ʱ�ɹ�
        return _cache.Add($"lock:{key}", DateTime.Now, seconds);
    }
    
    public void Unlock(String key)
    {
        _cache.Remove($"lock:{key}");
    }
}

// ʹ��
var locker = new SimpleLock(cache);
if (locker.TryLock("order:create"))
{
    try
    {
        // ִ��ҵ��
    }
    finally
    {
        locker.Unlock("order:create");
    }
}
```

### 5. �Ự����

```csharp
public class SessionCache
{
    private readonly ICache _cache;
    private readonly Int32 _expire = 1800;  // 30����
    
    public void Set(String sessionId, Object data)
    {
        _cache.Set($"session:{sessionId}", data, _expire);
    }
    
    public T? Get<T>(String sessionId)
    {
        var key = $"session:{sessionId}";
        var data = _cache.Get<T>(key);
        
        if (data != null)
        {
            // ����
            _cache.SetExpire(key, TimeSpan.FromSeconds(_expire));
        }
        
        return data;
    }
}
```

### 6. ���������Ż�

```csharp
// ������ȡ
var keys = new[] { "user:1", "user:2", "user:3" };
var users = cache.GetAll<User>(keys);

foreach (var kv in users)
{
    Console.WriteLine($"{kv.Key}: {kv.Value?.Name}");
}

// ��������
var items = new Dictionary<String, User>
{
    ["user:1"] = new User { Id = 1, Name = "����" },
    ["user:2"] = new User { Id = 2, Name = "����" }
};
cache.SetAll(items, 300);
```

## ����ʱ��˵��

| expire ֵ | ���� |
|-----------|------|
| < 0 | ʹ��Ĭ�Ϲ���ʱ�� `Expire` |
| = 0 | �������� |
| > 0 | �������� N ������ |

```csharp
// ʹ��Ĭ�Ϲ���ʱ��
cache.Set("key1", value);

// ��������
cache.Set("key2", value, 0);

// 1Сʱ�����
cache.Set("key3", value, 3600);

// ʹ�� TimeSpan
cache.Set("key4", value, TimeSpan.FromHours(1));
```

## ����ע��

```csharp
// ע�Ỻ�����
services.AddSingleton<ICache>(MemoryCache.Instance);

// �򴴽���ʵ��
services.AddSingleton<ICache>(sp => new MemoryCache
{
    Capacity = 50000,
    Expire = 600
});
```

## ������淶

����ʹ��ð�ŷָ��Ĳ㼶������

```
����:��ʶ[:������]
user:123
user:123:profile
order:2024:001
config:app:debug
```

## ���ʵ��

### 1. ������������

```csharp
var cache = new MemoryCache
{
    Capacity = 100000,  // �����ڴ����
    Period = 60         // �������
};
```

### 2. ���������¼�

```csharp
var cache = new MemoryCache();
cache.KeyExpired += (s, e) =>
{
    XTrace.WriteLine($"�������: {e.Key}");
    // ���������ﴥ������Ԥ��
};
```

### 3. ����� Key

```csharp
// ���Ƽ����洢�����
cache.Set("bigdata", hugeList);

// �Ƽ�����ִ洢
foreach (var item in hugeList)
{
    cache.Set($"item:{item.Id}", item);
}
```

### 4. ʹ��ǰ׺����

```csharp
// ��ͬģ��ʹ�ò�ͬǰ׺
cache.Set("user:session:abc", data);
cache.Set("order:temp:123", data);

// ��ǰ׺����ɾ��
cache.Remove("user:session:*");
```

## �������

- [����� Pool](pool-�����Pool.md)
- [�ֵ仺�� DictionaryCache](dictionary_cache-�ֵ仺��.md)
- [ѩ���㷨 Snowflake](snowflake-ѩ���㷨Snowflake.md)
