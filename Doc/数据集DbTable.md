# DbTable ʹ���ֲ�

`NewLife.Data.DbTable` ��һ�����������ڴ����ݱ������ڳ��ء��У��ֶΣ�+ �У���¼�����ṹ�����ݡ�

���ó�����

- DAL ��ѯ��ѽ�������浽�ڴ棬֧�ֶ�α�����ɸѡ��ת��
- �ڲ����� `DataTable` ������½��п�ƽ̨���ݽ���
- �����ݶ�дΪ�����ƣ���Ч/��ѹ������Json��Xml��Csv
- �ڱ�������ģ���б�֮����ӳ��

- �����ռ䣺`NewLife.Data`
- ������ͣ�`DbTable`��`DbRow`

文档站点已归档，当前请以仓库内 Doc 为准

---

## 1. ���ݽṹ

`DbTable` �ĺ������Ĳ�����ɣ�

- `Columns`����������
- `Types`�����������飨�� `Columns` һһ��Ӧ��
- `Rows`���м��ϣ�ÿ���� `Object?[]`���� `Columns/Types` ���룩
- `Total`������������ȡ/д�������ʱ��ʹ�ã�

ʾ����

```csharp
var dt = new DbTable
{
    Columns = ["Id", "Name", "Enable"],
    Types = [typeof(Int32), typeof(String), typeof(Boolean)],
    Rows =
    [
        new Object?[] { 1, "Stone", true },
        new Object?[] { 2, "NewLife", false },
    ],
    Total = 2
};
```

ע�⣺

- ͨ�� `Rows[i].Length == Columns.Length`
- �����ƶ�д���� `Types`������ʼ���� `Columns` ͬ������

---

## 2. �����ݿ��ȡ��`IDataReader` / `DbDataReader`��

### 2.1 ͬ����ȡ

```csharp
using var cmd = connection.CreateCommand();
cmd.CommandText = "select Id, Name from User";

using var dr = cmd.ExecuteReader();

var table = new DbTable();
table.Read(dr);

Console.WriteLine(table); // DbTable[����][����]
```

`Read(dr)` ���Զ����ã�

- `ReadHeader(dr)`����ȡ�������ֶ�����
- `ReadData(dr)`�����ж�ȡ����� `Rows/Total`

### 2.2 �첽��ȡ

```csharp
await using var cmd = connection.CreateCommand();
cmd.CommandText = "select Id, Name from User";

await using var dr = await cmd.ExecuteReaderAsync();

var table = new DbTable();
await table.ReadAsync(dr);
```

������ڲ�ʹ�� `ConfigureAwait(false)`��

### 2.3 ָ���ֶ�ӳ�䣨`fields`��

`ReadData`/`ReadDataAsync` ֧�ִ��� `fields`�����ڽ���Ŀ���� i��ӳ�䵽��ȡ���ġ�Դ�� fields[i]����

```csharp
// Ŀ���У�Id, Name
table.Columns = ["Id", "Name"];
table.Types = [typeof(Int32), typeof(String)];

// �Ӷ�ȡ���ĵ� 2 �к͵� 0 ��ȡֵ
table.ReadData(dr, fields: [2, 0]);
```

#### `DBNull` Ĭ��ֵ����

��ȡ�� `DBNull.Value` ʱ��`DbTable` �ᰴ��������д��Ĭ��ֵ��������ֵ `0`��`false`��`DateTime.MinValue`���������Ǳ��� `null`��

---

## 3. �� `DataTable` ��ת

### 3.1 �� `DataTable` ��ȡ

```csharp
var table = new DbTable();
var count = table.Read(dataTable);
```

- ��� `dataTable.Columns` ��ȡ����������
- ���ÿ�е� `ItemArray` ��Ϊ `Object?[]` ���� `Rows`

### 3.2 д�뵽 `DataTable`

```csharp
DataTable dataTable = table.ToDataTable();

// �������ж���
var dt2 = table.Write(existing);
```

---

## 4. ���������л����Ƽ���

`DbTable` ���ö����ƶ�д���ʺϴ�����������/���̣�

- ͷ�������� + �汾 + ��� + �ж��� + ����
- �����壺��������˳����ֵд��
- `*.gz` �ļ����Զ�ѹ��/��ѹ

### 4.1 д��/��ȡ `Stream`

```csharp
using var ms = new MemoryStream();
table.Write(ms);

ms.Position = 0;
var table2 = new DbTable();
table2.Read(ms);
```

### 4.2 תΪ���ݰ� `IPacket`

�ʺ����紫�䣨ͷ��Ԥ�� 8 �ֽڣ������ϲ�Э��׷�Ӱ�ͷ����

```csharp
IPacket pk = table.ToPacket();
```

### 4.3 ����/�����ļ�

```csharp
table.SaveFile("data.db");
table.SaveFile("data.db.gz", compressed: true);

var t2 = new DbTable();
t2.LoadFile("data.db");
```

### 4.4 ���������߶��ߴ���������һ���Լ���ȫ�� `Rows`��

