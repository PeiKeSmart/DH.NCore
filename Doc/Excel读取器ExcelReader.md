# ExcelReader ʹ���ֲ�

���ĵ�����Դ�� `DH.NCore/IO/ExcelReader.cs`������˵�� `ExcelReader`�������� Excel xlsx ��ȡ�����Ķ�λ��֧�ַ�Χ�����ݶ�ȡ��ʽ������ת��������ʹ��ע�����

> �ؼ��ʣ�xlsx��ZipArchive��sharedStrings��styles��sheetData�������� AA/AB��ȱʧ�в��롢��ֵ��ʽ��

---

## 1. ����

`ExcelReader` ��һ�������ڡ��������ݡ��������� xlsx ��ȡ����

- ��֧�� `xlsx`��OpenXML���������� zip ѹ������
- ������������ Office/Interop �����
- ��ǰʵ��ֻ����С��������
  - �����ַ�����`xl/sharedStrings.xml`��
  - ��ʽ��`xl/styles.xml`�����ָ�ʽ��
  - ���������ݣ�`xl/worksheets/sheet*.xml` �е� `sheetData`��

���ó�����

- ������/������������� Excel��
- ֻ��Ҫ�ѹ��������ж�ȡ�ɶ������飻
- ����ע��ʽ���㡢�ϲ���Ԫ��ͼ������ע�ȸ������ԡ�

---

## 2. ��������Դ����

### 2.1 `ExcelReader(String fileName)`

- �Թ�����ʽ���ļ���`FileShare.ReadWrite`�������ļ�����������ռ��ʱ������
- �� `ZipArchive` ��ȡ zip ���ݣ�
- ���캯������������ `Parse()` ������Ҫ��������

### 2.2 `ExcelReader(Stream stream, Encoding encoding)`

- ���� xlsx �����������÷��������������ڣ��豣�ֿɶ�����
- `encoding` ���� zip ��Ŀ����/ע�͵ȱ��루һ��Ϊ UTF-8����

### 2.3 Dispose

`ExcelReader` �̳� `DisposeBase`��

- `Dispose(Boolean)` ������ `_entries` ���ͷ� `_zip`��
- ���ͬʱ�ͷ���ײ� `FileStream`�����ɹ��캯����������

���飺

- ʼ��ʹ�� `using var reader = new ExcelReader(...)`��

---

## 3. ��������

### 3.1 `FileName`

- ���ͣ�`String?`
- ���ļ����캯��ֱ�Ӹ�ֵ��
- �������캯���У��� `stream is FileStream` ʱȡ `fs.Name`��

### 3.2 `Sheets`

- ���ͣ�`ICollection<String>?`
- ���壺���ù��������Ƽ��ϣ������� `_entries.Keys`����

˵����

- `Parse()` ��ѹ���������ӳ�䵽��Ӧ `ZipArchiveEntry`��

---

## 4. ��ȡ����

### 4.1 `IEnumerable<Object?[]> ReadRows(String? sheet = null)`

���з������ݣ���һ��ͨ���Ǳ�ͷ����

- `sheet=null` ʱĬ��ȡ `Sheets.FirstOrDefault()`��
- �Ҳ������������� `ArgumentOutOfRangeException`��
- ��ȡ���̣�
  1. ��Ŀ�� sheet ��Ŀ����
  2. `XDocument.Load` ��ȡ XML��
  3. �ڸ��ڵ����� `sheetData`��
  4. ����ÿ�� `<row>`�������� `<c>` ��Ԫ����н�����

����ֵ��

- ÿһ����һ�� `Object?[]`��
- ֵ���ܱ�ת��Ϊ��`DateTime` / `TimeSpan` / `Int32` / `Int64` / `Decimal` / `Double` / `Boolean` / `String`��
- ��ֵ��ȱʧ���� `null` ��ʾ��

### 4.2 �ؼ���Ϊ����������ȱʧ�в���

Excel ��Ԫ�������� `A1`��`AB23`��ʵ�ֻ᣺

- ��������ĸΪ 0 ��������`A=0`��`B=1`��`AA=26`����
- �����г������У�����ֻ�� A��C�������Զ��� B ��Ϊ `null`��
- ���¼�������� `headerColumnCount`����������β����ȱʧҲ�Ჹ�뵽������һ�¡�

��ʹ�ã�

- ��ȡ������ӽ�����ά���񡱵�ֱ�۽ṹ��
- ����ֱ�Ӱ����������ʡ�

---

## 5. ��Ԫ�����ͽ�����ת������

### 5.1 �����ַ�����`t="s"`��

����Ԫ������ `t="s"`��

