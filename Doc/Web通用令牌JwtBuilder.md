# Webͨ������ JwtBuilder

## ����

`JwtBuilder` �� DH.NCore �е� JSON Web Token (JWT) ���ɺ���֤�����ࡣJWT ��һ�ֽ��ա��԰��������Ƹ�ʽ���㷺���� Web API ��֤����Ȩ��JwtBuilder ֧�� HS256/HS384/HS512 �� RS256/RS384/RS512 �������㷨��

**�����ռ�**��`NewLife.Web`  
**文档地址**：历史文档已归档，当前请以仓库内 Doc 为准

## ��������

- **���㷨֧��**��HS256��HS384��HS512��RS256��RS384��RS512
- **��׼����**��֧�� iss��sub��aud��exp��nbf��iat��jti �ȱ�׼����
- **�Զ�������**��֧����������Я�������Զ�������
- **ʱ����֤**���Զ���֤����ʱ�����Чʱ��
- **����չ**��֧��ע���Զ���ǩ���㷨

## ���ٿ�ʼ

### ��������

```csharp
using NewLife.Web;

var builder = new JwtBuilder
{
    Secret = "your-secret-key-at-least-32-characters",
    Expire = DateTime.Now.AddHours(2),  // 2Сʱ�����
    Subject = "user123",                // �û���ʶ
    Issuer = "MyApp"                    // �䷢��
};

// �����Զ�������
builder["role"] = "admin";
builder["name"] = "����";

// ��������
var token = builder.Encode(new { });
Console.WriteLine(token);
```

### ��֤����

```csharp
var builder = new JwtBuilder
{
    Secret = "your-secret-key-at-least-32-characters"
};

if (builder.TryDecode(token, out var message))
{
    Console.WriteLine($"�û�: {builder.Subject}");
    Console.WriteLine($"��ɫ: {builder["role"]}");
    Console.WriteLine($"����ʱ��: {builder.Expire}");
}
else
{
    Console.WriteLine($"��֤ʧ��: {message}");
}
```

## API �ο�

### ����

#### ��׼����

```csharp
/// <summary>�䷢�� (iss)</summary>
public String? Issuer { get; set; }

/// <summary>���������� (sub)���ɴ���û�ID</summary>
public String? Subject { get; set; }

/// <summary>���� (aud)</summary>
public String? Audience { get; set; }

/// <summary>����ʱ�� (exp)��Ĭ��2Сʱ</summary>
public DateTime Expire { get; set; }

/// <summary>��Чʱ�� (nbf)���ڴ�֮ǰ��Ч</summary>
public DateTime NotBefore { get; set; }

/// <summary>�䷢ʱ�� (iat)</summary>
public DateTime IssuedAt { get; set; }

/// <summary>���Ʊ�ʶ (jti)</summary>
public String? Id { get; set; }
```

#### ��������

```csharp
/// <summary>�㷨��Ĭ��HS256</summary>
public String Algorithm { get; set; }

/// <summary>�������ͣ�Ĭ��JWT</summary>
public String? Type { get; set; }

/// <summary>��Կ</summary>
public String? Secret { get; set; }

/// <summary>�Զ���������</summary>
public IDictionary<String, Object?> Items { get; }
```

#### ������

```csharp
// ��ȡ�������Զ�������
public Object? this[String key] { get; set; }
```

### Encode - ��������

```csharp
public String Encode(Object payload)
```

�����ݱ���Ϊ JWT �����ַ�����

**����**��
- `payload`��Ҫ��������ݶ���

**����ֵ**��JWT �����ַ���

**ʾ��**��
```csharp
var builder = new JwtBuilder
{
    Secret = "my-secret-key-32-characters-long",
    Expire = DateTime.Now.AddDays(7),
    Subject = "user_001"
};

// ��ʽ1��ʹ�����Ժ�������
builder["permissions"] = new[] { "read", "write" };
var token1 = builder.Encode(new { });

// ��ʽ2��ֱ�Ӵ������
var token2 = builder.Encode(new
{
    userId = 123,
    userName = "test",
    permissions = new[] { "read", "write" }
});
```

### TryDecode - ��֤����������

```csharp
public Boolean TryDecode(String token, out String? message)
```

��֤ JWT ���Ʋ��������ݡ�

**����**��
- `token`��JWT �����ַ���
- `message`����֤ʧ��ʱ�Ĵ�����Ϣ

**����ֵ**����֤�Ƿ�ɹ�

**��֤����**��
1. JWT ��ʽ�Ƿ���ȷ������ʽ��
2. ǩ���Ƿ���Ч
3. �Ƿ�����Ч����
4. �Ƿ�����Ч

**ʾ��**��
```csharp
var builder = new JwtBuilder
{
    Secret = "my-secret-key-32-characters-long"
};

if (builder.TryDecode(token, out var message))
{
    // ��֤�ɹ�����ȡ����
    var userId = builder.Subject;
    var expire = builder.Expire;
    var permissions = builder["permissions"];
}
else
{
    // ��֤ʧ��
    Console.WriteLine($"����: {message}");
    // ���ܵĴ���:
    // - "JWT��ʽ����ȷ"
    // - "�����ѹ���"
    // - "����δ��Ч"
    // - "δ������Կ"
}
```

### Parse - ����������֤

```csharp
public String[]? Parse(String token)
```

���������ƽṹ������֤ǩ����������Ҫ����֤ǰ��ȡ���ݵĳ�����

**����ֵ**������ʽ���� [header, payload, signature]����ʽ���󷵻� null

**ʾ��**��
```csharp
var builder = new JwtBuilder();
var parts = builder.Parse(token);

if (parts != null)
{
    // ���Զ�ȡ�㷨������ʱ���
    Console.WriteLine($"�㷨: {builder.Algorithm}");
    Console.WriteLine($"����: {builder.Expire}");
    
    // Ȼ��������Կ����������֤
    builder.Secret = "...";
    if (builder.TryDecode(token, out _)) { }
}
```

### RegisterAlgorithm - ע���Զ����㷨

```csharp
public static void RegisterAlgorithm(
    String algorithm, 
    JwtEncodeDelegate encode, 
    JwtDecodeDelegate? decode)
```

ע���Զ���ǩ���㷨��

**ʾ��**��
```csharp
// ע���Զ����㷨
JwtBuilder.RegisterAlgorithm(
    "ES256",
    (data, secret) => ECDsaHelper.SignSha256(data, secret),
    (data, secret, signature) => ECDsaHelper.VerifySha256(data, secret, signature)
);

// ʹ���Զ����㷨
var builder = new JwtBuilder
{
    Algorithm = "ES256",
    Secret = ecdsaPrivateKey
};
var token = builder.Encode(new { });
```

## ֧�ֵ��㷨

| �㷨 | ���� | ��ԿҪ�� | ˵�� |
|------|------|---------|------|
| HS256 | HMAC | �Գ���Կ | Ĭ���㷨���ʺϴ�������� |
| HS384 | HMAC | �Գ���Կ | �����Ĺ�ϣ |
| HS512 | HMAC | �Գ���Կ | ��Ĺ�ϣ |
| RS256 | RSA | ��˽Կ�� | �ǶԳƼ��ܣ��ʺϷֲ�ʽ |
| RS384 | RSA | ��˽Կ�� | �����Ĺ�ϣ |
| RS512 | RSA | ��˽Կ�� | ��Ĺ�ϣ |

## ʹ�ó���

### 1. API ��֤

```csharp
// ��¼�ӿ� - ��������
[HttpPost("login")]
public IActionResult Login(String username, String password)
{
    var user = ValidateUser(username, password);
    if (user == null) return Unauthorized();
    
    var builder = new JwtBuilder
    {
        Secret = Configuration["Jwt:Secret"],
        Expire = DateTime.Now.AddHours(24),
        Subject = user.Id.ToString(),
        Issuer = "MyApi"
    };
    builder["role"] = user.Role;
    builder["name"] = user.Name;
    
    var token = builder.Encode(new { });
    return Ok(new { token });
}

// ��֤�м��
public class JwtMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"]
            .ToString().TrimStart("Bearer ");
        
        if (!token.IsNullOrEmpty())
        {
            var builder = new JwtBuilder
            {
                Secret = _configuration["Jwt:Secret"]
            };
            
            if (builder.TryDecode(token, out _))
            {
                context.Items["UserId"] = builder.Subject;
                context.Items["Role"] = builder["role"];
            }
        }
        
        await _next(context);
    }
}
```

### 2. ˢ������

```csharp
public class TokenService
{
    public (String accessToken, String refreshToken) CreateTokenPair(User user)
    {
        // �������� - ������Ч
        var accessBuilder = new JwtBuilder
        {
            Secret = _secret,
            Expire = DateTime.Now.AddMinutes(15),
            Subject = user.Id.ToString()
        };
        accessBuilder["type"] = "access";
        
        // ˢ������ - ������Ч
        var refreshBuilder = new JwtBuilder
        {
            Secret = _secret,
            Expire = DateTime.Now.AddDays(7),
            Subject = user.Id.ToString()
        };
        refreshBuilder["type"] = "refresh";
        
        return (accessBuilder.Encode(new { }), refreshBuilder.Encode(new { }));
    }
    
    public String? RefreshAccessToken(String refreshToken)
    {
        var builder = new JwtBuilder { Secret = _secret };
        
        if (!builder.TryDecode(refreshToken, out _)) return null;
        if (builder["type"]?.ToString() != "refresh") return null;
        
        // �����µķ�������
        var newBuilder = new JwtBuilder
        {
            Secret = _secret,
            Expire = DateTime.Now.AddMinutes(15),
            Subject = builder.Subject
        };
        newBuilder["type"] = "access";
        
        return newBuilder.Encode(new { });
    }
}
```

### 3. RSA �ǶԳ�ǩ��

```csharp
// �����ǩ����ʹ��˽Կ��
var privateKey = File.ReadAllText("private.pem");
var builder = new JwtBuilder
{
    Algorithm = "RS256",
    Secret = privateKey,
    Expire = DateTime.Now.AddHours(1),
    Subject = "user123"
};
var token = builder.Encode(new { });

// �ͻ���/����������֤��ʹ�ù�Կ��
var publicKey = File.ReadAllText("public.pem");
var verifier = new JwtBuilder
{
    Algorithm = "RS256",
    Secret = publicKey
};
if (verifier.TryDecode(token, out var msg))
{
    Console.WriteLine($"��֤�ɹ�: {verifier.Subject}");
}
```

## ���ʵ��

### 1. ��ȫ����Կ����

```csharp
// ���Ƽ���Ӳ������Կ
var builder = new JwtBuilder { Secret = "my-secret" };

// �Ƽ��������û򻷾�������ȡ
var builder = new JwtBuilder
{
    Secret = Environment.GetEnvironmentVariable("JWT_SECRET")
        ?? Configuration["Jwt:Secret"]
};
```

### 2. �����Ĺ���ʱ��

```csharp
// �������ƣ����ڣ�15����-2Сʱ��
Expire = DateTime.Now.AddMinutes(30);

// ˢ�����ƣ����ڣ�1-7�죩
Expire = DateTime.Now.AddDays(7);

// ��ס�����ƣ����ڣ�30�죩
Expire = DateTime.Now.AddDays(30);
```

### 3. ��С����������

```csharp
// ���Ƽ�����Ŵ�������
builder["userProfile"] = new { /* ����� */ };

// �Ƽ�������ű�Ҫ��ʶ
builder.Subject = user.Id.ToString();  // ��Ҫ����ʱ�����ݿ�
builder["role"] = user.Role;           // ���õ���Ȩ��Ϣ
```

### 4. ��֤��������

```csharp
if (builder.TryDecode(token, out var message))
{
    // ������֤�䷢��
    if (builder.Issuer != "MyApp")
    {
        // �䷢�߲�ƥ��
    }
    
    // ������֤����
    if (builder.Audience != "web-client")
    {
        // ���ڲ�ƥ��
    }
}
```

## JWT ��ȫע������

1. **��Ҫ�洢��������**��JWT Ĭ�ϲ����ܣ�payload �ɱ� Base64 ����
2. **ʹ�� HTTPS**����ֹ���Ʊ��м��˽ػ�
3. **���ú�������ʱ��**���������Ʊ����õķ���
4. **ʹ���㹻������Կ**��HS256 ���� 32 �ֽ�
5. **��֤��������**������ iss��aud��exp ��

## �������

- [�ֲ�ʽ����ǩ������ TokenProvider](token_provider-�ֲ�ʽ����ǩ������TokenProvider.md)
- [��ȫ��չ SecurityHelper](security_helper-��ȫ��չSecurityHelper.md)
