# Communications 
### Remote Event
Broadcast the event to the client.
```csharp
public interface IFoo
{
    event System.Action<string> BroadcastEvent;
}
```
---
#### Restrictions

Delegates only support System.Action<...>.