- `<v>` �洢���ǹ����ַ���������
- �ᵽ `_sharedStrings[sharedIndex]` ȡ��ʵ�ı���

�����ַ������� `xl/sharedStrings.xml`������Ŀ����ȱʧ����������

### 5.2 ������`t="b"`��

- `0/1` �� `true/false`��
- תΪ `Boolean`��

### 5.3 ��ʽ����ı���`t="str"`��

- �������⴦����ֱ��ȡ�ı�ֵ��

### 5.4 ����/����/ʱ�䣺��ʽ����ת��

����Ԫ��ֵΪ�ַ����Ҵ�����ʽ `_styles` ʱ��

- ��ȡ��Ԫ������ `s`��StyleIndex����
- ���� `styles[si]` �� `NumFmtId/Format` ����ת�����ԡ�

ת���߼�λ�� `ChangeType(Object? val, ExcelNumberFormat st)`��

- **����/ʱ��**��
  - ��������ʽ���� `yy`/`mmm` �� `NumFmtId` �� 14~17 ��Ϊ 22��
  - Excel ����ֵ�� 1900-01-01 Ϊ��׼����ʷ����ʵ�ֻ��� `d-2` ������
  - ʹ�� `AddSeconds(Math.Round((d - 2) * 24 * 3600))`��������ܸ�����

- **ʱ����**��TimeSpan����
  - ������`NumFmtId` �� 18~21 �� 45~47��
  - תΪ `TimeSpan.FromSeconds(Math.Round(d2 * 24 * 3600))`��

- **General / 0**��
  - ������`NumFmtId == 0`��
  - ���γ��� `Int32`��`Int64`��`Decimal(InvariantCulture)`��`Double`��

- **������ʽ**��
  - ������`NumFmtId` Ϊ 1/3/37/38��
  - ���� `Int32/Int64`��

- **С����ʽ**��
  - ������`NumFmtId` Ϊ 2/4/11/39/40��
  - ���� `Decimal(InvariantCulture)` �� `Double`��

- **�ٷֱ�**��
  - ������`NumFmtId` Ϊ 9/10��
  - ���� `Double`��ע�⣺�õ����� 0.x���� 12% => 0.12����

- **�ı���ʽ**��
  - ������`NumFmtId == 49`��
  - ���ɽ���Ϊ��ֵ����ת���ַ��������⵼��ʱ������ֵ���ͣ���

---

## 6. ��Сʾ��

### 6.1 ��ȡ��һ��������

```csharp
using NewLife.IO;

using var reader = new ExcelReader("./data.xlsx");

foreach (var row in reader.ReadRows())
{
    // ��һ��ͨ���Ǳ�ͷ
    // row[i] ������ String/Int32/DateTime/Boolean/TimeSpan/null
}
```

### 6.2 ָ������������

```csharp
using var reader = new ExcelReader("./data.xlsx");

var sheet = reader.Sheets?.FirstOrDefault();
if (!sheet.IsNullOrEmpty())
{
    foreach (var row in reader.ReadRows(sheet))
    {
    }
}
```

### 6.3 �� `CsvFile` ��ϣ�Excel ת CSV

```csharp
using NewLife.IO;

using var reader = new ExcelReader("./data.xlsx");
using var csv = new CsvFile("./out.csv", write: true);

foreach (var row in reader.ReadRows())
{
    csv.WriteLine(row);
}
```

---

## 7. ע�������볣������

### 7.1 ֻ��ȡ `sheetData`

��ʵ��ֻ��ȡ `sheetData`�����������

- �ϲ���Ԫ��mergedCells��
- ��ʽ���㣨ֻ�������
- ͼƬ/ͼ��/��ע

����Ҫ��չ���ɸ��� OpenXML �ṹ���� `ZipArchive` ��Ŀ����������

### 7.2 �ڴ�ռ��

��ǰʵ�ֶ�ÿ�� `ReadRows()`��

- �� `XDocument.Load` ������ sheet XML �����ڴ档

�Գ�����������ռ�ý϶��ڴ棻��Ҫ֧�ָ����ļ�����Ҫ��Ϊ `XmlReader` ��ʽ���������ڹ�����չ�����ڱ��ĵ���Χ����

### 7.3 ����ƫ�ƣ��� 2��

Դ���ж� Excel ��������ֵʹ�� `d - 2` ����ʷ������Ϊ������ƥ�������û���������������ھ������ϸ�Ҫ����Ҫ��Ͼ���������֤��

---

## 8. �������

- 历史文档已归档，当前请以仓库内 Doc 为准
- Դ�룺`DH.NCore/IO/ExcelReader.cs`
