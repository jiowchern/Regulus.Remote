using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    internal interface ITriggerableAbility
    {
        Types.Rect Bounds { get;  }

        void Interactive(long time, List<Map.EntityInfomation> inbrounds);
    }
}
