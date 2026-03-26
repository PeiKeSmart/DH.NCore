# ����ģ�� Actor

## ����

`Actor` �� DH.NCore �е��������б��ģ�ͣ�������Ϣ����ʵ���̰߳�ȫ���첽������ÿ�� Actor ӵ�ж�������Ϣ����ʹ����̣߳�ͨ����Ϣ���ݽ���ͨ�ţ������˴�ͳ�����ƴ����ĸ����Ժ��������⡣

**�����ռ�**��`NewLife.Model`  
**文档地址**：历史文档已归档，当前请以仓库内 Doc 为准

## ��������

- **�������**��ͨ����Ϣ���и���״̬��������ʽ����
- **�����߳�**��ÿ�� Actor ʹ�ö����̴߳�����Ϣ����Ӱ���̳߳�
- **��������**��֧������������Ϣ�����������
- **��������**��֧��������Ϣ���������������ֹ�ڴ����
- **�Զ�����**��������Ϣʱ�Զ����� Actor
- **����׷��**������ `ITracer` ֧����·׷��

## ���ٿ�ʼ

```csharp
using NewLife.Model;

// ���� Actor
public class MyActor : Actor
{
    protected override Task ReceiveAsync(ActorContext context, CancellationToken cancellationToken)
    {
        var message = context.Message;
        Console.WriteLine($"�յ���Ϣ: {message}");
        return Task.CompletedTask;
    }
}

// ʹ�� Actor
var actor = new MyActor();

// ������Ϣ
actor.Tell("Hello");
actor.Tell("World");

// �ȴ�������ɺ�ֹͣ
actor.Stop(5000);
```

## API �ο�

### IActor �ӿ�

```csharp
public interface IActor
{
    /// <summary>������Ϣ�������ڲ�����</summary>
    /// <param name="message">��Ϣ����</param>
    /// <param name="sender">������Actor</param>
    /// <returns>���ش�������Ϣ��</returns>
    Int32 Tell(Object message, IActor? sender = null);
}
```

### ActorContext ��

```csharp
public class ActorContext
{
    /// <summary>������</summary>
    public IActor? Sender { get; set; }
    
    /// <summary>��Ϣ</summary>
    public Object? Message { get; set; }
}
```

### Actor ����

#### ����

```csharp
/// <summary>����</summary>
public String Name { get; set; }

/// <summary>�Ƿ�����</summary>
public Boolean Active { get; }

/// <summary>�������������ɶѻ�����Ϣ����Ĭ��Int32.MaxValue</summary>
public Int32 BoundedCapacity { get; set; }

/// <summary>����С��ÿ�δ�����Ϣ����Ĭ��1</summary>
public Int32 BatchSize { get; set; }

/// <summary>�Ƿ�ʱ�����С�Ĭ��true��ʹ�ö����߳�</summary>
public Boolean LongRunning { get; set; }

/// <summary>��ǰ���г���</summary>
public Int32 QueueLength { get; }

/// <summary>����׷����</summary>
public ITracer? Tracer { get; set; }
```

#### Tell - ������Ϣ

```csharp
public virtual Int32 Tell(Object message, IActor? sender = null)
```

�� Actor ������Ϣ����� Actor δ���������Զ�������

**����**��
- `message`����Ϣ���󣬿�������������
- `sender`�������� Actor�����ڻظ���Ϣ

**����ֵ**����ǰ����������Ϣ��

**ʾ��**��
```csharp
var actor = new MyActor();

// ���ͼ���Ϣ
actor.Tell("Hello");

// ���͸��Ӷ���
actor.Tell(new { Id = 1, Name = "Test" });

// ��������
actor.Tell("Ping", senderActor);
```

#### Start - ���� Actor

```csharp
public virtual Task? Start()
public virtual Task? Start(CancellationToken cancellationToken)
```

�ֶ����� Actor��ͨ������Ҫ�ֶ����ã�`Tell` ���Զ�������

**ʾ��**��
```csharp
var actor = new MyActor();

// �ֶ�����
actor.Start();

// ��ȡ����������
using var cts = new CancellationTokenSource();
actor.Start(cts.Token);
```

#### Stop - ֹͣ Actor

```csharp
public virtual Boolean Stop(Int32 msTimeout = 0)
```

