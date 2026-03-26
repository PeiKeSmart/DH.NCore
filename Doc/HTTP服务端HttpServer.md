# HttpServer ʹ���ֲ�

���ĵ�����Դ�� `DH.NCore/Http/HttpServer.cs`������˵�� `HttpServer`�������� HTTP ����������ְ��·��ע�᷽ʽ��ƥ�������ʹ��ע�����

> �ؼ��ʣ�·��ӳ�䡢ͨ��� `*`��ί�д�������������ӳ�䡢��̬�ļ���ƥ�仺�桢�̰߳�ȫ��

---

## 1. ����

`HttpServer` �̳��� `NetServer` ��ʵ�� `IHttpHost`�������� TCP ����֮���ṩ HTTP Э�鴦��������

��Ҫְ��

1. ����·��ӳ�� `Routes`�������յ�����ʱ����·��ƥ�� `IHttpHandler`��
2. Ϊÿ������Ự������Ӧ�� `HttpSession` Э�鴦������`CreateHandler`����
3. �ṩ���� `Map` ���أ�ί��/������/��̬�ļ����Լ�ע�ᡣ

---

## 2. Ĭ����Ϊ��ؼ�����

### 2.1 ��������

���캯���У�`HttpServer` ��Ĭ������Ϊ��

- `Name = "Http"`
- `Port = 80`
- `ProtocolType = NetType.Http`
- `ServerName = "NewLife-HttpServer/{Major}.{Minor}"`���ӳ��򼯰汾���ɣ�Ĭ����Ӧͷ��ʶ���ƣ���ʷ�������ֶΣ�

### 2.2 `ServerName`

- ���ͣ�`String`
- ���壺���� HTTP ��Ӧͷ�е� `Server` ���ƣ�����д����Э��ջ����������ɣ���

### 2.3 `Routes`

- ���ͣ�`IDictionary<String, IHttpHandler>`
- Key��·�������ִ�Сд���򣺲����֣�`StringComparer.OrdinalIgnoreCase`��
- Value����������`IHttpHandler`��

˵����

- ·�� Key ����ע��ʱͳһȷ���� `/` ��ͷ��
- ��ע��Ḳ����ע�ᣨ`Routes[path] = handler`����

---

## 3. �Ự��Э�鴦��

### 3.1 `CreateHandler(INetSession session)`

`HttpServer` ��Ϊÿһ���ײ�����Ự����һ���µ� `HttpSession`��

- ���أ�`new HttpSession()`

����ζ�ţ�

- HTTP ����������/��Ӧ���������߼���Ҫ�� `HttpSession` �е���
- `HttpServer` ���۽��ڡ�·�ɱ�ά�����͡�ƥ�䴦��������

---

## 4. ·��ע�� API

`HttpServer` �ṩ����·��ע�᷽ʽ������ͳһ��˽�з��� `SetRoute(String path, IHttpHandler handler)`��

### 4.1 ӳ�䴦����ʵ��

```csharp
var server = new HttpServer();
server.Map("/api/test", new MyHandler());
```

- `Map(String path, IHttpHandler handler)`

### 4.2 ӳ��ί�У�Delegate��

�����ڿ���ע�������ӿڡ�

- `Map(String path, HttpProcessDelegate handler)`
- `Map<TResult>(String path, Func<TResult> handler)`
- `Map<TModel, TResult>(String path, Func<TModel, TResult> handler)`
- `Map<T1, T2, TResult>(String path, Func<T1, T2, TResult> handler)`
- `Map<T1, T2, T3, TResult>(String path, Func<T1, T2, T3, TResult> handler)`
- `Map<T1, T2, T3, T4, TResult>(String path, Func<T1, T2, T3, T4, TResult> handler)`

˵����

- ��Щ���ػᴴ�� `DelegateHandler` ����ί�и�ֵ�� `Callback`��

ʾ����

```csharp
server.Map("/health", () => "OK");
```

### 4.3 ӳ�������

```csharp
server.MapController<MyController>();
```

- `MapController<TController>(String? path = null)`
- `MapController(Type controllerType, String? path = null)`

����

- `path` Ϊ��ʱ��Ĭ��Ϊ `/{ControllerName}`������ ControllerName ���� `controllerType.Name.TrimEnd("Controller")`��
- ������·�����ջᱻ�淶��Ϊ��`/{xxx}/*`��
- ע��Ĵ���������Ϊ `ControllerHandler`���� `ControllerType` ָ��Ŀ����������͡�

ʾ����

```csharp
server.MapController<MyController>("/api");
// ʵ��ע��·��Ϊ /api/*
```

### 4.4 ӳ�侲̬�ļ�Ŀ¼

```csharp
server.MapStaticFiles("/js", "./wwwroot/js");
```

