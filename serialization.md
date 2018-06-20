### 序列化
#### [位置](https://github.com/jiowchern/Regulus/tree/master/Library/Regulus.Serialization)
#### [測試](https://github.com/jiowchern/Regulus/tree/master/Test/Regulus.SerializationTests)

#### 使用說明1
```csharp
[NUnit.Framework.Test()]
public void TestSerializer1()
{
    // 填入序列化的類型
    var types = new[] {typeof(int), typeof(int[]), typeof(float), typeof(string), typeof(char), typeof(char[])};
    var ser = new Serializer(new DescriberBuilder(types));

    var intZeroBuffer = ser.ObjectToBuffer("123");
    var intZero = ser.BufferToObject(intZeroBuffer);
    
    Assert.AreEqual("123" , intZero);
}
```
這個方法需要填入要序列化的類型，雖然有使用上不便的情形但是卻能達到最高壓縮效率。

#### 使用說明2
```csharp
[NUnit.Framework.Test()]
public void TestSerializerStringArray()
{
    var ser = new Regulus.Serialization.Dynamic.Serializer();

    var buf = ser.ObjectToBuffer(new[] { "1", "2", "3", "4", "5" });
    var val = (string[])ser.BufferToObject(buf);
    Assert.AreEqual("1", val[0]);
    Assert.AreEqual("2", val[1]);
    Assert.AreEqual("3", val[2]);
    Assert.AreEqual("4", val[3]);
    Assert.AreEqual("5", val[4]);
}
```
省略掉使用者自行填入序列化類型，但是相對序列化出來的資料比第一個方法大。