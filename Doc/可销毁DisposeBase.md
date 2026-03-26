# ������ DisposeBase

## ����

`DisposeBase` �� DH.NCore ��ʵ�� `IDisposable` ģʽ�ĳ�����࣬�ṩ��׼����Դ�ͷ�ģʽ����Ч��ֹ�ڴ����Դй©����ȷ���ͷ��߼�ִֻ��һ�Σ���֧�������¼�֪ͨ��

**�����ռ�**��`NewLife`  
**文档地址**：历史文档已归档，当前请以仓库内 Doc 为准

## ��������

- **��׼ Dispose ģʽ**����ȷʵ�� `IDisposable` �ӿ�
- **�����ͷű�֤**��ͨ��ԭ�Ӳ���ȷ���ͷ��߼�ִֻ��һ��
- **�ս���֧��**�������ǵ��� `Dispose` ʱ�ṩ�����ͷ�
- **�����¼�**��֧���ڶ�������ʱ�����¼�֪ͨ
- **��������**���ṩ `ThrowIfDisposed` �� `TryDispose` �ȸ�������

## ���ٿ�ʼ

```csharp
using NewLife;

// �̳� DisposeBase ʵ����Դ����
public class MyResource : DisposeBase
{
    private Stream _stream;
    
    public MyResource(String path)
    {
        _stream = File.OpenRead(path);
    }
    
    protected override void Dispose(Boolean disposing)
    {
        base.Dispose(disposing);
        
        if (disposing)
        {
            // �ͷ��й���Դ
            _stream?.Dispose();
            _stream = null;
        }
    }
}

// ʹ�� using ����Զ��ͷ�
using var resource = new MyResource("data.txt");
```

## API �ο�

### IDisposable2 �ӿ�

```csharp
public interface IDisposable2 : IDisposable
{
    Boolean Disposed { get; }
    event EventHandler OnDisposed;
}
```

��չ�Ŀ����ٽӿڣ������� `Disposed` ״̬���Ժ������¼���

### DisposeBase ��

#### ����

##### Disposed

```csharp
public Boolean Disposed { get; }
```

��ʾ�����Ƿ��ѱ��ͷš�

**ʾ��**��
```csharp
var resource = new MyResource();
Console.WriteLine(resource.Disposed);  // False

resource.Dispose();
Console.WriteLine(resource.Disposed);  // True
```

#### ����

##### Dispose

```csharp
public void Dispose()
```

�ͷ���Դ�����ú�ᴥ�� `OnDisposed` �¼�����֪ͨ GC ���ٵ����ս�����

##### Dispose(Boolean)

```csharp
protected virtual void Dispose(Boolean disposing)
```

ʵ�ʵ���Դ�ͷŷ������������ش˷���ʵ�־�����ͷ��߼���

**����˵��**��
- `disposing`��`true` ��ʾ�� `Dispose()` �������ã�Ӧ�ͷ�������Դ��`false` ��ʾ���ս������ã�ֻӦ�ͷŷ��й���Դ

**����ʾ��**��
```csharp
protected override void Dispose(Boolean disposing)
{
    // 1. ���ȵ��û��෽��
    base.Dispose(disposing);
    
    // 2. �ͷ��й���Դ�������� Dispose() ����ʱ��
    if (disposing)
    {
        _managedResource?.Dispose();
        _managedResource = null;
    }
    
    // 3. �ͷŷ��й���Դ������·����ִ�У�
    if (_handle != IntPtr.Zero)
    {
        CloseHandle(_handle);
        _handle = IntPtr.Zero;
    }
}
```

##### ThrowIfDisposed

```csharp
protected void ThrowIfDisposed()
```

�ڹ��������е��ã����������ͷ����׳� `ObjectDisposedException`��

**ʾ��**��
```csharp
public class MyResource : DisposeBase
{
    public void DoWork()
    {
        ThrowIfDisposed();  // ���ͷ�ʱ�׳��쳣
        
        // ִ��ʵ�ʹ���...
    }
}
```

#### �¼�

##### OnDisposed

```csharp
public event EventHandler? OnDisposed
```

��������ʱ�������¼���

**ע��**���¼��������ս����߳��д��������ķ�Ӧ���������ض��߳������ġ�

**ʾ��**��
```csharp
var resource = new MyResource();
resource.OnDisposed += (sender, e) =>
{
    Console.WriteLine("��Դ���ͷ�");
};

resource.Dispose();  // �������Դ���ͷ�
```

### DisposeHelper ������

#### TryDispose

```csharp
public static Object? TryDispose(this Object? obj)
```

�������ٶ����������ʵ���� `IDisposable` ������� `Dispose` ������֧�ּ������ͣ��������������Ԫ�ء�

**ʾ��**��
```csharp
// ���ٵ�������
var stream = File.OpenRead("test.txt");
stream.TryDispose();

// ���ټ����е����ж���
var streams = new List<Stream>
{
    File.OpenRead("a.txt"),
    File.OpenRead("b.txt"),
    File.OpenRead("c.txt")
};
streams.TryDispose();  // ����������
```

## ʹ�ó���

### 1. �����ļ����

