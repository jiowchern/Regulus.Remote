# Communications 
### Property synchronization.
If an object needs to be synchronized with the client...  
```csharp
public interface IFoo
{
    Regulus.Remote.Property<int> Number{get;}
}
```
The server implements the property.  
```csharp
namesapce Server
{
    class Foo : IFoo
    {
        public Foo()
        {
            _Number = new Regulus.Remote.Property<int>();
            _Number.Value = 1;
        }
        Regulus.Remote.Property<int> IFoo.Number => _Number;        
    }    
}
```
---
#### Restrictions
1. Can only be serializable structures or classes. Interfaces are not supported.  
There is no support because the interface has client communication permissions to consider.  
2.  Get method only supported.
