# Regulus Serialization

## Introduction
This is a simple ordinalization tool that supports types ```short, ushort, int, uint, bool, logn, ulong, float, decimal, double, char, byte, enum, string, System.Guid``` and its array types.


## Use

**Define**
```csharp
public struct ClassA
{
    public int Field1;
    public string Field2;
}
```

**Static**
```csharp
var classA = new ClassA();
classA.Field1 = 1;
classA.Field2 = "2";

// need to add the required type when creating.
var serializer = new Regulus.Serialization.Serializer(new Regulus.Serialization.DescriberBuilder(typeof(ClassA)).Describers);

// Serialization
var buffer = serializer.ObjectToBuffer(classA);

// Deserialization
var cloneClassA = serializer.ObjectToBuffer(buffer) as ClassA;

```



**Dynamic**
```csharp
var classA = new ClassA();
classA.Field1 = 1;
classA.Field2 = "2";

var serializer = new Regulus.Serialization.Dynamic.Serializer();

// Serialization
var buffer = serializer.ObjectToBuffer(classA);

// Deserialization
var cloneClassA = serializer.ObjectToBuffer(buffer) as ClassA;

```