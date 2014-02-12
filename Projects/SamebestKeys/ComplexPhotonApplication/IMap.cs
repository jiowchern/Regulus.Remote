using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    interface IMap
    {
        void Into(Entity entity);
        void Left(Entity entity);
    }
}
