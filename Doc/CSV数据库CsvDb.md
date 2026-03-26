# CsvDb ʹ���ֲ�

���ĵ�����Դ�� `DH.NCore/IO/CsvDb.cs` �������� `DH.NCore/IO/CsvFile.cs`������˵�� `CsvDb<T>`��CSV �ļ����������ݿ⣩�����Ŀ�ꡢ���ݸ�ʽ������ģ���� CRUD �÷���

> �ؼ��ʣ�׷��д��������˳���ѯ���������С���ͷӳ�䡢���񻺴桢���仺�桢���л���������

---

## 1. ����

`CsvDb<T>` ��һ���� CSV �ļ���Ϊ�־û��洢�ġ����������ݿ⡱���ʺϣ�

- ����������Ҫ **����׷�ӣ�Append��**��
- ��Ҫ **˳��ɨ��ʽ���ٲ�ѯ**��`Query`����
- �����޸�/ɾ�����޸�/ɾ�������ǡ�ȫ����д������
- ����˳�����SQLite �ȹ�ϵ�������Ƿ��ػ������𻵣�`CsvDb<T>` ��ȡʱ�� **��������**����߿ɻָ��ԡ�

��ҪԼ����

- **��֧���̰߳�ȫ**����ע����ȷҪ�����ȷ�����̲߳�������Դ���в��ַ���ʹ�� `lock (this)` ������������������������ơ�

---

## 2. �����ļ���ʽ

### 2.1 �ļ�ͷ��Header��

��������������Դ��ʵ�� `T` �Ĺ���ʵ�����ԣ�

- ͨ�����仺�� `_properties = typeof(T).GetProperties(...)` ��ȡ���ԣ�
- ����ʹ�� `SerialHelper.GetName(PropertyInfo)`�������������������Ա��������л���/����һ�¡�

д�ļ�ʱ��

- ���ļ�Ϊ�գ�`FileStream.Position == 0`��ʱд���ͷ��

��ȡ�ļ�ʱ��

- ������Ϊ CSV ������
- �������ļ��� -> ������������ӳ������ `columnToProperty`������ÿ�ж����ֵ䡣

### 2.2 �����У�Data Rows��

ÿ�ж�Ӧһ�� `T` ʵ����

д��ʱ��

- �� `T` ʵ�� `IModel`���������� `src[e.Name]` ��ȡֵ��
- ����ͨ������ `item.GetValue(e)` ��ȡ����ֵ��

��ȡʱ��

- ���� `new T()`��
- ��ÿ�г��԰�Ŀ����������������У�飨����/����/���ڵȣ���У��ʧ�����������ֶΣ�
- ���ַ��� `raw` ת��ΪĿ�����ͣ�`raw.ChangeType(pi.PropertyType)`��
- ��ͨ�� `IModel` �� `model.SetValue(pi, value)` ��ֵ��

---

## 3. ��������

### 3.1 `FileName`

- ���ͣ�`String?`
- ���壺CSV �����ļ�·��

ʹ��Ҫ��

- �������ã�δ���õ��û��׳� `ArgumentNullException`���� `GetFile()`����

### 3.2 `Encoding`

- ���ͣ�`Encoding`
- Ĭ�ϣ�`Encoding.UTF8`

Ӱ�죺

- ��ȡ��д�� CSV ʱ���ݸ� `CsvFile.Encoding`��

### 3.3 `Comparer`

- ���ͣ�`IEqualityComparer<T>`
- Ĭ�ϣ�`EqualityComparer<T>.Default`

��;��

- `Remove(T)` / `Remove(IEnumerable<T>)` / `Find(T)` / `Set(T, ...)` �������жϡ�ʵ���Ƿ���ͬ����

��ͨ�����캯�� `CsvDb(Func<T?, T?, Boolean> comparer)` �����Զ���Ƚ��߼���

---

## 4. ����ģ�ͣ�����д��

### 4.1 `BeginTransaction()`

- ��Ϊ���ѵ�ǰ�ļ�ȫ�����ݶ����ڴ棨`_cache = FindAll().ToList()`����
- ֮��� `Add/Remove/Set/Clear/Find/Query` �����ڻ������������Ƶ�� I/O����

### 4.2 `Commit()`

- ��Ϊ���� `_cache` ����д���ļ���`Write(_cache, false)`����Ȼ����ջ��档

### 4.3 `Rollback()`

- ��Ϊ������ջ��棬��д�ش��̡�

### 4.4 Dispose �Զ��ύ

`CsvDb<T>` �̳� `DisposeBase`���� `Dispose(Boolean)` �����л���� `Commit()`��

- �����������������л��棬�ͷŶ���ʱ���Զ��ύ��������ʷ������Ϊ����

���飺

- �ԡ���������Ϊ���ĳ�����������ʾ���� `Commit()`�������쳣ʱ���ύ��

---

## 5. д��/׷��

### 5.1 `Write(IEnumerable<T> models, Boolean append)`

���壺����д�롣

�ؼ��㣺

