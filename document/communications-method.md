# Communications 
### Remote Method Invention
Defines a method to give a client a call.    
```csharp
public interface IFoo
{
    Regulus.Remote.Value<string> Method(string arg1);
}
```
The server implements the method.  
```csharp
namesapce Server
{
    class Foo : IFoo
    {
        Regulus.Remote.Value<string> IFoo.Method(string arg1)
        {
            return $"Response:{arg1}";
        }
    }    
}
```
---
#### Restrictions
1. The maximum amount of method parameters is 5.  
If there are more than 5, refactor to one class.  
2. Return values only support void or Regulus.Remote.Value.