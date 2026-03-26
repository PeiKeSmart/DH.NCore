# Snowflake ѩ���㷨ʹ���ֲ�

���ĵ�����Դ�� `DH.NCore/Data/Snowflake.cs` ����� `XUnitTest.Core/Data/SnowflakeTests.cs`������˵�� `Snowflake`��ѩ���㷨�ֲ�ʽ Id ������������ơ��÷���ע�����

> �ؼ��ʣ�������WorkerId��ʱ��������кš�ʱ��ز�����Ⱥ���䣨Redis����

---

## 1. ����

`Snowflake` ��һ�� 64 bit �� `Int64` ��Ϊȫ��Ψһ Id��

λ���䣺

- 1 bit������������λ��
- 41 bit��ʱ��������룩
- 10 bit�������ڵ㣨`WorkerId`��0~1023��
- 12 bit�����кţ�0~4095��

���ɵ� Id �߱������ص㣺

- ����������������ʱ���ƽ���
- ͬһ�����ڿ�������� 4096 �� Id
- **��ͬһ�� `Snowflake` ʵ����**�ɱ�֤���ظ�

��ҪԼ����

- **ҵ���ڱ���ȷ������**���������������ڶ�� `Snowflake` ʵ�������� `WorkerId` ��ͬ������ܲ����ظ� Id��

---

## 2. ���ĸ���

### 2.1 `StartTimestamp`����ʼʱ�����

- ���ԣ�`public DateTime StartTimestamp { get; set; }`
- Ĭ��ֵ��`UTC 1970-01-01` תΪ����ʱ�䣨`DateTimeKind.Local`��

����Ҫ�㣺

- `Snowflake` ��Ѳ�������ʱ��ת���� `StartTimestamp` ����ʱ����Ȼ������õ���������
- Ĭ��ʹ�ñ���ʱ�䣬��Ϊ�˷������ѩ�� Id ʱֱ�ӵõ�����ʱ�䣬������������ҵ�������ǰ��������ڷֱ�/�����ĳ�������

ʹ�ý��飺

- `StartTimestamp` **�������״ε��� `NewId()` ǰ����**���״����ɺ����޸Ĳ���Ӱ���ѳ�ʼ��ʵ������Ϊ��ʼ������ֻ��һ�Σ���

### 2.2 `WorkerId`�������ڵ� Id��

- ���ԣ�`public Int32 WorkerId { get; set; }`
- ��Χ��0~1023��10 λ��

˵����

- �ڷֲ�ʽϵͳ�ڣ�`WorkerId` ��ȫ��Ψһ�Ծ����˿�ڵ��Ƿ������ظ� Id��
- ������Ĭ���㷨��IP/����/�̣߳�**�޷����Ա�֤Ψһ**����Ҫ�󳡾������ⲿ��ʽ���䡣

### 2.3 `Sequence`�����кţ�

- ���ԣ�`public Int32 Sequence => _sequence;`
- ��Χ��0~4095��12 λ��

˵����

- ͬһ������ͨ���������кű�֤Ψһ��
- ������������� 4095��ʱ���㷨���߼����ƽ�����һ����������ɡ�

---

## 3. WorkerId ��ʼ�����ȼ�

`Snowflake` ���״����� Id ʱ���Զ�ִ��һ�γ�ʼ����`Initialize()`���������ȼ����� `WorkerId`��

1. ���ʵ�� `WorkerId > 0`��ʹ��ʵ��ֵ
2. ������� `Snowflake.GlobalWorkerId > 0`��ʹ�� `GlobalWorkerId & 1023`
3. ������� `Snowflake.Cluster != null`������ `JoinCluster(Cluster)` �Ӽ�Ⱥ����
4. ����ʹ��Ĭ���㷨���ɣ����� IP ������ʵ���� + ����/�̣߳�

ע�⣺

