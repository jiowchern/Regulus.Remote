using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    interface IActorPropertyAbility
    {
        void Injury(int damage);

        object Direction { get; set; }

        float NormalSpeed { get; set; }

        float CurrentSpeed { get; set; }
    }
}
