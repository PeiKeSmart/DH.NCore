# ��ȫ��չ SecurityHelper

## ����

`SecurityHelper` �� DH.NCore �еİ�ȫ�㷨�����࣬�ṩ���õĹ�ϣ�㷨���ԳƼ��ܡ��ǶԳƼ��ܵȹ��ܵ���չ������֧�� MD5��SHA ϵ�С�CRC��AES��DES��RSA �����������㷨��

**�����ռ�**��`NewLife`  
**文档地址**：历史文档已归档，当前请以仓库内 Doc 为准

## ��������

- **��ϣ�㷨**��MD5��SHA1��SHA256��SHA384��SHA512��CRC16��CRC32��Murmur128
- **�ԳƼ���**��AES��DES��3DES��RC4��SM4
- **�ǶԳƼ���**��RSA��DSA
- **������**��ʹ���߳̾�̬���������㷨ʵ���������ظ�����
- **������**�������㷨������չ������ʽ�ṩ

## ���ٿ�ʼ

```csharp
using NewLife;

// MD5 ��ϣ
var hash = "password".MD5();           // 32λʮ�������ַ���
var hash16 = "password".MD5_16();      // 16λʮ�������ַ���

// SHA256 ��ϣ
var sha = data.SHA256();               // �����ֽ�����
var shaHex = data.SHA256().ToHex();    // תΪʮ�������ַ���

// AES ����
var encrypted = data.Encrypt(Aes.Create(), key);
var decrypted = encrypted.Decrypt(Aes.Create(), key);

// CRC У��
var crc32 = data.Crc();
var crc16 = data.Crc16();
```

## API �ο�

### ��ϣ�㷨

#### MD5

```csharp
public static Byte[] MD5(this Byte[] data)
public static String MD5(this String data, Encoding? encoding = null)
public static String MD5_16(this String data, Encoding? encoding = null)
public static Byte[] MD5(this FileInfo file)
```

���� MD5 ɢ��ֵ��

**ʾ��**��
```csharp
// �ַ��� MD5��32λ��
"password".MD5()                 // "5F4DCC3B5AA765D61D8327DEB882CF99"

// �ַ��� MD5��16λ��ȡ�м�8�ֽڣ�
"password".MD5_16()              // "5AA765D61D8327DE"

// �ֽ����� MD5
var data = Encoding.UTF8.GetBytes("hello");
var hash = data.MD5();           // ���� 16 �ֽ�����

// �ļ� MD5
var fileHash = "large-file.zip".AsFile().MD5().ToHex();
```

#### SHA ϵ��

```csharp
public static Byte[] SHA1(this Byte[] data, Byte[]? key)
public static Byte[] SHA256(this Byte[] data, Byte[]? key = null)
public static Byte[] SHA384(this Byte[] data, Byte[]? key)
public static Byte[] SHA512(this Byte[] data, Byte[]? key)
```

���� SHA ϵ��ɢ��ֵ����ѡ HMAC ��Կ��

**ʾ��**��
```csharp
var data = Encoding.UTF8.GetBytes("hello");

// ��ͨ��ϣ
var sha256 = data.SHA256();              // 32 �ֽ�
var sha512 = data.SHA512(null);          // 64 �ֽ�

// HMAC ��ϣ������Կ��
var key = Encoding.UTF8.GetBytes("secret");
var hmac256 = data.SHA256(key);
var hmac512 = data.SHA512(key);
```

#### CRC У��

```csharp
public static UInt32 Crc(this Byte[] data)
public static UInt16 Crc16(this Byte[] data)
```

���� CRC У��ֵ��

**ʾ��**��
```csharp
var data = new Byte[] { 1, 2, 3, 4, 5 };

var crc32 = data.Crc();          // UInt32 У��ֵ
var crc16 = data.Crc16();        // UInt16 У��ֵ
```

#### Murmur128

```csharp
public static Byte[] Murmur128(this Byte[] data, UInt32 seed = 0)
```

���� Murmur128 �Ǽ��ܹ�ϣ�������ڹ�ϣ���ȳ������ٶȱ� MD5 ��ܶࡣ

**ʾ��**��
```csharp
var hash = data.Murmur128();                  // Ĭ������
var hashWithSeed = data.Murmur128(12345);     // ָ������
```

### �ԳƼ���

#### Encrypt / Decrypt

