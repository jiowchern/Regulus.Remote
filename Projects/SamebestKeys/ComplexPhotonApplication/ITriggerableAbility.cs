using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    interface ITriggerableAbility
    {
        Types.Rect Bounds { get;  }

        void Interactive(long time, List<Map.EntityInfomation> inbrounds);
    }
}
