# Regulus Serialization

## 介紹
這是一個簡易的序化工具, 可支援的類型有  ```short, ushort, int, uint, bool, logn, ulong, float, decimal, double, char, byte, enum, string, System.Guid``` 跟其陣列類型.


## 使用

**定義**
```csharp
public struct ClassA
{
    public int Field1;
    public string Field2;
}
```

**靜態方式**
```csharp
var classA = new ClassA();
classA.Field1 = 1;
classA.Field2 = "2";

// 建立時需要加入需要的類型
var serializer = new Regulus.Serialization.Serializer(new Regulus.Serialization.DescriberBuilder(typeof(ClassA)).Describers);

// 序列化
var buffer = serializer.ObjectToBuffer(classA);

// 反序列化
var cloneClassA = serializer.ObjectToBuffer(buffer) as ClassA;

```



**動態方式**
```csharp
var classA = new ClassA();
classA.Field1 = 1;
classA.Field2 = "2";

var serializer = new Regulus.Serialization.Dynamic.Serializer();

// 序列化
var buffer = serializer.ObjectToBuffer(classA);

// 反序列化
var cloneClassA = serializer.ObjectToBuffer(buffer) as ClassA;

```