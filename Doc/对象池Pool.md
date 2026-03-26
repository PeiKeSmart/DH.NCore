# ����� Pool

## ����

`Pool<T>` �� DH.NCore �е������������ʵ�֣���������������ƣ�ͨ�� CAS ����ʵ�ָ����ܵĶ����á�����ؿ�����������Ƶ���������ٶ�������� GC ѹ�����ر��ʺϸ�Ƶ������

**�����ռ�**��`NewLife.Collections`  
**文档地址**：历史文档已归档，当前请以仓库内 Doc 为准

## ��������

- **�������**��ʹ�� `Interlocked.CompareExchange` ʵ����������
- **�ȵ��λ**������ά�����ȶ������������ٶ�
- **GC �Ѻ�**��֧�ֶ��� GC ʱ�Զ�����
- **������**��O(N) ɨ�裬�ṹ��������ñ���������
- **���ó�**���ṩ `StringBuilder`��`MemoryStream` �ȳ��ó�

## ���ٿ�ʼ

### ����ʹ��

```csharp
using NewLife.Collections;

// ���������
var pool = new Pool<MyObject>();

// ��ȡ����
var obj = pool.Get();

try
{
    // ʹ�ö���...
    obj.DoSomething();
}
finally
{
    // �黹����
    pool.Return(obj);
}
```

### ʹ������ StringBuilder ��

```csharp
using NewLife.Collections;

// �ӳ��л�ȡ StringBuilder
var sb = Pool.StringBuilder.Get();

sb.Append("Hello ");
sb.Append("World");

// �黹����ȡ���
var result = sb.Return(true);  // ���� "Hello World"

// ����Ҫ���ʱ
sb.Return(false);
```

## API �ο�

### IPool&lt;T&gt; �ӿ�

```csharp
public interface IPool<T>
{
    /// <summary>����ش�С</summary>
    Int32 Max { get; set; }
    
    /// <summary>��ȡ����</summary>
    T Get();
    
    /// <summary>�黹����</summary>
    Boolean Return(T value);
    
    /// <summary>��ն����</summary>
    Int32 Clear();
}
```

### Pool&lt;T&gt; ��

```csharp
public class Pool<T> : IPool<T> where T : class
{
    /// <summary>����ش�С��Ĭ�� CPU*2����С8</summary>
    public Int32 Max { get; set; }
    
    /// <summary>��ȡ���󣬳ؿ�ʱ������ʵ��</summary>
    public virtual T Get();
    
    /// <summary>�黹����</summary>
    public virtual Boolean Return(T value);
    
    /// <summary>��ն����</summary>
    public virtual Int32 Clear();
    
    /// <summary>�������󣨿���д��</summary>
    protected virtual T? OnCreate();
}
```

#### ���캯��

```csharp
// Ĭ�Ϲ��죬��СΪ CPU*2
var pool = new Pool<MyObject>();

// ָ����С
var pool = new Pool<MyObject>(100);

// ���� GC ������protected��
protected Pool(Int32 max, Boolean useGcClear)
```

## ���ö����

### StringBuilder ��

```csharp
public static class Pool
{
    /// <summary>�ַ�����������</summary>
    public static IPool<StringBuilder> StringBuilder { get; set; }
}
```

**ʹ��ʾ��**��
```csharp
var sb = Pool.StringBuilder.Get();
sb.Append("Name: ");
sb.Append(name);
sb.AppendLine();

// ��ʽ1���黹����ȡ���
var result = sb.Return(true);

// ��ʽ2�����黹
sb.Return(false);
```

### StringBuilderPool ��

```csharp
public class StringBuilderPool : Pool<StringBuilder>
{
    /// <summary>��ʼ������Ĭ��100</summary>
    public Int32 InitialCapacity { get; set; }
    
    /// <summary>�����������������أ�Ĭ��4K</summary>
    public Int32 MaximumCapacity { get; set; }
}
```

�黹ʱ�Զ�������ݣ�������������Ĳ�������С�

## ʹ�ó���

### 1. ��Ƶ������

```csharp
public class MessageProcessor
{
    private readonly Pool<Message> _pool = new();
    
    public void Process(Byte[] data)
    {
        var msg = _pool.Get();
        try
        {
            msg.Parse(data);
            HandleMessage(msg);
        }
        finally
        {
            msg.Reset();  // ����״̬
            _pool.Return(msg);
        }
    }
}
```

### 2. ��������

```csharp
public class BufferPool : Pool<Byte[]>
{
    public Int32 BufferSize { get; set; } = 4096;
    
    protected override Byte[] OnCreate() => new Byte[BufferSize];
    
    public override Boolean Return(Byte[] value)
    {
        // ��С��ƥ�䲻���
        if (value.Length != BufferSize) return false;
        
        // �������
        Array.Clear(value, 0, value.Length);
        
        return base.Return(value);
    }
}

// ʹ��
var bufferPool = new BufferPool { BufferSize = 8192 };
var buffer = bufferPool.Get();
try
{
    var read = stream.Read(buffer, 0, buffer.Length);
    // ��������...
}
finally
{
    bufferPool.Return(buffer);
}
```

### 3. ���ݿ����ӳ�

```csharp
public class ConnectionPool : Pool<DbConnection>
{
    public String ConnectionString { get; set; }
    
    protected override DbConnection OnCreate()
    {
        var conn = new MySqlConnection(ConnectionString);
        conn.Open();
        return conn;
    }
    
    public override Boolean Return(DbConnection value)
    {
        // �����ѶϿ������
        if (value.State != ConnectionState.Open) return false;
        
        return base.Return(value);
    }
}
```

### 4. ��ʱ����

```csharp
public class ListPool<T> : Pool<List<T>>
{
    public Int32 MaxCapacity { get; set; } = 1000;
    
    protected override List<T> OnCreate() => new(16);
    
    public override Boolean Return(List<T> value)
    {
        if (value.Capacity > MaxCapacity) return false;
        
        value.Clear();
        return base.Return(value);
    }
}

// ʹ��
var listPool = new ListPool<Int32>();
var list = listPool.Get();
try
{
    list.Add(1);
    list.Add(2);
    // ����...
}
finally
{
    listPool.Return(list);
}
```

### 5. ��� using ģʽ

```csharp
public class PooledObject<T> : IDisposable where T : class
{
    private readonly IPool<T> _pool;
    public T Value { get; }
    
    public PooledObject(IPool<T> pool)
    {
        _pool = pool;
        Value = pool.Get();
    }
    
    public void Dispose()
    {
        _pool.Return(Value);
    }
}

// ʹ��
using (var pooled = new PooledObject<StringBuilder>(Pool.StringBuilder))
{
    pooled.Value.Append("Hello");
    var result = pooled.Value.ToString();
}
```

## �Զ�������

```csharp
public class MyObjectPool : Pool<MyObject>
{
    public MyObjectPool() : base(100) { }  // �ش�С100
    
    protected override MyObject OnCreate()
    {
        // �Զ��崴���߼�
        return new MyObject
        {
            Id = Guid.NewGuid(),
            CreateTime = DateTime.Now
        };
    }
    
    public override Boolean Return(MyObject value)
    {
        // �黹ǰ���ö���
        value.Reset();
        
        // ��֤����״̬
        if (!value.IsValid) return false;
        
        return base.Return(value);
    }
}
```

## �����Ż�

### 1. Ԥ�ȶ����

```csharp
var pool = new Pool<MyObject>(50);

// Ԥ�ȣ�����һ������������
for (var i = 0; i < 50; i++)
{
    var obj = new MyObject();
    pool.Return(obj);
}
```

### 2. �������ô�С

```csharp
// ���ݲ���������
var pool = new Pool<MyObject>(Environment.ProcessorCount * 4);
```

### 3. ��������

```csharp
// ��������Ӱ�� GC������ʹ�� ArrayPool
var buffer = ArrayPool<Byte>.Shared.Rent(1024 * 1024);
try
{
    // ʹ�û�����
}
finally
{
    ArrayPool<Byte>.Shared.Return(buffer);
}
```

## �� ArrayPool �Ա�

| ���� | Pool&lt;T&gt; | ArrayPool&lt;T&gt; |
|------|--------------|-------------------|
| Ŀ������ | �������� | ���� |
| �̰߳�ȫ | �ǣ�CAS�� | �� |
| GC ���� | ��ѡ | �Զ� |
| ��С���� | �̶� | ��̬ |
| ���ó��� | ������ | ������ |

## ���ʵ��

1. **��ʱ�黹**��ʹ�� try-finally ȷ������黹
2. **����״̬**���黹ǰ���������ڲ�״̬
3. **��֤����**���黹ʱ��������Ч��
4. **������С**�����ݲ��������óش�С
5. **����й©**��ȷ���쳣·��Ҳ�ܹ黹����

## �������

- [����ϵͳ ICache](cache-����ϵͳICache.md)
- [�ַ�����չ StringHelper](string_helper-�ַ�����չStringHelper.md)
- [������չ IOHelper](io_helper-������չIOHelper.md)
