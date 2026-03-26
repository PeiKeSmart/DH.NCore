# ApiHttpClient ʹ���ֲ�

## ����

`ApiHttpClient` �� DH.NCore �ṩ�� Http Ӧ�ýӿڿͻ��ˣ��ǶԶ�������ַ�İ�װ�����ڵײ������� `HttpClient`���ṩͳһ�ĸ��ؾ���͹���ת��������

### ��������

- **���ַ����**��֧�����ö�������ַ���Զ����и��ؾ���
- **����ת��**���ڵ㲻����ʱ�Զ��л������ýڵ�
- **���ؾ���**��֧�ֹ���ת�ơ���Ȩ��ѯ�����ٵ�������ģʽ
- **���Ƽ�Ȩ**��֧�� Token �� Authentication ���ּ�Ȩ��ʽ
- **��Ӧ����**��֧���Զ���״̬��������ֶ����ƣ����䲻ͬƽ̨
- **����չ��**��֧���Զ��� JsonHost��Filter���¼���

## ���ٿ�ʼ

### �����÷�

```csharp
// �����ͻ���
var client = new ApiHttpClient("http://api.example.com");

// GET ����
var result = await client.GetAsync<UserInfo>("user/info", new { id = 123 });

// POST ����
var response = await client.PostAsync<ResultModel>("user/create", new { name = "test", age = 18 });

// ͬ������
var data = client.Get<String>("api/data");
```

### ���ַ����

```csharp
// ���ŷָ������ַ
var client = new ApiHttpClient("http://api1.example.com,http://api2.example.com,http://api3.example.com");

// �����ֶ�����
var client = new ApiHttpClient();
client.Add("primary", "http://api1.example.com");
client.Add("backup", "http://api2.example.com");
```

## ���ؾ���

### ���ָ��ؾ���ģʽ

| ģʽ | ö��ֵ | ˵�� |
|------|--------|------|
| ����ת�� | `LoadBalanceMode.Failover` | ����ʹ�����ڵ㣬ʧ��ʱ�Զ��л������ýڵ㣬��һ��ʱ���Զ��л� |
| ��Ȩ��ѯ | `LoadBalanceMode.RoundRobin` | ��Ȩ�ط������󵽶���ڵ㣬�Զ����β����ýڵ� |
| ���ٵ��� | `LoadBalanceMode.Race` | �����������ڵ㣬ȡ�����Ӧ��ȡ���������� |

### ����ת��ģʽ��Ĭ�ϣ�

```csharp
var client = new ApiHttpClient("http://primary.example.com,http://backup.example.com")
{
    LoadBalanceMode = LoadBalanceMode.Failover,  // Ĭ��ֵ
    ShieldingTime = 60  // �����ýڵ�����60��
};

// �������ʹ�� primary��primary ������ʱ�Զ��л��� backup
// 60���᳢���л� primary
var result = await client.GetAsync<Object>("api/data");
```

### ��Ȩ��ѯģʽ

```csharp
// ��ʽ��name=weight*url
var client = new ApiHttpClient("master=3*http://api1.example.com,slave=7*http://api2.example.com")
{
    LoadBalanceMode = LoadBalanceMode.RoundRobin
};

// master Ȩ��3��slave Ȩ��7
// 10�������У�master Լ3�Σ�slave Լ7��
```

### ���ٵ���ģʽ

```csharp
var client = new ApiHttpClient("http://api1.example.com,http://api2.example.com,http://api3.example.com")
{
    LoadBalanceMode = LoadBalanceMode.Race
};

// �����������нڵ㣬����������Ӧ
// �����ڶ���Ӧʱ��Ҫ�󼫸ߵĳ���
```

## ������֤

### Token ����

```csharp
var client = new ApiHttpClient("http://api.example.com")
{
    Token = "your_access_token"
};

// ����ͷ�Զ����ӣ�Authorization: Bearer your_access_token
```

### Authentication ����

```csharp
var client = new ApiHttpClient("http://api.example.com")
{
    Authentication = new AuthenticationHeaderValue("Bearer", "your_token")
};

// ����ʹ�� Basic ��֤
client.Authentication = new AuthenticationHeaderValue("Basic", 
    Convert.ToBase64String(Encoding.UTF8.GetBytes("user:password")));
```

### ����ڵ���� Token

```csharp
// �� URL ��ָ�� Token
var client = new ApiHttpClient();
client.Add("service1", "http://api1.example.com#token=token_for_api1");
client.Add("service2", "http://api2.example.com#token=token_for_api2");
```

> **���ȼ�**��`Token` ���������� `Authentication` ���ԡ�

## ��Ӧ����

### ��׼��Ӧ��ʽ

Ĭ��֧��������Ӧ��ʽ��

```json
{
    "code": 0,
    "message": "success",
    "data": { ... }
}
```

### �Զ����ֶ�����

```csharp
var client = new ApiHttpClient("http://api.example.com")
{
    CodeName = "status",    // ״̬���ֶ�����Ĭ���Զ�ʶ�� code/errcode/status
    DataName = "result"     // �����ֶ�����Ĭ�� data
};

// ������Ӧ��ʽ��{"status": 0, "result": {...}}
```