ֹͣ Actor�����ٽ�������Ϣ��

**����**��
- `msTimeout`���ȴ���������0=���ȴ���-1=���޵ȴ�

**����ֵ**���Ƿ��ڳ�ʱǰ���������Ϣ����

**ʾ��**��
```csharp
// ����ֹͣ�����ȴ�
actor.Stop(0);

// �ȴ����5��
var completed = actor.Stop(5000);
if (!completed)
    Console.WriteLine("����Ϣδ�������");

// ���޵ȴ�
actor.Stop(-1);
```

#### ReceiveAsync - ������Ϣ

```csharp
// ����������BatchSize=1��
protected virtual Task ReceiveAsync(ActorContext context, CancellationToken cancellationToken)

// ����������BatchSize>1��
protected virtual Task ReceiveAsync(ActorContext[] contexts, CancellationToken cancellationToken)
```

������д�˷���ʵ����Ϣ�����߼���

## ʹ�ó���

### 1. ��־�ռ���

```csharp
public class LogActor : Actor
{
    private readonly StreamWriter _writer;
    
    public LogActor(String filePath)
    {
        Name = "LogActor";
        BatchSize = 100;  // ����д��
        BoundedCapacity = 10000;  // ���ƶ���
        
        _writer = new StreamWriter(filePath, true) { AutoFlush = false };
    }
    
    protected override async Task ReceiveAsync(ActorContext[] contexts, CancellationToken cancellationToken)
    {
        foreach (var ctx in contexts)
        {
            if (ctx.Message is String log)
            {
                await _writer.WriteLineAsync(log);
            }
        }
        await _writer.FlushAsync();
    }
    
    protected override void Dispose(Boolean disposing)
    {
        base.Dispose(disposing);
        _writer?.Dispose();
    }
}

// ʹ��
var logger = new LogActor("app.log");
logger.Tell($"[{DateTime.Now:HH:mm:ss}] Ӧ������");
logger.Tell($"[{DateTime.Now:HH:mm:ss}] ��������");
```

### 2. ��Ϣ������

```csharp
public class MessageProcessor : Actor
{
    private readonly IMessageHandler _handler;
    
    public MessageProcessor(IMessageHandler handler)
    {
        Name = "MessageProcessor";
        _handler = handler;
    }
    
    protected override async Task ReceiveAsync(ActorContext context, CancellationToken cancellationToken)
    {
        if (context.Message is Message msg)
        {
            try
            {
                await _handler.HandleAsync(msg, cancellationToken);
                
                // �ظ�������
                context.Sender?.Tell(new Ack { MessageId = msg.Id });
            }
            catch (Exception ex)
            {
                context.Sender?.Tell(new Error { MessageId = msg.Id, Exception = ex });
            }
        }
    }
}
```

### 3. ���ݾۺ���

```csharp
public class DataAggregator : Actor
{
    private readonly Dictionary<String, Int32> _counts = new();
    private DateTime _lastFlush = DateTime.Now;
    
    public DataAggregator()
    {
        Name = "DataAggregator";
        BatchSize = 50;
    }
    
    protected override Task ReceiveAsync(ActorContext[] contexts, CancellationToken cancellationToken)
    {
        foreach (var ctx in contexts)
        {
            if (ctx.Message is String key)
            {
                _counts.TryGetValue(key, out var count);
                _counts[key] = count + 1;
            }
        }
        
        // ÿ�������һ��ͳ��
        if ((DateTime.Now - _lastFlush).TotalMinutes >= 1)
        {
            foreach (var kv in _counts)
            {
                Console.WriteLine($"{kv.Key}: {kv.Value}");
            }
            _counts.Clear();
            _lastFlush = DateTime.Now;
        }
        
        return Task.CompletedTask;
    }
}
```

### 4. Actor ֮��ͨ��

