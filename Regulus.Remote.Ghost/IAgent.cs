using Regulus.Network;
using System;

namespace Regulus.Remote.Ghost
{
    public interface IAgent : INotifierQueryable , IDisposable
    {                
        bool Active { get; }
     
        float Ping { get; }

        event Action<string, string> ErrorMethodEvent;

        void Update();
    }
}