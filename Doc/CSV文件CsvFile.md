# CsvFile ʹ���ֲ�

���ĵ�����Դ�� `DH.NCore/IO/CsvFile.cs`������˵�� `CsvFile`��CSV ��д���������Ŀ�ꡢRFC4180 ������Ϊ��ͬ��/�첽 API���Լ����ļ������µ�ʹ�ý��顣

> �ؼ��ʣ�RFC4180����ʽ�����������ֶΡ�CRLF��������д��Encoding��Separator��

---

## 1. ����

`CsvFile` ��һ�����򡰳��� CSV �ļ��������������֧࣬�֣�

- **���У�Record����ȡ**������¼���������Ǽ� `ReadLine()+Split`��
- **����д��**������׷��д�룬����һ���Թ��������ļ���
- **RFC4180 �����������**��
  - �ֶ�ʹ�� `Separator` �ָ���
  - �ֶ��к��ָ���/����/˫����ʱʹ��˫���Ű�����
  - �ֶ���˫������ `""` ת�壻
  - ���������ֶ��ڲ����ֻ��У������ֶΣ���

���ó�����

- ���ݵ��뵼����
- ��Ҫ�Գ��� CSV ����ʽ��ȡ���߶��ߴ�����
- ��Ҫ��ȷ����������/����/���ŵ��ֶΡ�

---

## 2. ��������

### 2.1 `Encoding`

- ���ͣ�`Encoding`
- Ĭ�ϣ�`Encoding.UTF8`

Ӱ�죺

- ��ȡʱ���ڹ��� `StreamReader`��
- д��ʱ���ڹ��� `StreamWriter`��

˵����

- `EnsureReader()` ʹ�� `new StreamReader(_stream, Encoding)`��Ĭ������ BOM ��⣨`detectEncodingFromByteOrderMarks=true` ΪĬ����Ϊ����

### 2.2 `Separator`

- ���ͣ�`Char`
- Ĭ�ϣ�`,`

˵����

- ��ȡʱ�����ֶηָ���
- д��ʱ����ƴ���ֶΣ����ݴ��ж��Ƿ���Ҫ�����š�

---

## 3. ��������Դ����

### 3.1 ���췽ʽ

- `CsvFile(Stream stream)`
- `CsvFile(Stream stream, Boolean leaveOpen)`
- `CsvFile(String file, Boolean write = false)`

`write=false`���� `FileAccess.Read` �򿪣�ֻ������

`write=true`���� `FileAccess.ReadWrite` �򿪣���д�����Զ��ضϣ����ʺ�����׷�ӻ򸲸�д������дʱ�ɵ��÷����� `Position/SetLength`����

### 3.2 `leaveOpen`

��ʹ�� `CsvFile(Stream, leaveOpen:true)`��

- `Dispose()` ����ر� `_stream`��
- ���Ի� `Flush()`/�ͷ��ڲ� `_reader/_writer`��

### 3.3 Dispose ��Ϊ

- `Dispose()` ���� `_writer?.Flush()`������д��������δ���̣�
- �� `_leaveOpen=false`�����ͷ� `_reader/_writer` ���ر�����

�� `NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER` �»��ṩ `DisposeAsync()`��

---

## 4. ��ȡ��RFC4180 ��� Record ������

### 4.1 `String[]? ReadLine()`

��ȡһ����¼��Record���������ֶ����飺

- EOF ���� `null`��
- ֧�������ֶ��ڲ����� `Separator`��`\r\n`��`\n`��
- `""` ����Ϊ `"`��
- ֧��β�����ֶΣ����� `a,b,` => �����ֶΣ����һ��Ϊ���ַ�������

### 4.2 `IEnumerable<String[]> ReadAll()`

ͬ��ö�ٶ�ȡȫ����¼��

- �ڲ�ѭ������ `ReadLine()` ֱ�� EOF��

### 4.3 �첽��ȡ

���� `NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER` �¿��ã�

- `ValueTask<String[]?> ReadLineAsync()`
- `IAsyncEnumerable<String[]> ReadAllAsync()`

�첽ʵ�ֲ����ڲ��ַ���������`Char[4096]`��������

---

## 5. д�루RFC4180 ���ת�壩

### 5.1 `void WriteLine(IEnumerable<Object?> line)`

дһ�м�¼���Զ�׷����β���У���

- `DateTime`��ʹ�� `ToFullString("")`��
- `Boolean`��д `1/0`��
- �������ͣ�`item + ""` ת�ַ�����
- **�������ַ��������� > 9 �ҿɽ���Ϊ Int64��**��ǰ�� `\t`�����ڱ��� Excel/WPS ����ʾΪ��ѧ��������
- ���ֶΰ������ָ���/CR/LF/˫���ţ��������˫���ţ������ڲ�˫�����滻Ϊ `""`��

### 5.2 `void WriteAll(IEnumerable<IEnumerable<Object?>> data)`

����д�룺

- �ڲ�ѭ������ `WriteLine(line)`��

### 5.3 `Task WriteLineAsync(IEnumerable<Object> line)`

�첽дһ�У���д�����첽����

ע�⣺

- �÷�������Ϊ `IEnumerable<Object>`�������� `null` ���ͬ�� `Object?` ��ͬ����

---

## 6. ��Сʾ��

### 6.1 ��ȡ CSV

```csharp
using NewLife.IO;

using var csv = new CsvFile("./data.csv");

while (true)
{
    var row = csv.ReadLine();
    if (row == null) break;

    // row ���ֶ�����
    // ���磺row[0], row[1] ...
}
```

### 6.2 д�� CSV������д��

```csharp
using NewLife.IO;

using var csv = new CsvFile("./out.csv", write: true);

// ֱ��д��
csv.WriteLine("Id", "Name", "Remark");
csv.WriteLine(1, "Stone", "hello,world");
```

### 6.3 ׷��д��������

```csharp
using NewLife.IO;

using var csv = new CsvFile("./out.csv", write: true);

// ���캯�� write:true �� ReadWrite ���Ҳ��ض�
// ��Ҫ׷�ӵ�β���������ж�λ
// ��Ҳ���� FileStream + CsvFile(stream, true) ����

csv.WriteLine(DateTime.Now, true, "append");
```

---

## 7. ע�������볣������

### 7.1 ��ȡ���ǰ��������С����ǰ�����¼��

���ֶα�˫���Ű������ڲ�������ʱ��`ReadLine()` ���Խ���ж������űպ�Ϊֹ������ CSV ������ȷ����Ϊ��

### 7.2 `Separator` ��Ϊ�Ʊ�����TSV��

```csharp
var csv = new CsvFile(file) { Separator = '\t' };
```

д��ʱ��ݴ��ж��Ƿ���Ҫ���š�

### 7.3 ����ѡ��

- Excel ��ĳЩ�����¶� UTF-8 �� BOM ʶ�𲻼ѣ�����Ҫ���ݣ��ɿ���д��ǰ������� BOM ����� `Encoding.UTF8` �� BOM �İ汾���ɵ��÷�������д�룩��

---

## 8. �������

- 历史文档已归档，当前请以仓库内 Doc 为准
- Դ�룺`DH.NCore/IO/CsvFile.cs`