```csharp
public class PingActor : Actor
{
    protected override Task ReceiveAsync(ActorContext context, CancellationToken cancellationToken)
    {
        if (context.Message is String msg && msg == "Ping")
        {
            Console.WriteLine("PingActor �յ� Ping������ Pong");
            context.Sender?.Tell("Pong", this);
        }
        return Task.CompletedTask;
    }
}

public class PongActor : Actor
{
    protected override Task ReceiveAsync(ActorContext context, CancellationToken cancellationToken)
    {
        if (context.Message is String msg && msg == "Pong")
        {
            Console.WriteLine("PongActor �յ� Pong");
        }
        return Task.CompletedTask;
    }
}

// ʹ��
var ping = new PingActor();
var pong = new PongActor();

// pong ���� Ping �� ping��ping ��ظ� Pong
ping.Tell("Ping", pong);
```

### 5. ����������

```csharp
public class RateLimitedActor : Actor
{
    private readonly SemaphoreSlim _semaphore;
    private readonly Int32 _maxConcurrency;
    
    public RateLimitedActor(Int32 maxConcurrency = 10)
    {
        _maxConcurrency = maxConcurrency;
        _semaphore = new SemaphoreSlim(maxConcurrency);
        BatchSize = maxConcurrency;
    }
    
    protected override async Task ReceiveAsync(ActorContext[] contexts, CancellationToken cancellationToken)
    {
        var tasks = new List<Task>();
        
        foreach (var ctx in contexts)
        {
            await _semaphore.WaitAsync(cancellationToken);
            
            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    await ProcessAsync(ctx.Message, cancellationToken);
                }
                finally
                {
                    _semaphore.Release();
                }
            }, cancellationToken));
        }
        
        await Task.WhenAll(tasks);
    }
    
    private async Task ProcessAsync(Object? message, CancellationToken cancellationToken)
    {
        // �����߼�
        await Task.Delay(100, cancellationToken);
    }
}
```

## ���ʵ��

### 1. ������������С

```csharp
// IO�ܼ��ͣ��ϴ�����
var ioActor = new IoActor { BatchSize = 100 };

// CPU�ܼ��ͣ���С����
var cpuActor = new CpuActor { BatchSize = 10 };

// ʵʱ��Ҫ��ߣ���������
var realtimeActor = new RealtimeActor { BatchSize = 1 };
```

### 2. ���ö�������

```csharp
// ��ֹ�ڴ����
var actor = new MyActor
{
    BoundedCapacity = 10000  // ���ѻ�1������Ϣ
};

// �����г���
if (actor.QueueLength > 5000)
{
    Console.WriteLine("���棺��Ϣ��ѹ");
}
```

### 3. ����ֹͣ

```csharp
// ֹͣ��������Ϣ���ȴ�������Ϣ�������
var completed = actor.Stop(30_000);  // ����30��

if (!completed)
{
    Console.WriteLine($"�� {actor.QueueLength} ����Ϣδ����");
}
```

### 4. �쳣����

```csharp
public class SafeActor : Actor
{
    protected override async Task ReceiveAsync(ActorContext context, CancellationToken cancellationToken)
    {
        try
        {
            await ProcessAsync(context.Message);
        }
        catch (Exception ex)
        {
            // ��¼��־�����׳��쳣
            XTrace.WriteException(ex);
            
            // ��ѡ�����͵����Ŷ���
            DeadLetterActor?.Tell(new DeadLetter
            {
                Message = context.Message,
                Exception = ex
            });
        }
    }
}
```

### 5. ����׷��

```csharp
var actor = new MyActor
{
    Tracer = new DefaultTracer()  // ��ʹ���ǳ�׷��
};

// ׷����Ϣ���Զ���¼��
// - actor:Start
// - actor:Loop
// - actor:Stop
```

## ����������ģ�ͶԱ�

| ���� | Actor | Task/async | �� |
|------|-------|------------|-----|
| �̰߳�ȫ | ��Ȼ��ȫ | ��Ҫע�� | ��Ҫ��ʽ���� |
| ��̸��Ӷ� | �е� | �� | �� |
| ���ó��� | IO�ܼ� | ͨ�� | ����״̬ |
| ��ѹ���� | ֧�� | ��֧�� | ��֧�� |
| ��Ϣ˳�� | ��֤ | ����֤ | ������ |

## �������

- [�߼���ʱ�� TimerX](timerx-�߼���ʱ��TimerX.md)
- [������Ӧ������ Host](host-������Ӧ������Host.md)
- [��·׷�� ITracer](tracer-��·׷��ITracer.md)
