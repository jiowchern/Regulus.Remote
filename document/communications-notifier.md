# Communications 
### Interface Notifier.
**The interface can broadcast its child interface to the client.**

Define the property```Regulus.Remote.INotifier<T>```.  
```csharp
public interface IFoo
{
    Regulus.Remote.INotifier<IBar> IBars {get;}
}
```
Implement foo and notifier sample.
```csharp
class Foo : IFoo ,Regulus.Remote.INotifier<IBar>
{
    readonly List<IBar> _Bars;
    public Foo()
    {
        _Bars = new List<IBar>();
    }
    // IFoo's implementations
    Regulus.Remote.INotifier<IBar> IFoo.IBars => this;
    // Regulus.Remote.INotifier<IBar>'s implementations
    event Action<T> _SupplyEvent ;
    event Action<T> Regulus.Remote.INotifier<IBar>.Supply 
    {
        add {_SupplyEvent += value;}    
        remove {_SupplyEvent -= value;}    
    }
    event Action<T> _UnsupplyEvent ;
    event Action<T> Regulus.Remote.INotifier<IBar>.Supply 
    {
        add {_UnsupplyEvent += value;}    
        remove {_UnsupplyEvent -= value;}    
    }
    IBar[] Ghosts => _Bars.ToArray();
    // Foo's implementations.
    public void Add(IBar bar)
    {
        _Bars.Add(bar);
        _SupplyEvent(bar);
    }
    public void Remove(IBar bar)
    {
        _Bars.Remove(bar);
        _UnsupplyEvent(bar);
    }
    public void Dispose()
    {
        foreach(var bar in _Bars)
        {
            _UnsupplyEvent(bar);
        }
        _Bars.Clear();
    }
}
```
Calling ```Foo.Add``` will pass ```IBar``` to the client, whereas calling ```Foo.Remove``` will cancel the client object.

---
#### Restrictions
1. Notifier supports only interfaces.
2. When ```Foo``` is to be released, it is recommended that ```IBar``` be cancelled by calling ```Unsupply```.
