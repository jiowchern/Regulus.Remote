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
1. The maximum amount of event parameters is 5.  
If there are more than 5, refactor to one class.  
2. Delegates only support System.Action.
