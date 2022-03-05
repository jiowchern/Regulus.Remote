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
You can accept the return value on the client side by the following methods.  
```csharp
namesapce Client
{
    class Bar 
    {
        void async OnFoo(IFoo foo)
        {
            // Method 1
            // Using OnValue ,receive the value returned from the server.
            var val = foo.Method("test arg.");
            val.OnValue += (ret) =>
            {
                // ret is Response:"test arg."
            };

            // Method 2
            // Using await ,receive the value returned from the server.
            var ret = await foo.Method("test arg.");
            // ret is Response:"test arg."            
        }
    }    
}
```

---
#### Restrictions
Return values only support void or Regulus.Remote.Value.