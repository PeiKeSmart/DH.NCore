# ·����չ PathHelper

## ����

`PathHelper` �� DH.NCore �е�·�����������࣬�ṩ��ƽ̨���ļ�·��������Ŀ¼�������ļ�ѹ����ѹ����ϣУ��ȹ��ܡ����ܴ������·���;���·�����Զ����� Windows �� Linux ��·���ָ�����

**�����ռ�**��`System.IO`������ֱ��ʹ�ã�����������ã�  
**文档地址**：历史文档已归档，当前请以仓库内 Doc 为准

## ��������

- **��ƽ̨·������**���Զ����� Windows��`\`���� Linux��`/`��·���ָ���
- **����·������**��֧�����·��������·��������·��
- **��������֧��**��ͨ�������в����򻷾��������û���Ŀ¼
- **ѹ����ѹ֧��**��֧�� zip��tar��tar.gz��7z �ȸ�ʽ
- **�ļ���ϣУ��**��֧�� MD5��SHA1��SHA256��SHA512��CRC32

## ���ٿ�ʼ

```csharp
using System.IO;

// ��ȡ����·��
var path = "config/app.json".GetFullPath();

// ȷ��Ŀ¼����
"logs/2024/01/".EnsureDirectory(false);

// �ϲ�·��
var file = "data".CombinePath("users", "config.json");

// ѹ��Ŀ¼
"output".AsDirectory().Compress("backup.zip");

// ��֤�ļ���ϣ
var valid = "app.exe".AsFile().VerifyHash("md5$1234567890abcdef");
```

## API �ο�

### ·������

#### BasePath

```csharp
public static String? BasePath { get; set; }
```

����Ŀ¼������ `GetBasePath` ��������Ҫ���� X ����ڲ���Ŀ¼��ר��Ϊ������������ơ�

**���÷�ʽ**�������ȼ�����
1. �����в�����`-BasePath /app/data` �� `--BasePath /app/data`
2. ����������`BasePath=/app/data`
3. Ĭ��ֵ��Ӧ�ó��������Ŀ¼

#### BaseDirectory

```csharp
public static String? BaseDirectory { get; set; }
```

��׼Ŀ¼������ `GetFullPath` ������֧��ͨ�������в����ͻ����������á�

### ·��ת��

#### GetFullPath

```csharp
public static String GetFullPath(this String path)
```

��ȡ�ļ���Ŀ¼����Ӧ�ó������Ŀ¼��ȫ·����

**�ص�**��
- �Զ��������·��
- �Զ�ת��·���ָ���
- ֧������·����`\\server\share`��
- ֧�� `~` ��ͷ��·��

**ʾ��**��
```csharp
// ���·��ת����·��
"config/app.json".GetFullPath()      
// Windows: C:\MyApp\config\app.json
// Linux: /home/user/myapp/config/app.json

// ���Ǿ���·����ԭ������
"C:\\temp\\file.txt".GetFullPath()   // C:\temp\file.txt
"/var/log/app.log".GetFullPath()     // /var/log/app.log

// ����·��
"\\\\server\\share\\file.txt".GetFullPath()  // \\server\share\file.txt

// ~ ��ͷ��·��
"~/config/app.json".GetFullPath()    // ȥ�� ~ ��ƴ�ӻ���Ŀ¼
```

#### GetBasePath

```csharp
public static String GetBasePath(this String path)
```

��ȡ�ļ���Ŀ¼��ȫ·�������� X ����ڲ���Ŀ¼��

**ʾ��**��
```csharp
"logs/app.log".GetBasePath()
// ���� BasePath ������·��
```

#### GetCurrentPath

```csharp
public static String GetCurrentPath(this String path)
```

��ȡ�ļ���Ŀ¼���ڵ�ǰ����Ŀ¼��ȫ·����

**ʾ��**��
```csharp
"output/result.txt".GetCurrentPath()
// ���� Environment.CurrentDirectory ������·��
```

### Ŀ¼����

#### EnsureDirectory

```csharp
public static String EnsureDirectory(this String path, Boolean isfile = true)
```

ȷ��Ŀ¼���ڣ����������򴴽���

**����˵��**��
- `isfile`��·���Ƿ�Ϊ�ļ�·����`true` ʱȡĿ¼���֣�б�ܽ�β��·��ʼ����ΪĿ¼��

**ʾ��**��
```csharp
// ȷ���ļ�����Ŀ¼����
"logs/2024/01/app.log".EnsureDirectory(true);
// ���� logs/2024/01/ Ŀ¼

// ȷ��Ŀ¼��������
"data/cache/".EnsureDirectory(false);
// ���� data/cache/ Ŀ¼

// б�ܽ�β��·��ʼ����ΪĿ¼
"output/temp/".EnsureDirectory();  // isfile ����������
```

#### CombinePath

```csharp
public static String CombinePath(this String? path, params String[] ps)
```

�ϲ����·����

**ʾ��**��
```csharp
"data".CombinePath("users", "config.json")
// Windows: data\users\config.json
// Linux: data/users/config.json

// ֧�ֿ�·��
"".CombinePath("logs", "app.log")  // logs/app.log
```

### �ļ�����

#### AsFile

```csharp
public static FileInfo AsFile(this String file)
```

��·���ַ���ת��Ϊ `FileInfo` ����

**ʾ��**��
```csharp
var fi = "config/app.json".AsFile();
if (fi.Exists)
{
    Console.WriteLine($"�ļ���С: {fi.Length}");
}
```

#### ReadBytes

```csharp
public static Byte[] ReadBytes(this FileInfo file, Int32 offset = 0, Int32 count = -1)
```

���ļ���ȡ�ֽ����ݡ�

**ʾ��**��
```csharp
// ��ȡ�����ļ�
var data = "data.bin".AsFile().ReadBytes();

// ��ȡָ����Χ
var header = "data.bin".AsFile().ReadBytes(0, 100);  // ǰ100�ֽ�
var tail = "data.bin".AsFile().ReadBytes(1000, 50);  // ��1000��ʼ��50�ֽ�
```

#### WriteBytes

```csharp
public static FileInfo WriteBytes(this FileInfo file, Byte[] data, Int32 offset = 0)
```

���ļ�д���ֽ����ݡ�

**ʾ��**��
```csharp
var data = new Byte[] { 1, 2, 3, 4, 5 };
"output.bin".AsFile().WriteBytes(data);
```

#### CopyToIfNewer

```csharp
public static Boolean CopyToIfNewer(this FileInfo fi, String destFileName)
```

����Դ�ļ���Ŀ���ļ���ʱ�Ÿ��ơ�

**ʾ��**��
```csharp
var source = "src/app.dll".AsFile();
if (source.CopyToIfNewer("dest/app.dll"))
{
    Console.WriteLine("�ļ��Ѹ���");
}
```

### Ŀ¼����

#### AsDirectory

```csharp
public static DirectoryInfo AsDirectory(this String dir)
```

��·���ַ���ת��Ϊ `DirectoryInfo` ����

**ʾ��**��
```csharp
var di = "data/cache".AsDirectory();
if (di.Exists)
{
    Console.WriteLine($"���� {di.GetFiles().Length} ���ļ�");
}
```

#### GetAllFiles

```csharp
public static IEnumerable<FileInfo> GetAllFiles(this DirectoryInfo di, String? exts = null, Boolean allSub = false)
```

��ȡĿ¼�����з����������ļ���֧�ֶ���չ��ƥ�䡣

**ʾ��**��
```csharp
var dir = "src".AsDirectory();

// ��ȡ�����ļ�
var allFiles = dir.GetAllFiles();

// ��ȡָ����չ���ļ�
var csharpFiles = dir.GetAllFiles("*.cs");

// ����չ��ƥ�䣨�ֺš����ߡ����ŷָ���
var codeFiles = dir.GetAllFiles("*.cs;*.xaml;*.json");

// ������Ŀ¼
var allCsharp = dir.GetAllFiles("*.cs", true);
```

#### CopyTo

```csharp
public static String[] CopyTo(this DirectoryInfo di, String destDirName, String? exts = null, Boolean allSub = false, Action<String>? callback = null)
```

����Ŀ¼�е��ļ���Ŀ��Ŀ¼��

**ʾ��**��
```csharp
var copied = "src".AsDirectory().CopyTo("backup", "*.cs;*.json", true, name =>
{
    Console.WriteLine($"����: {name}");
});
Console.WriteLine($"������ {copied.Length} ���ļ�");
```

#### CopyToIfNewer

```csharp
public static String[] CopyToIfNewer(this DirectoryInfo di, String destDirName, String? exts = null, Boolean allSub = false, Action<String>? callback = null)
```

������ԴĿ¼�б�Ŀ��Ŀ¼���µ��ļ���

**ʾ��**��
```csharp
var updated = "src".AsDirectory().CopyToIfNewer("dest", "*.dll;*.exe", true);
```

### ѹ����ѹ

#### Extract���ļ���ѹ��

```csharp
public static void Extract(this FileInfo fi, String destDir, Boolean overwrite = false)
```

��ѹ�ļ���ָ��Ŀ¼��

**֧�ָ�ʽ**��zip��tar��tar.gz��tgz��7z���� Windows��

**ʾ��**��
```csharp
// ��ѹ zip �ļ�
"package.zip".AsFile().Extract("output");

// ��ѹ tar.gz �ļ�
"archive.tar.gz".AsFile().Extract("output", overwrite: true);

// Ĭ�Ͻ�ѹ��ͬ��Ŀ¼
"app.zip".AsFile().Extract("");  // ��ѹ�� app/ Ŀ¼
```

#### Compress���ļ�ѹ����

```csharp
public static void Compress(this FileInfo fi, String destFile)
```

ѹ�������ļ���

**ʾ��**��
```csharp
"large-file.log".AsFile().Compress("large-file.zip");
"data.bin".AsFile().Compress("data.tar.gz");
```

#### Compress��Ŀ¼ѹ����

```csharp
public static void Compress(this DirectoryInfo di, String? destFile = null)
public static void Compress(this DirectoryInfo di, String? destFile, Boolean includeBaseDirectory)
```

ѹ������Ŀ¼��

**ʾ��**��
```csharp
// ѹ��Ŀ¼��Ĭ�� zip ��ʽ��
"src".AsDirectory().Compress("src.zip");

// ѹ��Ϊ tar.gz
"dist".AsDirectory().Compress("dist.tar.gz");

// ������Ŀ¼����
"project".AsDirectory().Compress("project.zip", true);
```

### �ļ���ϣУ��

#### VerifyHash

```csharp
public static Boolean VerifyHash(this FileInfo file, String hash)
```

��֤�ļ���ϣ�Ƿ�ƥ��Ԥ��ֵ��

**֧�ֵ��㷨**��
- MD5��16λ��32λ��
- SHA1
- SHA256
- SHA512
- CRC32

**��ϣ��ʽ**��
- ��ǰ׺��`md5$abc123...`��`sha256$def456...`��`crc32$12345678`
- ��ǰ׺�����ݳ����Զ�ʶ��
  - 8 �ַ���CRC32
  - 16/32 �ַ���MD5
  - 40 �ַ���SHA1
  - 64 �ַ���SHA256
  - 128 �ַ���SHA512

**ʾ��**��
```csharp
var file = "app.exe".AsFile();

// ���㷨ǰ׺
file.VerifyHash("md5$d41d8cd98f00b204e9800998ecf8427e")
file.VerifyHash("sha256$e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855")
file.VerifyHash("crc32$00000000")

// ��ǰ׺���Զ�ʶ��
file.VerifyHash("d41d8cd98f00b204e9800998ecf8427e")  // 32λ -> MD5
file.VerifyHash("d41d8cd98f00b204")                  // 16λ -> MD5��ǰ8�ֽڣ�
file.VerifyHash("12345678")                          // 8λ -> CRC32
```

## ʹ�ó���

### 1. �����ļ�����

```csharp
public class ConfigManager
{
    public T Load<T>(String configName) where T : new()
    {
        var path = $"config/{configName}.json".GetFullPath();
        path.EnsureDirectory(true);
        
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<T>(json) ?? new T();
        }
        
        return new T();
    }
}
```

### 2. ��־Ŀ¼����

```csharp
public class LogManager
{
    public String GetLogPath()
    {
        var today = DateTime.Today;
        var path = $"logs/{today:yyyy}/{today:MM}/{today:dd}/".GetBasePath();
        path.EnsureDirectory(false);
        return path;
    }
}
```

### 3. ����������У��

```csharp
public class UpdateManager
{
    public async Task<Boolean> UpdateAsync(String url, String expectedHash)
    {
        var tempFile = Path.GetTempFileName();
        
        // �����ļ�
        await DownloadAsync(url, tempFile);
        
        // У���ϣ
        if (!tempFile.AsFile().VerifyHash(expectedHash))
        {
            File.Delete(tempFile);
            return false;
        }
        
        // ��ѹ����
        tempFile.AsFile().Extract("update_temp", overwrite: true);
        
        return true;
    }
}
```

### 4. ��Ŀ����

```csharp
public class Deployer
{
    public void Deploy(String sourceDir, String targetDir)
    {
        var source = sourceDir.AsDirectory();
        
        // �������и��µ��ļ�
        var updated = source.CopyToIfNewer(targetDir, "*.dll;*.exe;*.json", true, name =>
        {
            Console.WriteLine($"����: {name}");
        });
        
        Console.WriteLine($"������ {updated.Length} ���ļ�");
        
        // ѹ������
        targetDir.AsDirectory().Compress($"backup_{DateTime.Now:yyyyMMdd}.zip");
    }
}
```

## ���ʵ��

### 1. ʼ��ʹ�� GetFullPath ����·��

```csharp
// �Ƽ���ʹ����չ������ȡ����·��
var path = "config/app.json".GetFullPath();

// ���Ƽ���ֱ��ʹ�����·��
var path = "config/app.json";  // �����ڲ�ͬ��������Ϊ��һ��
```

### 2. �����ļ�ǰȷ��Ŀ¼����

```csharp
// �Ƽ�����ȷ��Ŀ¼����
var path = "logs/2024/01/app.log".GetFullPath();
path.EnsureDirectory(true);
File.WriteAllText(path, content);

// ���Ƽ��������׳� DirectoryNotFoundException
File.WriteAllText("logs/2024/01/app.log", content);
```

### 3. ʹ�� AsFile/AsDirectory ��ʽ����

```csharp
// ������ʽ����
var size = "data.bin".AsFile().ReadBytes().Length;
var files = "src".AsDirectory().GetAllFiles("*.cs", true).Count();
```

## ƽ̨����

| ���� | Windows | Linux |
|------|---------|-------|
| ·���ָ��� | `\` | `/` |
| 7z ѹ�� | ? ֧�� | ? ��֧�� |
| tar.gz ѹ�� | .NET 7+ ԭ��֧�� | .NET 7+ ԭ��֧�� |

## �������

- [������չ IOHelper](io_helper-������չIOHelper.md)
- [ѹ����ѹ��](compression-ѹ����ѹ��.md)
- [��ȫ��չ SecurityHelper](security_helper-��ȫ��չSecurityHelper.md)