- `MapStaticFiles(String path, String contentPath)`

����

- `path` ��ȷ���� `/` ��ͷ��
- ʵ������ƥ���·�� Key Ϊ `path.EnsureEnd("/").EnsureEnd("*")`������ `/js/*`��
- `StaticFilesHandler.Path` Ϊ `path.EnsureEnd("/")`������ `/js/`����
- `StaticFilesHandler.ContentPath` Ϊ����� `contentPath`��

---

## 5. ·�����ù淶����`SetRoute`��

����·��ע������ͳһ����

- ����У�飺
  - `path` ����Ϊ��
  - `handler` ����Ϊ��
- ·���淶����
  - `path = path.EnsureStart("/")`
- �������壺
  - `Routes[path] = handler`

ע�⣺

- `SetRoute` �����Զ�����β�� `/` �� `*`������ `MapController` / `MapStaticFiles` ����

---

## 6. ·��ƥ�����`MatchHandler`��

`MatchHandler(String path, HttpRequest? request)` ���ڸ��ݡ��ѹ淶���������·����������ѯ�ַ�������ƥ�䴦������

ƥ��˳��

1. **��ȷƥ��**��`Routes.TryGetValue(path, out handler)`
2. **��������**��
   - `_pathCache.TryGetValue(path, out p)`
   - Ȼ�� `Routes.TryGetValue(p, out handler)`
3. **ͨ���ƥ��**��ö�� `Routes`���԰��� `*` �� key ִ�У�
   - `key.IsMatch(path)`

### 6.1 ͨ���Լ��

- ����·�� key ���� `*` ʱ���Ž���ģ��ƥ�䡣
- ƥ���߼����� `IsMatch` ��չ���������Ի������ַ���ƥ����������

### 6.2 ƥ�仺�� `_pathCache`

- ���ͣ�`IDictionary<String, String>`
- Key������·�� `path`
- Value�����е�·�� key������ `/api/*`��

������ԣ�

- ���� `StaticFilesHandler`������� `path -> routeKey`��
- �Ǿ�̬�ļ������� `path.Split('/')` ���� `<= 3` �Ż��档

Ŀ�ģ�

- ���⶯̬ URL���������� id ��·������ɻ����������ͣ�
- �Գ�����·������ģ��ƥ�䡣

---

## 7. �̰߳�ȫ�벢��ע������

��ǰʵ�ֵĲ������壺

- `Routes` Ĭ���� `Dictionary`�����ǲ���������
- ���ͳ����������׶μ���ע��·�ɣ�������ֻ�����ʣ�
- �������ڶ�̬��ɾ·�ɣ���Ҫ���÷����м������л����ʡ�

���յ㣺

- ���������޸� `Routes` ��ͬʱ���� `MatchHandler`�����ܴ��� `Dictionary` ö���쳣�������һ�½����
- `_pathCache` ͬ��Ϊ `Dictionary`��������дҲ����֤��ȫ��

���飺

- ������ɺ�Ҫ�ٱ��·�ɣ�
- �������ⲿ������ȷ�� `Map/SetRoute` �� `MatchHandler` ������ִ�С�

---

## 8. ��Сʾ��

> ˵����ʾ��ֻ��ʾ `HttpServer` ��·��ע������ϡ�ʵ�������������Ự�շ��������� `NetServer` �ṩ��������Ŀ������ʾ���� `NetServer` �ĵ�Ϊ׼��

```csharp
using NewLife.Http;

var server = new HttpServer
{
    Port = 8080,
    ServerName = "MyServer/1.0",
};

server.Map("/health", () => "OK");
server.MapStaticFiles("/static", "./wwwroot");
server.MapController<MyController>("/api");

server.Start();
```

---

## 9. ��������

### 9.1 Ϊʲô `MapController` ���Զ����� `/*`��

������ͨ����Ҫƥ���䡰��·���������� `/api/user/list`��`/api/user/detail/123` �ȡ�ͨ�� `/*` ��ͬһ���������������ӹܸ�ǰ׺�µ���������

### 9.2 Ϊʲô����·���������棿

�Զ�ζ�̬ URL ȫ��������ܵ��� `_pathCache` �����������������ͣ�����ǰ���Խ������·����̬�ļ����У���ʵ�ּ�����ռ�֮������С�

---

## 10. ���Դ��

- `DH.NCore/Http/HttpServer.cs`
- `DH.NCore/Http/HttpSession.cs`
- `DH.NCore/Http/Handlers/DelegateHandler.cs`������Ŀʵ��·��Ϊ׼��
- `DH.NCore/Http/Handlers/ControllerHandler.cs`
- `DH.NCore/Http/Handlers/StaticFilesHandler.cs`