- ������ʹ�õ����ж� `WorkerId <= 0`����� **`WorkerId=0` �ᱻ��Ϊ��δ���á��������ߺ�������**������ϣ���̶�Ϊ 0����Ҫ����ȷ����Ҫ������ʼ�����ǣ�һ�㲻���飩��

---

## 4. API �ٲ�

### 4.1 `Int64 NewId()`

���ڵ�ǰʱ��������һ�� Id��

��ΪҪ�㣺

- ʹ�� `DateTime.Now`������ʱ�䣩����ͨ�� `ConvertKind` ת���� `StartTimestamp` ��ʱ����
- ����ʱ��ز���
  - ����⵽ʱ��ز��һز����ȴ��� `MaxClockBack`��Լ 1 Сʱ + 10 �룩���׳� `InvalidOperationException`
  - ����ʹ����һ��ʱ����������ɣ����ֵ�ʵ��Ψһ�ԣ�

���ó�����

- ����ҵ���������ɣ���ã���

### 4.2 `Int64 NewId(DateTime time)`

����ָ��ʱ������ Id��Я����ǰʵ���� `WorkerId` �����кţ���

ע�⣺

- ����Ϊͬһ����ָ������ʱ�䡱���ɳ��� 4096 �� Id��������ظ�����Ϊ���к�ֻ�� 12 λ����ȡģ����

���ó�����

- ��Ҫ��ҵ��ʱ�乹����� Id�����簴�ɼ�ʱ����⣩��

### 4.3 `Int64 NewId(DateTime time, Int32 uid)`

����ָ��ʱ������ Id��ʹ�� `uid` �ĵ� 10 λ��Ϊ `WorkerId`��1024 ���飩���Ա��� 12 λ���кš�

���ó�����

- ���������ݲɼ���ÿ 1024 ��������Ϊһ�飬ÿ��ÿ������������ 4096 �� Id��

ע�⣺

- ��ͬһ����ͬһ�������ɳ��� 4096 �� Id��������ظ���

### 4.4 `Int64 NewId22(DateTime time, Int32 uid)`

����ָ��ʱ������ Id��ʹ�� 22 λҵ�� Id��`uid & ((1<<22)-1)`�������ٱ������кš�

���ó�����

- ���������ݲɼ���ÿ 4,194,304 ��������һ�飬ÿ��ÿ������� 1 �� Id��
- ��������� upsert��ͬһ����ͬһ������д��������ʱֻ����һ�С�

ע�⣺

- ͬһҵ�� id ��ͬһ���������ɶ�� Id ���ظ�����Ϊû�����кţ���

### 4.5 `Int64 GetId(DateTime time)`

��ʱ��ת��Ϊ��������ʱ�䲿�֡��� Id������ `WorkerId` �����кţ���

���ó�����

- ����ʱ��Ƭ�β�ѯ����ʱ������ת��Ϊѩ�� Id ������з�Χ��ѯ��

### 4.6 `Boolean TryParse(Int64 id, out DateTime time, out Int32 workerId, out Int32 sequence)`

����ѩ�� Id���õ�ʱ�䡢`WorkerId`�����кš�

˵����

- �����õ��� `time` �� `StartTimestamp` ����ʱ����ʱ�䡣

### 4.7 `DateTime ConvertKind(DateTime time)`

������ʱ��ת��Ϊ�� `StartTimestamp` ��ͬ��ʱ�������������

����

- `time.Kind == DateTimeKind.Unspecified`��ֱ�ӷ��أ�����ת����
- `StartTimestamp.Kind == Utc`������ `time.ToUniversalTime()`
- `StartTimestamp.Kind == Local`������ `time.ToLocalTime()`

---

## 5. ��Ⱥģʽ��ȷ�� WorkerId ����Ψһ

### 5.1 `static ICache? Cluster`

`Snowflake.Cluster` ��������һ������ʵ����Ϊ WorkerId ������������ʹ�� Redis��

- �� `Cluster != null` ��ʵ�� `WorkerId` δ��ʽ����ʱ����ʼ���׶λ���� `JoinCluster(Cluster)`��

### 5.2 `void JoinCluster(ICache cache, String key = "SnowflakeWorkerId")`

ͨ���������Ӽ�Ⱥ��ȡ WorkerId��

- `workerId = (Int32)cache.Increment(key, 1)`
- `WorkerId = workerId & 1023`

ʹ�ý��飺

- �ڶ����/��ڵ㳡�����������Ȳ��ü�Ⱥ���� WorkerId��
- ��Ҫ���ֻ�����dev/test/prod��ʱ������Ϊ��ͬ����ʹ�ò�ͬ�� `key`������ WorkerId ���佻�档

---

## 6. ��ȷʹ������

### 6.1 ��Ӧ���ڱ��ֵ���

�ؼ��㣺

- ÿ��Ӧ��/�����ڣ�����ֻ����һ��ȫ�� `Snowflake` ʵ����
- ��ʹ�� ORM/�м�������� XCode������ȷ��ͬһ�ű�����ͬһҵ���򣩲�Ҫ���� new һ�� `Snowflake`��

ʾ����

```csharp
using NewLife.Data;

public static class IdGenerator
{
    public static readonly Snowflake Instance = new()
    {
        // ������Ҫ�����״ε��� NewId ǰ����
        // StartTimestamp = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Local),
        // WorkerId = 1,
    };
}

var id = IdGenerator.Instance.NewId();
```

### 6.2 ��ʽָ�� WorkerId���Ƽ���

```csharp
var snow = new Snowflake { WorkerId = 1 };
var id = snow.NewId();
```

### 6.3 ʹ�ü�Ⱥ���� WorkerId���Ƽ����ֲ�ʽ������

```csharp
using NewLife.Caching;
using NewLife.Data;

Snowflake.Cluster = /* RedisCache ʵ�� */;

var snow = new Snowflake();
var id = snow.NewId();
```

---

## 7. �����������

### 7.1 Ϊʲôǿ������������

`Snowflake` ֻ��֤����ʵ�������ɵ� Id Ψһ��

- ֻҪ���ֶ��ʵ�������� `WorkerId` һ���������¾Ϳ��ܲ����ظ���

### 7.2 Ĭ�� WorkerId �Ƿ�ɿ���

Ĭ�ϲ���ʹ�� IP ����ʵ�� Id + ����/�߳���Ϣ��Ŀ���ǽ���ͬ����ͻ���ʣ����޷������л����¾���Ψһ��

- ����������NAT��ͬһ������ IP �仯�����������ȶ����������ͻ��
- ��Ҫ�󳡾�ǿ�ҽ��飺��ʽ WorkerId ��ʹ�� Redis �������䡣

### 7.3 ʱ��ز��ᷢ��ʲô��

- С��Χ�ز����������ϴ�ʱ����������ɣ����ֵ�ʵ�����ظ���
- �ز������׳��쳣�ܾ����ɣ���ֹ�������ײ����

### 7.4 ʹ�� `NewId(DateTime time)` �Ƿ�һ��Ψһ��

��һ����

- ��ͬһ�����ڣ����к�ֻ�� 12 λ��4096����������ȡģ�����ظ����ա�
- ���� API ���ʺϡ���ʱ�����/��������ҵ�����󣬶����Ǹ߲���д��ͬһ��ҵ��ʱ��㡣

---

## 8. ������˵��

- `Snowflake` ���ڻ����⣬���� `net45` ~ `net10` ��Ŀ���ܡ�
- �㷨���Ļ��� `DateTime`��`Interlocked`��`Volatile`��`ICache` ��ͨ�� API��

---

## 9. �������

- 历史文档已归档，当前请以仓库内 Doc 为准
- Դ�룺`DH.NCore/Data/Snowflake.cs`
- ��Ԫ���ԣ�`XUnitTest.Core/Data/SnowflakeTests.cs`
