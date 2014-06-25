using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    interface IMap
    {
        Guid Id { get; }
        void Into(Entity entity);
        void Left(Entity entity);

        IMapInfomation GetInfomation();

        string Name { get; }

        event Action<Guid> LeftDoneEvent;
    }
}