```csharp
var table = new DbTable();
foreach (var row in table.LoadRows("data.db.gz"))
{
    // row �� Object?[]
}
```

˵����

- `LoadRows` ���ȶ�ȡͷ�����ٸ��� `Total` ������ȡ����
- �� `Total == 0` ���ļ��ǿգ����� `rows = -1` һֱ����������

### 4.5 ���������ߴ�����д��

```csharp
var table = new DbTable
{
    Columns = ["Id", "Name"],
    Types = [typeof(Int32), typeof(String)]
};

IEnumerable<Object?[]> rows = GetRows();
var count = table.SaveRows("data.db.gz", rows);
```

ָ����ӳ��˳��

```csharp
var fields = new[] { 1, 0 }; // Ŀ���� i ��ӦԴ row �� fields[i]
var count = table.SaveRows("data.db", rows, fields);
```

- `fields[i] == -1` ��ʾд���ֵ����Ŀ��������д�룩

---

## 5. Json ���л�

`ToJson()` ����ת��Ϊ���ֵ����顱�����л���

```csharp
String json = table.ToJson(indented: true);
```

�ֵ�������ʽ��

```csharp
IList<IDictionary<String, Object?>> list = table.ToDictionary();
```

- �ֵ� key Ϊ���� `Columns[i]`
- value Ϊ��ֵ `row[i]`

---

## 6. Xml ���л�

`GetXml()` ������ `DbTable` ���ڵ㣬�ڲ�ÿ���� `Table` �ڵ㣬ÿ��������Ϊ�ӽڵ㡣

```csharp
String xml = table.GetXml();
```

д�뵽���� `Stream`��

```csharp
await table.WriteXml(stream);
```

����д����ԣ�

- `Boolean`��д�벼��ֵ
- `DateTime`��д�� `DateTimeOffset`
- `DateTimeOffset`��ֱ��д��
- `IFormattable`������ʽд���ַ���
- ������`ToString()`

---

## 7. Csv ���л�

```csharp
table.SaveCsv("data.csv");

var t2 = new DbTable();
t2.LoadCsv("data.csv");
```

- `SaveCsv`����д��ͷ�������У���д��������
- `LoadCsv`����һ����Ϊ `Columns`��������Ϊ������

---

## 8. ģ�ͻ�ת

### 8.1 ģ���б�д�� `DbTable`

```csharp
var list = new[]
{
    new User { Id = 1, Name = "Stone" },
    new User { Id = 2, Name = "NewLife" },
};

var table = new DbTable();
table.WriteModels(list);
```

����

- ѡ�� `T` �Ĺ�������
- �������������������ԡ���`IsBaseType()`��
- �� `Columns` Ϊ�����Զ����������� `Columns/Types`
- ��ֵͨ�������ȡ����ģ��ʵ�� `IModel`���������������� `model[name]`

### 8.2 `DbTable` ��ȡΪģ���б�

```csharp
IEnumerable<User> users = table.ReadModels<User>();

// ��ָ�� Type
IEnumerable<Object> objs = table.ReadModels(typeof(User));
```

ӳ�����

- ʹ�� `SerialHelper.GetName(PropertyInfo)` ��ȡ�ֶ�����֧�����Ա�����
- ������Сд������ƥ������
- ��Ŀ��ģ��ʵ�� `IModel`����ͨ����������ֵ������ͨ������ `SetValue`

---

## 9. ��ݶ�ȡ�������л�ȡֵ��

```csharp
var name = table.Get<String>(row: 0, name: "Name");

if (table.TryGet<Int32>(1, "Id", out var id))
{
    // ...
}
```

- `GetColumn(name)` ֧�ֺ��Դ�Сд

---

## 10. ö���� `DbRow`

`DbTable` ʵ�� `IEnumerable<DbRow>`����ֱ�� `foreach`��

```csharp
foreach (var row in table)
{
    // row �� DbRow
}
```

��ȡָ���У�

```csharp
var row = table.GetRow(0);
```

---

## 11. ��¡

`Clone()` Ϊǳ������

- `Columns/Types` ����Ϊ������
- `Rows` ʹ�� `ToList()` �������б����������������Թ���

```csharp
var copy = table.Clone();
```

---

## 12. ��������

### 12.1 Ϊʲô `DBNull` ����Ĭ��ֵ��

���� `DbTable.ReadData(...)` �ļȶ����ԣ������������Ĭ��ֵ�����ں���ֱ������ֵ/����/ʱ����㡣

��ҵ����Ҫ���� `null` ���壬�����ϲ����д�����

### 12.2 �����Ƹ�ʽ�Ƿ��ȶ���

�����Ƹ�ʽ�����汾�ţ���ǰ�汾Ϊ `3`������ȡʱ�������߰汾���׳� `InvalidDataException`��

### 12.3 ��������������ô�ã�

- ��ȡ������ `LoadRows`/`ReadRows` ��������
- д�룺���� `SaveRows`/`WriteRows` ����д��
- ���紫�䣺ʹ�� `ToPacket()`