```csharp
public class FileProcessor : DisposeBase
{
    private FileStream? _fileStream;
    private StreamReader? _reader;
    
    public FileProcessor(String path)
    {
        _fileStream = File.OpenRead(path);
        _reader = new StreamReader(_fileStream);
    }
    
    public String ReadLine()
    {
        ThrowIfDisposed();
        return _reader?.ReadLine() ?? String.Empty;
    }
    
    protected override void Dispose(Boolean disposing)
    {
        base.Dispose(disposing);
        
        if (disposing)
        {
            _reader?.Dispose();
            _reader = null;
            _fileStream?.Dispose();
            _fileStream = null;
        }
    }
}
```

### 2. ������������

```csharp
public class TcpConnection : DisposeBase
{
    private Socket? _socket;
    private NetworkStream? _stream;
    
    public TcpConnection(String host, Int32 port)
    {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _socket.Connect(host, port);
        _stream = new NetworkStream(_socket, ownsSocket: false);
    }
    
    public void Send(Byte[] data)
    {
        ThrowIfDisposed();
        _stream?.Write(data, 0, data.Length);
    }
    
    protected override void Dispose(Boolean disposing)
    {
        base.Dispose(disposing);
        
        if (disposing)
        {
            _stream?.Dispose();
            _stream = null;
            _socket?.Dispose();
            _socket = null;
        }
    }
}
```

### 3. ��Դ�ع���

```csharp
public class ResourcePool<T> : DisposeBase where T : IDisposable
{
    private readonly ConcurrentBag<T> _pool = new();
    private readonly Func<T> _factory;
    
    public ResourcePool(Func<T> factory)
    {
        _factory = factory;
    }
    
    public T Rent()
    {
        ThrowIfDisposed();
        return _pool.TryTake(out var item) ? item : _factory();
    }
    
    public void Return(T item)
    {
        if (Disposed)
        {
            item.Dispose();
            return;
        }
        _pool.Add(item);
    }
    
    protected override void Dispose(Boolean disposing)
    {
        base.Dispose(disposing);
        
        if (disposing)
        {
            while (_pool.TryTake(out var item))
            {
                item.Dispose();
            }
        }
    }
}
```

### 4. ���������¼�

```csharp
public class ResourceMonitor
{
    public void Monitor(IDisposable2 resource)
    {
        resource.OnDisposed += (sender, e) =>
        {
            var name = sender?.GetType().Name;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {name} ���ͷ�");
        };
    }
}

// ʹ��
var monitor = new ResourceMonitor();
var resource = new MyResource();
monitor.Monitor(resource);

resource.Dispose();  // �����[12:30:45] MyResource ���ͷ�
```

## ���ʵ��

### 1. ���ǵ��û��෽��

```csharp
protected override void Dispose(Boolean disposing)
{
    // ? ���ȵ��û��෽��
    base.Dispose(disposing);
    
    // Ȼ���ͷ��Լ�����Դ
    if (disposing) { /* ... */ }
}
```

### 2. ʹ�� using ���

```csharp
// ? �Ƽ���ʹ�� using ���ȷ���ͷ�
using var resource = new MyResource();

// ? ���Ƽ����ֶ����� Dispose����������
var resource = new MyResource();
try
{
    // ʹ����Դ
}
finally
{
    resource.Dispose();
}
```

### 3. �ڹ��������м��״̬

```csharp
public class MyResource : DisposeBase
{
    public void DoWork()
    {
        // ? �ڷ�����ʼʱ����Ƿ����ͷ�
        ThrowIfDisposed();
        
        // ִ��ʵ�ʹ���
    }
}
```

### 4. ��ȷ�����йܺͷ��й���Դ

```csharp
protected override void Dispose(Boolean disposing)
{
    base.Dispose(disposing);
    
    // �й���Դ������ disposing=true ʱ�ͷ�
    if (disposing)
    {
        _managedObject?.Dispose();
    }
    
    // ���й���Դ�����������Ҫ�ͷ�
    if (_nativeHandle != IntPtr.Zero)
    {
        NativeMethods.CloseHandle(_nativeHandle);
        _nativeHandle = IntPtr.Zero;
    }
}
```

### 5. �������ս������׳��쳣

```csharp
// DisposeBase �Ѿ��������ս����е��쳣
// ����� Dispose(Boolean) ����ҲӦ�ò�����ܵ��쳣
protected override void Dispose(Boolean disposing)
{
    base.Dispose(disposing);
    
    try
    {
        // �ͷ���Դ
    }
    catch (Exception ex)
    {
        // ��¼�����׳�
        XTrace.WriteException(ex);
    }
}
```

## Dispose ģʽ���

```
���� Dispose()
       ��
       ��
  Dispose(true)
       ��
       ������? �ͷ��й���Դ
       ��
       ������? �ͷŷ��й���Դ
       ��
       ������? ���� OnDisposed �¼�
       ��
       ������? GC.SuppressFinalize(this)
              ��
              ������? �ս������ٱ�����


�ս��� ~DisposeBase()  (������ǵ��� Dispose)
       ��
       ��
  Dispose(false)
       ��
       ������? �ͷŷ��й���Դ
       ��
       ������? ���� OnDisposed �¼�
```

## �������

- [����� ObjectPool](object_pool-�����ObjectPool.md)
- [�������� ObjectContainer](object_container-��������ObjectContainer.md)
