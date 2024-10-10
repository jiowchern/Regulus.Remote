using Regulus.Network;
using System;

namespace Regulus.Remote.Ghost
{
    public interface IAgent : INotifierQueryable 
    {                
        bool Active { get; }
     
        float Ping { get; }

        event Action<string, string> ErrorMethodEvent;

        void Update();
        void Enable(IStreamable streamable);
        void Disable();
    }
}