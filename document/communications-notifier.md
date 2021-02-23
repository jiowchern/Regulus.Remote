# Communications 
### Interface Notifier.
**The interface can broadcast its child interface to the client.**

Define the property```Regulus.Remote.Notifier<T>```.  
```csharp
public interface IFoo
{
    Regulus.Remote.Notifier<IBar> BarNotifier {get;}
}
```
The server implements the property.  
```csharp
namespace Server
{
    public interface IBar
    {
    }

    class Bar : IBar
    {    
    }

    class Foo : IFoo
    {
        readonly NotifiableCollection<IBar> _Bars;
        readonly Notifier<IBar> _BarNotifier;

        public Foo()
        {
            _Bars = new NotifiableCollection<IBar>();
            _BarNotifier = new Regulus.Remote.Notifier<IBar>(_Bars);            
        }

        Regulus.Remote.Notifier<IBar> IFoo.BarNotifier => _BarNotifier;        

        public void AddBar(Bar bar)
        {
            _Bars.Items.Add(bar);            
        }

        public void RemoveBar(Bar bar)
        {
            _Bars.Items.Remove(bar);            
        }    

        public void Dispose()
        {
            _Bars.Items.Clear();
            _BarNotifier.Dispose();
        }            
    }    
}
```
In the client.
```csharp
namespace Client
{
    class Program
    {
        void _OnFoo(IFoo foo)
        {
            foo.BarNotifier.Base.Supply += _OnBar;
        }
        void _OnBar(IBar bar)   
        {
            
        }
    }
}
```


---
#### Restrictions
1. Notifier supports only interfaces.
2. No duplicate instances may be added.