### ֧�ֵ�״̬���ֶ�

- `code`
- `errcode`
- `status`

### ֧�ֵ���Ϣ�ֶ�

- `message`
- `msg`
- `errmsg`
- `error`

## Http ����

```csharp
var client = new ApiHttpClient("http://api.example.com");

// GET - ����ƴ�ӵ� URL
var result = await client.GetAsync<T>("api/users", new { page = 1, size = 10 });

// POST - ���� JSON ���л��� Body
var result = await client.PostAsync<T>("api/users", new { name = "test" });

// PUT
var result = await client.PutAsync<T>("api/users/1", new { name = "updated" });

// PATCH
var result = await client.PatchAsync<T>("api/users/1", new { name = "patched" });

// DELETE
var result = await client.DeleteAsync<T>("api/users/1");

// ͨ�õ���
var result = await client.InvokeAsync<T>(HttpMethod.Post, "api/action", args);
```

## �߼�����

### ��ʱ����

```csharp
var client = new ApiHttpClient("http://api.example.com")
{
    Timeout = 30_000  // 30�룬Ĭ��15��
};
```

### ��������

```csharp
var client = new ApiHttpClient("http://api.example.com")
{
    UseProxy = true  // ʹ��ϵͳ������Ĭ��false
};
```

### SSL֤����֤

```csharp
var client = new ApiHttpClient("https://api.example.com")
{
    CertificateValidation = false  // ����֤֤�飬Ĭ��false
};
```

### �Զ��� UserAgent

```csharp
var client = new ApiHttpClient("http://api.example.com")
{
    DefaultUserAgent = "MyApp/1.0"
};
```

### �Զ��� Json ���л�

```csharp
var client = new ApiHttpClient("http://api.example.com")
{
    JsonHost = new FastJson()  // �Զ��� Json ���л���
};
```

## �¼��������

### OnRequest �¼�

```csharp
var client = new ApiHttpClient("http://api.example.com");

client.OnRequest += (sender, e) =>
{
    // �����Զ�������ͷ
    e.Request.Headers.Add("X-Request-Id", Guid.NewGuid().ToString());
    e.Request.Headers.Add("X-Timestamp", DateTime.Now.Ticks.ToString());
};
```

### OnCreateClient �¼�

```csharp
client.OnCreateClient += (sender, e) =>
{
    // ���� HttpClient
    e.Client.DefaultRequestHeaders.Add("X-App-Version", "1.0.0");
};
```

### Http ������

```csharp
// ʹ�����õ����ƹ�����
var filter = new TokenHttpFilter
{
    UserName = "app_id",
    Password = "app_secret"
};

var client = new ApiHttpClient("http://api.example.com")
{
    Filter = filter
};

// ���������Զ��������ƵĻ�ȡ��ˢ��
```

### �Զ��������

```csharp
public class MyHttpFilter : IHttpFilter
{
    public Task OnRequest(HttpClient client, HttpRequestMessage request, Object? state, CancellationToken cancellationToken)
    {
        // ����ǰ����
        request.Headers.Add("X-Custom", "value");
        return Task.CompletedTask;
    }

    public Task OnResponse(HttpClient client, HttpResponseMessage response, Object? state, CancellationToken cancellationToken)
    {
        // ��Ӧ����
        return Task.CompletedTask;
    }

    public Task OnError(HttpClient client, Exception ex, Object? state, CancellationToken cancellationToken)
    {
        // ������
        return Task.CompletedTask;
    }
}
```

## ����״̬���

### �鿴��ǰ����

```csharp
var client = new ApiHttpClient("http://api1.example.com,http://api2.example.com");

// ��ǰ����ʹ�õķ���
var current = client.Current;
Console.WriteLine($"��ǰ����{current?.Name} - {current?.Address}");

// ��ǰ��������
Console.WriteLine($"����Դ��{client.Source}");
```

### �鿴�����б�״̬

```csharp
foreach (var svc in client.Services)
{
    Console.WriteLine($"����{svc.Name}");
    Console.WriteLine($"  ��ַ��{svc.Address}");
    Console.WriteLine($"  Ȩ�أ�{svc.Weight}");
    Console.WriteLine($"  ���ô�����{svc.Times}");
    Console.WriteLine($"  ���������{svc.Errors}");
    Console.WriteLine($"  �Ƿ���ã�{svc.IsAvailable()}");
    Console.WriteLine($"  �´ο���ʱ�䣺{svc.NextTime}");
}
```

## ��·׷��

```csharp
var client = new ApiHttpClient("http://api.example.com")
{
    Tracer = DefaultTracer.Instance,  // ������·׷����
    SlowTrace = 5_000  // ����5���¼��������־
};
```

## ����ע��

### ASP.NET Core ����

```csharp
// ע�����
services.AddSingleton<IApiClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var client = new ApiHttpClient(config["ApiServer:Urls"])
    {
        Timeout = config.GetValue<Int32>("ApiServer:Timeout"),
        ServiceProvider = sp
    };
    return client;
});

// ʹ����������
services.AddSingleton<IApiClient>(sp =>
{
    return new ApiHttpClient(sp, "ApiServerConfig");  // ���������Ķ�ȡ
});
```

### IConfigMapping �ӿ�

```csharp
// ApiHttpClient ʵ���� IConfigMapping �ӿ�
// ����ͨ���������Ķ�̬���·����ַ

var configProvider = services.GetRequiredService<IConfigProvider>();
configProvider.Bind(client, true, "ApiServer");  // �����ý�
```

## �ļ�����

```csharp
var client = new ApiHttpClient("http://download.example.com");

// �����ļ���У���ϣ
await client.DownloadFileAsync(
    requestUri: "files/package.zip",
    fileName: "D:/downloads/package.zip",
    expectedHash: "sha256:abc123...",  // ��ѡ��֧�� md5/sha1/sha256/sha512
    cancellationToken: default
);
```

## �쳣����

### ApiException

```csharp
try
{
    var result = await client.GetAsync<Object>("api/data");
}
catch (ApiException ex)
{
    // ҵ���쳣������˷��صĴ����룩
    Console.WriteLine($"�����룺{ex.Code}");
    Console.WriteLine($"������Ϣ��{ex.Message}");
}
catch (HttpRequestException ex)
{
    // �����쳣
    Console.WriteLine($"�������{ex.Message}");
}
```

## ���ʵ��

### 1. ���ÿͻ���ʵ��

```csharp
// ? �Ƽ�����Ϊ����ʹ��
public class MyService
{
    private static readonly ApiHttpClient _client = new("http://api.example.com");
    
    public Task<T> GetDataAsync<T>() => _client.GetAsync<T>("api/data");
}

// ? ���⣺ÿ�����󴴽���ʵ��
public async Task<T> GetDataAsync<T>()
{
    using var client = new ApiHttpClient("http://api.example.com");  // ���Ƽ�
    return await client.GetAsync<T>("api/data");
}
```

### 2. �������ó�ʱ

```csharp
var client = new ApiHttpClient("http://api.example.com")
{
    Timeout = 10_000,  // ���ݽӿ��������ú�����ʱ
    SlowTrace = 3_000  // ��������ֵ
};
```

### 3. ���ù���ת��

```csharp
var client = new ApiHttpClient("http://primary.example.com,http://backup.example.com")
{
    ShieldingTime = 30,  // ���Ͻڵ�����30��
    LoadBalanceMode = LoadBalanceMode.Failover
};
```

### 4. ʹ����·׷��

```csharp
var client = new ApiHttpClient("http://api.example.com")
{
    Tracer = DefaultTracer.Instance,
    Log = XTrace.Log  // ������־
};
```

## ����ʾ��

```csharp
using NewLife.Log;
using NewLife.Remoting;

// �����ͻ���
var client = new ApiHttpClient("master=3*http://api1.example.com,slave=7*http://api2.example.com")
{
    Token = "your_access_token",
    Timeout = 15_000,
    ShieldingTime = 60,
    LoadBalanceMode = LoadBalanceMode.RoundRobin,
    CodeName = "code",
    DataName = "data",
    Tracer = DefaultTracer.Instance,
    Log = XTrace.Log
};

// ������������
client.OnRequest += (sender, e) =>
{
    e.Request.Headers.Add("X-Request-Id", Guid.NewGuid().ToString());
};

try
{
    // ��������
    var users = await client.GetAsync<List<UserInfo>>("api/users", new { page = 1, size = 10 });
    
    foreach (var user in users)
    {
        Console.WriteLine($"�û���{user.Name}");
    }
    
    // �鿴��ǰʹ�õķ���
    Console.WriteLine($"�������{client.Source} - {client.Current?.Address}");
}
catch (ApiException ex)
{
    Console.WriteLine($"ҵ����� [{ex.Code}]��{ex.Message}");
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"�������{ex.Message}");
}
```

## �������

| ���� | ˵�� |
|------|------|
| `ApiHttpClient` | Http Ӧ�ýӿڿͻ��� |
| `ServiceEndpoint` | ����˵㣬������ַ��Ȩ�ء�״̬����Ϣ |
| `ILoadBalancer` | ���ؾ������ӿ� |
| `FailoverLoadBalancer` | ����ת�Ƹ��ؾ����� |
| `WeightedRoundRobinLoadBalancer` | ��Ȩ��ѯ���ؾ����� |
| `RaceLoadBalancer` | ���ٸ��ؾ����� |
| `IHttpFilter` | Http �������ӿ� |
| `TokenHttpFilter` | ���ƹ����� |
| `ApiException` | Api ҵ���쳣 |

## �汾��ʷ

- **v11.0+**�����븺�ؾ���ģʽö�٣�֧�־��ٵ���
- **v10.0+**��֧���Զ��� CodeName/DataName
- **v9.0+**��֧����·׷��
