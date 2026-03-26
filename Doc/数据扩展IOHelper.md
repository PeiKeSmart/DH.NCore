# ������չ IOHelper

## ����

`IOHelper` �� DH.NCore �е� IO ���������࣬�ṩ��Ч���������������ֽ�����ת����ѹ����ѹ������ת���ȹ��ܡ���� .NET 6+ �� `Stream.Read` ����仯�����˼����Դ�����ȷ�������п�ܰ汾����Ϊһ�¡�

**�����ռ�**��`NewLife`  
**文档地址**：历史文档已归档，当前请以仓库内 Doc 为准

## ��������

- **��ȷ��ȡ**��`ReadExactly` ȷ����ȡָ���ֽ�������� .NET 6+ ���ֶ�ȡ����
- **ѹ����ѹ**��֧�� Deflate �� GZip �����㷨
- **�ֽ���ת��**��֧�ִ��/С���ֽ���ת��
- **ʮ�����Ʊ���**����Ч��ʮ�������ַ���ת��
- **�䳤����**��֧�� 7-bit �����ѹ��������д

## ���ٿ�ʼ

```csharp
using NewLife;

// �ֽ�����תʮ�������ַ���
var hex = new Byte[] { 0x12, 0xAB, 0xCD }.ToHex();  // "12ABCD"

// ʮ�������ַ���ת�ֽ�����
var data = "12ABCD".ToHex();  // [0x12, 0xAB, 0xCD]

// Base64 �����
var base64 = data.ToBase64();
var bytes = base64.ToBase64();

// ѹ������
var compressed = data.Compress();
var decompressed = compressed.Decompress();

// ��ת�ַ���
using var stream = new MemoryStream(Encoding.UTF8.GetBytes("Hello"));
var str = stream.ToStr();  // "Hello"
```

## API �ο�

### ��������

#### MaxSafeArraySize

```csharp
public static Int32 MaxSafeArraySize { get; set; } = 1024 * 1024;
```

���ȫ�����С�������ô�Сʱ����ȡ���ݲ�����ǿ��ʧ�ܡ�

**��;**�����������ã���������������ʱ��ȡ�������鵼��Ӧ�ñ�����

**ʾ��**��
```csharp
// ��Ҫ������Ͷ���������ʱ���ʵ��ſ�
IOHelper.MaxSafeArraySize = 10 * 1024 * 1024;  // 10MB
```

### ��������ȡ

#### ReadExactly

```csharp
public static Int32 ReadExactly(this Stream stream, Byte[] buffer, Int32 offset, Int32 count)
public static Byte[] ReadExactly(this Stream stream, Int64 count)
```

��ȷ��ȡָ���ֽ����������ݲ������׳� `EndOfStreamException`��

**����**��.NET 6 ��ʼ��`Stream.Read` ���ܷ��ز������ݣ�partial read����������ȷ����ȡ�������ݡ�

**ʾ��**��
```csharp
using var fs = File.OpenRead("data.bin");

// ��ȡ�̶����ȵ�Э��ͷ
var header = new Byte[16];
fs.ReadExactly(header, 0, 16);

// ��ȡ������������
var data = fs.ReadExactly(1024);
```

#### ReadAtLeast

```csharp
public static Int32 ReadAtLeast(this Stream stream, Byte[] buffer, Int32 offset, Int32 count, Int32 minimumBytes, Boolean throwOnEndOfStream = true)
```

��ȡ����ָ���ֽ�����������ȡ���൫������ `count`��

**����˵��**��
- `minimumBytes`��������Ҫ��ȡ���ֽ���
- `throwOnEndOfStream`�����ݲ���ʱ�Ƿ��׳��쳣

**ʾ��**��
```csharp
var buffer = new Byte[1024];

// ���ٶ�ȡ 100 �ֽڣ���� 1024 �ֽ�
var read = stream.ReadAtLeast(buffer, 0, 1024, 100, throwOnEndOfStream: false);
if (read < 100)
{
    Console.WriteLine("���ݲ���");
}
```

#### ReadBytes

```csharp
public static Byte[] ReadBytes(this Stream stream, Int64 length)
```

�����ж�ȡָ�����ȵ��ֽ����顣

**����˵��**��
- `length`��Ҫ��ȡ���ֽ�����-1 ��ʾ��ȡ����ĩβ

**ʾ��**��
```csharp
// ��ȡָ������
var data = stream.ReadBytes(1024);

// ��ȡȫ��ʣ������
var all = stream.ReadBytes(-1);
```

### ������д��

#### Write

```csharp
public static Stream Write(this Stream des, params Byte[] src)
```

���ֽ�����д����������

**ʾ��**��
```csharp
using var ms = new MemoryStream();
ms.Write(new Byte[] { 1, 2, 3 });
ms.Write(new Byte[] { 4, 5, 6 });
```

#### WriteArray / ReadArray

```csharp
public static Stream WriteArray(this Stream des, params Byte[] src)
public static Byte[] ReadArray(this Stream des)
```

д��/��ȡ������ǰ׺���ֽ����飨ʹ�� 7-bit ����������Ϊ���ȣ���

**ʾ��**��
```csharp
using var ms = new MemoryStream();

// д�������ǰ׺������
ms.WriteArray(new Byte[] { 1, 2, 3, 4, 5 });

// ��ȡ
ms.Position = 0;
var data = ms.ReadArray();  // [1, 2, 3, 4, 5]
```

### ѹ����ѹ

#### Compress / Decompress��Deflate��

```csharp
public static Stream Compress(this Stream inStream, Stream? outStream = null)
public static Stream Decompress(this Stream inStream, Stream? outStream = null)
public static Byte[] Compress(this Byte[] data)
public static Byte[] Decompress(this Byte[] data)
```

ʹ�� Deflate �㷨ѹ��/��ѹ���ݡ�

**ʾ��**��
```csharp
// ѹ���ֽ�����
var data = Encoding.UTF8.GetBytes("Hello World!");
var compressed = data.Compress();
var decompressed = compressed.Decompress();

// ѹ��������
using var input = new MemoryStream(data);
using var output = new MemoryStream();
input.Compress(output);
```

#### CompressGZip / DecompressGZip

```csharp
public static Stream CompressGZip(this Stream inStream, Stream? outStream = null)
public static Stream DecompressGZip(this Stream inStream, Stream? outStream = null)
public static Byte[] CompressGZip(this Byte[] data)
public static Byte[] DecompressGZip(this Byte[] data)
```

ʹ�� GZip �㷨ѹ��/��ѹ���ݡ�GZip ��ʽ�����ļ�ͷ��Ϣ�������Ը��á�

**ʾ��**��
```csharp
var data = File.ReadAllBytes("large-file.txt");
var gzipped = data.CompressGZip();
File.WriteAllBytes("large-file.txt.gz", gzipped);
```

### �ֽ���ת��

#### ToUInt16 / ToUInt32 / ToUInt64

```csharp
public static UInt16 ToUInt16(this Byte[] data, Int32 offset = 0, Boolean isLittleEndian = true)
public static UInt32 ToUInt32(this Byte[] data, Int32 offset = 0, Boolean isLittleEndian = true)
public static UInt64 ToUInt64(this Byte[] data, Int32 offset = 0, Boolean isLittleEndian = true)
```

���ֽ������ȡ������֧�ִ��/С���ֽ���

**ʾ��**��
```csharp
var data = new Byte[] { 0x01, 0x00, 0x00, 0x00 };

// С����Ĭ�ϣ�
var value1 = data.ToUInt32();                    // 1
// �����
var value2 = data.ToUInt32(isLittleEndian: false);  // 16777216
```

#### GetBytes

```csharp
public static Byte[] GetBytes(this Int16 value, Boolean isLittleEndian = true)
public static Byte[] GetBytes(this Int32 value, Boolean isLittleEndian = true)
public static Byte[] GetBytes(this Int64 value, Boolean isLittleEndian = true)
// ... ��������
```

������ת��Ϊ�ֽ����顣

**ʾ��**��
```csharp
var bytes1 = 12345.GetBytes();                      // С����
var bytes2 = 12345.GetBytes(isLittleEndian: false); // �����
```

### ʮ�����Ʊ���

#### ToHex

```csharp
public static String ToHex(this Byte[] data, Int32 offset = 0, Int32 count = -1)
public static String ToHex(this Byte[] data, String? separate, Int32 lineSize = 0)
```

���ֽ�����ת��Ϊʮ�������ַ�����

**ʾ��**��
```csharp
var data = new Byte[] { 0x12, 0xAB, 0xCD, 0xEF };

// ����ת��
data.ToHex()                         // "12ABCDEF"

// ���ָ���
data.ToHex("-")                      // "12-AB-CD-EF"
data.ToHex(" ")                      // "12 AB CD EF"

// ������ʾ
var largeData = new Byte[32];
largeData.ToHex(" ", lineSize: 16)   // ÿ 16 �ֽ�һ��
```

#### ToHex���ַ���ת�ֽ����飩

```csharp
public static Byte[] ToHex(this String? data)
```

��ʮ�������ַ���ת��Ϊ�ֽ����顣

**ʾ��**��
```csharp
"12ABCDEF".ToHex()           // [0x12, 0xAB, 0xCD, 0xEF]
"12-AB-CD-EF".ToHex()        // [0x12, 0xAB, 0xCD, 0xEF]���Զ����Էָ�����
"12 AB CD EF".ToHex()        // [0x12, 0xAB, 0xCD, 0xEF]
```

### Base64 ����

#### ToBase64

```csharp
public static String ToBase64(this Byte[] data)
public static Byte[] ToBase64(this String? data)
```

Base64 ����롣

**ʾ��**��
```csharp
// ����
var data = Encoding.UTF8.GetBytes("Hello");
var base64 = data.ToBase64();        // "SGVsbG8="

// ����
var bytes = base64.ToBase64();       // [72, 101, 108, 108, 111]
```

### �ַ���ת��

#### ToStr

```csharp
public static String ToStr(this Stream stream, Encoding? encoding = null)
public static String ToStr(this Byte[] buf, Encoding? encoding = null, Int32 offset = 0, Int32 count = -1)
```

�������ֽ�����ת��Ϊ�ַ������Զ����� BOM��

**ʾ��**��
```csharp
// ��ת�ַ���
using var stream = new MemoryStream(Encoding.UTF8.GetBytes("Hello"));
var str = stream.ToStr();  // "Hello"

// �ֽ�����ת�ַ���
var data = Encoding.UTF8.GetBytes("����");
var text = data.ToStr(Encoding.UTF8);  // "����"
```

### �䳤��������

#### WriteEncodedInt / ReadEncodedInt

```csharp
public static Stream WriteEncodedInt(this Stream stream, Int32 value)
public static Int32 ReadEncodedInt(this Stream stream)
```

ʹ�� 7-bit �����д�䳤������Сֵռ�ø����ֽڡ�

**�������**��
- 0-127��1 �ֽ�
- 128-16383��2 �ֽ�
- 16384-2097151��3 �ֽ�
- �Դ�����

**ʾ��**��
```csharp
using var ms = new MemoryStream();

// д��Сֵֻ�� 1 �ֽ�
ms.WriteEncodedInt(100);   // 1 �ֽ�

// д���ֵ��Ҫ�����ֽ�
ms.WriteEncodedInt(10000); // 2 �ֽ�

// ��ȡ
ms.Position = 0;
var v1 = ms.ReadEncodedInt();  // 100
var v2 = ms.ReadEncodedInt();  // 10000
```

### ʱ���д

#### WriteDateTime / ReadDateTime

```csharp
public static Stream WriteDateTime(this Stream stream, DateTime dt)
public static DateTime ReadDateTime(this Stream stream)
```

�� Unix ʱ������룩��ʽ��дʱ�䣬4 �ֽڴ洢��

**ʾ��**��
```csharp
using var ms = new MemoryStream();

ms.WriteDateTime(DateTime.Now);

ms.Position = 0;
var dt = ms.ReadDateTime();
```

### �ֽ��������

#### ReadBytes

```csharp
public static Byte[] ReadBytes(this Byte[] src, Int32 offset, Int32 count)
```

���ֽ������и���ָ����Χ�����ݡ�

**ʾ��**��
```csharp
var data = new Byte[] { 1, 2, 3, 4, 5 };
var part = data.ReadBytes(1, 3);  // [2, 3, 4]
var rest = data.ReadBytes(2, -1); // [3, 4, 5]��-1 ��ʾ��ĩβ��
```

#### Write

```csharp
public static Int32 Write(this Byte[] dst, Int32 dstOffset, Byte[] src, Int32 srcOffset = 0, Int32 count = -1)
```

���ֽ�����д�����ݡ�

**ʾ��**��
```csharp
var buffer = new Byte[10];
var data = new Byte[] { 1, 2, 3 };
var written = buffer.Write(0, data);  // д�� 3 �ֽ�
```

#### CopyTo��ָ�����ȣ�

```csharp
public static void CopyTo(this Stream source, Stream destination, Int64 length, Int32 bufferSize)
```

��Դ������ָ���������ݵ�Ŀ������

**ʾ��**��
```csharp
using var source = File.OpenRead("large-file.bin");
using var dest = File.Create("part.bin");

// ֻ����ǰ 1MB
source.CopyTo(dest, 1024 * 1024, bufferSize: 81920);
```

## ʹ�ó���

### 1. ����Э�����

```csharp
public class ProtocolParser
{
    public Message Parse(Stream stream)
    {
        // ��ȡ�̶����ȵ�Э��ͷ
        var header = stream.ReadExactly(8);
        
        var magic = header.ToUInt32(0);
        var length = header.ToUInt32(4);
        
        // ��ȡ��Ϣ��
        var body = stream.ReadExactly(length);
        
        return new Message { Header = header, Body = body };
    }
}
```

### 2. ���������л�

```csharp
public class BinarySerializer
{
    public void Serialize(Stream stream, Object obj)
    {
        var json = JsonSerializer.Serialize(obj);
        var data = Encoding.UTF8.GetBytes(json);
        
        // д�볤�Ⱥ�����
        stream.WriteArray(data);
    }
    
    public T Deserialize<T>(Stream stream)
    {
        var data = stream.ReadArray();
        var json = data.ToStr(Encoding.UTF8);
        return JsonSerializer.Deserialize<T>(json);
    }
}
```

### 3. ����ѹ������

```csharp
public class CompressedTransport
{
    public Byte[] Send(Byte[] data)
    {
        // ѹ������
        var compressed = data.CompressGZip();
        
        // �����������[ѹ����־][ԭʼ����][ѹ������]
        using var ms = new MemoryStream();
        ms.WriteByte(1);  // ѹ����־
        ms.WriteEncodedInt(data.Length);  // ԭʼ����
        ms.Write(compressed);
        
        return ms.ToArray();
    }
    
    public Byte[] Receive(Byte[] packet)
    {
        using var ms = new MemoryStream(packet);
        var compressed = ms.ReadByte() == 1;
        var originalLength = ms.ReadEncodedInt();
        var data = ms.ReadBytes(-1);
        
        return compressed ? data.DecompressGZip() : data;
    }
}
```

## ���ʵ��

### 1. ʹ�� ReadExactly ��� Read

```csharp
// �Ƽ���ȷ����ȡ��������
var data = stream.ReadExactly(100);

// ���Ƽ�������ֻ��ȡ��������
var buffer = new Byte[100];
stream.Read(buffer, 0, 100);  // .NET 6+ ���ܷ���С�� 100
```

### 2. ע���ֽ���

```csharp
// ������ϵͳ����ʱע���ֽ���
// ����Э��ͨ��ʹ�ô����
var networkValue = data.ToUInt32(isLittleEndian: false);

// ���ش洢ͨ��ʹ��С����x86/x64 Ĭ�ϣ�
var localValue = data.ToUInt32();
```

### 3. ʹ�ö���ؼ����ڴ����

```csharp
using NewLife.Collections;

// ʹ���ڴ�����
var ms = Pool.MemoryStream.Get();
try
{
    // ʹ����...
}
finally
{
    ms.Return(true);  // �黹�����
}
```

## �������

- [·����չ PathHelper](path_helper-·����չPathHelper.md)
- [��ȫ��չ SecurityHelper](security_helper-��ȫ��չSecurityHelper.md)
- [���ݰ� IPacket](packet-���ݰ�IPacket.md)