- ���ļ���ʽ��`FileMode.OpenOrCreate` + `FileAccess.ReadWrite` + `FileShare.ReadWrite`��
- `append=true` ʱ�ƶ����ļ�β��`fs.Position = fs.Length`��
- �ļ�Ϊ��ʱд���ͷ��
- д���ִ�� `fs.SetLength(fs.Position)`��
  - ����д��`append=false`���������ض�ԭ�ļ����ಿ�֣�
  - ׷��дʱҲ��ѳ�������Ϊ��ǰλ�ã�ͨ���ȼۣ���

### 5.2 `Add(T model)` / `Add(IEnumerable<T> models)`

- ���� `BeginTransaction()`����׷�ӵ� `_cache`��
- ����ֱ�� `Write(..., append:true)`��������á�

---

## 6. ��ѯ

### 6.1 `IEnumerable<T> Query(Func<T, Boolean>? predicate, Int32 count = -1)`

���壺˳��ɨ���ѯ�����践�ء�

��ΪҪ�㣺

- ��������`_cache!=null`��ʱ���ӻ���ö�٣������� `yield return`��
- δ��������ʱ��
  - ʹ�� `CsvFile.ReadLine()` ���¼��ȡ��
  - ������¼��Ϊ��ͷ��
  - ����ÿ����¼����ӳ����䵽�¶���

���д�����

- ����ת���������κ��쳣�ᱻ���񲢼�¼��`XTrace.WriteException(ex)`��������������
- ��һ����û���κ��ֶγɹ�ƥ�䣨`success == 0`������Ϊ���в�������

`count`��

- Ĭ�� `-1` ��ʾ�����ƣ�
- ÿ `yield` һ�κ� `--count`���� 0 ������

### 6.2 `T? Find(Func<T, Boolean>? predicate)`

- �ȼ��� `Query(predicate, 1).FirstOrDefault()`��

### 6.3 `IList<T> FindAll()`

- ��������ʱ���ػ��渱�� `_cache.ToList()`��
- �����ȡȫ����

### 6.4 `Int32 FindCount()`

- �����񳡾���ʹ�� `StreamReader.ReadLine()` ���м���������ͷ������
- ע�⣺���ﰴ�����м����������� CSV �����ֶ��ڻ��е�������ڳ�������� CSV ��ͨ���ɽ��ܡ�

---

## 7. ������ɾ����ȫ����д��������

### 7.1 `Remove(Func<T, Boolean> predicate)`

- ���������񣺶� `_cache` ִ�� `RemoveAll`��
- ����
  - `FindAll()` ����ȫ����
  - ���˵������
  - �� `Write(list, false)` ����д�ء�

### 7.2 `Update(T model)` / `Set(T model)`

- `Update`��ֻ���£��������򷵻� `false`��
- `Set`����������£���������׷��һ����

δ��������ʱ��

- ��ȡȫ�����ڴ棬�޸ĺ󸲸�д�ء�

---

## 8. �첽��ѯ��net5+ / netstandard2.1+��

�� `NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER` ���ṩ��

- `IAsyncEnumerable<T> QueryAsync(Func<T, Boolean>? predicate, Int32 count = -1)`
- `Task<IList<T>> FindAllAsync()`

ʵ��Ҫ�㣺

- �ڲ�ʹ�� `CsvFile.ReadAllAsync()`��
- ͷ��ӳ���߼���ͬ����һ�£�
- �����쳣ʱͬ����¼�������С�

---

## 9. ��Сʾ��

### 9.1 ����ʵ��

```csharp
public class User
{
    public Int32 Id { get; set; }
    public String? Name { get; set; }
    public DateTime CreateTime { get; set; }
}
```

### 9.2 ׷��д��

```csharp
using NewLife.IO;

var db = new CsvDb<User>
{
    FileName = "./user.csv",
    Encoding = Encoding.UTF8,
};

db.Add(new User { Id = 1, Name = "Stone", CreateTime = DateTime.Now });
db.Add(new User { Id = 2, Name = "NewLife", CreateTime = DateTime.Now });
```

### 9.3 ��ѯ

```csharp
foreach (var u in db.Query(e => e.Id > 0))
{
    Console.WriteLine($"{u.Id} {u.Name}");
}
```

### 9.4 ����������

```csharp
using var db = new CsvDb<User> { FileName = "./user.csv" };

db.BeginTransaction();

db.Add(new User { Id = 3, Name = "Tx" });
db.Remove(e => e.Id == 1);

db.Commit();
```

---

## 10. ע�����������ʵ��

1. **��Ƶд�������� `Add`��������**������׷��д·��������ȫ����д��
2. **�޸�/ɾ����һ�֡�������������**������ `BeginTransaction()` ���д������� `Commit()`��
3. **���߳�ʹ��**����ʹ�ڲ��� `lock (this)`��Ҳ��������̲߳�������ͬһ��ʵ����
4. **��ͷ������������**��д��ʹ�� `SerialHelper.GetName`����ȡ�ǰ�����ӳ�䵽���ԣ������Զ������������л����ԣ���Ҫȷ��д��/��ȡһ�¡�

---

## 11. �������

- 历史文档已归档，当前请以仓库内 Doc 为准
- Դ�룺`DH.NCore/IO/CsvDb.cs`
- ������`DH.NCore/IO/CsvFile.cs`