```csharp
public static Byte[] Encrypt(this SymmetricAlgorithm sa, Byte[] data, Byte[]? pass = null, CipherMode mode = CipherMode.CBC, PaddingMode padding = PaddingMode.PKCS7)
public static Byte[] Decrypt(this SymmetricAlgorithm sa, Byte[] data, Byte[]? pass = null, CipherMode mode = CipherMode.CBC, PaddingMode padding = PaddingMode.PKCS7)
```

�ԳƼ���/�������ݡ�

**����˵��**��
- `pass`�����루���Զ���䵽���ʵ���Կ���ȣ�
- `mode`������ģʽ��CBC/ECB �ȣ���.NET Ĭ�� CBC��Java Ĭ�� ECB
- `padding`�����ģʽ��Ĭ�� PKCS7����ͬ Java �� PKCS5��

**ʾ��**��
```csharp
var data = Encoding.UTF8.GetBytes("Hello World!");
var key = Encoding.UTF8.GetBytes("my-secret-key-16");

// AES ���ܣ�CBC ģʽ��
var encrypted = Aes.Create().Encrypt(data, key);

// AES ����
var decrypted = Aes.Create().Decrypt(encrypted, key);

// ECB ģʽ���� Java ���ݣ�
var encryptedEcb = Aes.Create().Encrypt(data, key, CipherMode.ECB);
var decryptedEcb = Aes.Create().Decrypt(encryptedEcb, key, CipherMode.ECB);

// DES ����
var desKey = Encoding.UTF8.GetBytes("12345678");
var desEncrypted = DES.Create().Encrypt(data, desKey);

// 3DES ����
var tripleDesKey = Encoding.UTF8.GetBytes("123456789012345678901234");
var tripleDesEncrypted = TripleDES.Create().Encrypt(data, tripleDesKey);
```

#### ��ʽ����

```csharp
public static SymmetricAlgorithm Encrypt(this SymmetricAlgorithm sa, Stream instream, Stream outstream)
public static SymmetricAlgorithm Decrypt(this SymmetricAlgorithm sa, Stream instream, Stream outstream)
```

�����������м���/���ܣ��ʺϴ������ļ���

**ʾ��**��
```csharp
using var input = File.OpenRead("large-file.bin");
using var output = File.Create("large-file.enc");

var aes = Aes.Create();
aes.Key = key;
aes.IV = iv;
aes.Encrypt(input, output);
```

#### Transform

```csharp
public static Byte[] Transform(this ICryptoTransform transform, Byte[] data)
```

ʹ�� `ICryptoTransform` ֱ��ת�����ݡ�

**ʾ��**��
```csharp
var aes = Aes.Create();
aes.Key = key;
aes.IV = iv;

using var encryptor = aes.CreateEncryptor();
var encrypted = encryptor.Transform(data);

using var decryptor = aes.CreateDecryptor();
var decrypted = decryptor.Transform(encrypted);
```

#### RC4

```csharp
public static Byte[] RC4(this Byte[] data, Byte[] pass)
```

RC4 ��������ܡ�RC4 ���ܺͽ���ʹ����ͬ�ķ�����

**ʾ��**��
```csharp
var data = Encoding.UTF8.GetBytes("Hello");
var key = Encoding.UTF8.GetBytes("secret");

// ����
var encrypted = data.RC4(key);

// ���ܣ�ͬ���ķ�����
var decrypted = encrypted.RC4(key);
```

## ������ȫ��

### RSAHelper

RSA �ǶԳƼ��ܸ����ࡣ

```csharp
using NewLife.Security;

// ������Կ��
var (publicKey, privateKey) = RSAHelper.GenerateKey(2048);

// ����
var encrypted = RSAHelper.Encrypt(data, publicKey);

// ����
var decrypted = RSAHelper.Decrypt(encrypted, privateKey);

// ǩ��
var signature = RSAHelper.Sign(data, privateKey, "SHA256");

// ��ǩ
var isValid = RSAHelper.Verify(data, signature, publicKey, "SHA256");
```

### DSAHelper

DSA ����ǩ�������ࡣ

```csharp
using NewLife.Security;

// ǩ��
var signature = DSAHelper.Sign(data, privateKey);

// ��ǩ
var isValid = DSAHelper.Verify(data, signature, publicKey);
```

### Rand

�������������

```csharp
using NewLife.Security;

// ��������ֽ�
var bytes = Rand.NextBytes(16);

// �����������
var num = Rand.Next(1, 100);

// ��������ַ���
var str = Rand.NextString(16);           // �������ֺ���ĸ
var strWithSpecial = Rand.NextString(16, true);  // ���������ַ�
```

## ʹ�ó���

### 1. �����ϣ�洢

```csharp
public class PasswordHelper
{
    public String HashPassword(String password, String salt)
    {
        // ʹ�� SHA256 + ��ֵ
        var data = Encoding.UTF8.GetBytes(password + salt);
        return data.SHA256().ToHex();
    }
    
    public Boolean VerifyPassword(String password, String salt, String hash)
    {
        return HashPassword(password, salt).EqualIgnoreCase(hash);
    }
}
```

### 2. API ǩ����֤

```csharp
public class ApiSignature
{
    public String Sign(String data, String secret)
    {
        var key = Encoding.UTF8.GetBytes(secret);
        var content = Encoding.UTF8.GetBytes(data);
        return content.SHA256(key).ToHex();
    }
    
    public Boolean Verify(String data, String signature, String secret)
    {
        return Sign(data, secret).EqualIgnoreCase(signature);
    }
}
```

### 3. ���ݼ��ܴ���

```csharp
public class SecureTransport
{
    private readonly Byte[] _key;
    
    public SecureTransport(String password)
    {
        // ʹ������������Կ
        _key = password.MD5().ToHex().GetBytes()[..16];
    }
    
    public Byte[] Encrypt(Byte[] data)
    {
        return Aes.Create().Encrypt(data, _key);
    }
    
    public Byte[] Decrypt(Byte[] data)
    {
        return Aes.Create().Decrypt(data, _key);
    }
}
```

### 4. �ļ�������У��

```csharp
public class FileVerifier
{
    public String ComputeHash(String filePath)
    {
        return filePath.AsFile().MD5().ToHex();
    }
    
    public Boolean Verify(String filePath, String expectedHash)
    {
        var actualHash = ComputeHash(filePath);
        return actualHash.EqualIgnoreCase(expectedHash);
    }
}
```

## ���ʵ��

### 1. ѡ����ʵ��㷨

```csharp
// �����ϣ��ʹ�� SHA256 ���ǿ���㷨
var passwordHash = (password + salt).GetBytes().SHA256().ToHex();

// ���������ԣ�MD5 �㹻����
var checksum = data.MD5().ToHex();

// �����ܹ�ϣ����ʹ�� Murmur128
var hash = data.Murmur128();
```

### 2. ע�����ģʽ������

```csharp
// �� Java ϵͳ����ʱʹ�� ECB ģʽ
var encrypted = Aes.Create().Encrypt(data, key, CipherMode.ECB);

// ��ȫ��Ҫ���ʱʹ�� CBC ģʽ��Ĭ�ϣ�
var encrypted = Aes.Create().Encrypt(data, key, CipherMode.CBC);
```

### 3. ��Կ����

```csharp
// ��ҪӲ������Կ
var key = Environment.GetEnvironmentVariable("ENCRYPTION_KEY")?.ToHex();

// ʹ�ð�ȫ�������������Կ
var randomKey = Rand.NextBytes(32);
```

## �㷨�Ա�

| �㷨 | ������� | �ٶ� | ��ȫ�� | ��; |
|------|---------|------|--------|------|
| MD5 | 16�ֽ� | �ܿ� | �� | У��͡��ǰ�ȫ��ϣ |
| SHA1 | 20�ֽ� | �� | �� | ���ݾ�ϵͳ |
| SHA256 | 32�ֽ� | �� | �� | ͨ�ð�ȫ��ϣ |
| SHA512 | 64�ֽ� | ���� | �ܸ� | �߰�ȫҪ�� |
| CRC32 | 4�ֽ� | ���� | �� | ����У�� |
| Murmur128 | 16�ֽ� | ���� | �� | ��ϣ�� |

## �������

- [����ת�� Utility](utility-����ת��Utility.md)
- [������չ IOHelper](io_helper-������չIOHelper.md)
- [Webͨ������ JwtBuilder](jwt-Webͨ������JwtBuilder.md)
- [�ֲ�ʽ����ǩ������ TokenProvider](token_provider-�ֲ�ʽ����ǩ������TokenProvider.md)
