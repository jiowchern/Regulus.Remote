using System;
using System.Collections.Generic;
using System.Reflection;

namespace Regulus.BehaviourTree
{
    public interface ITicker
    {

        Guid Id { get; }
        string Tag { get; }
        void GetPath(ref List<Guid> nodes);

        ITicker[] GetChilds();

        void Reset();
        TICKRESULT Tick(float delta);
    }

    
}