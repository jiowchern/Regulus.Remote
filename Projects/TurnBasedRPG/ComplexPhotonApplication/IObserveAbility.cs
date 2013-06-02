using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
    interface IObserveAbility
    {
        void Update(IObservedAbility[] observeds);
        IObservedAbility Observed  { get; }
        float Vision { get; }
        event Action<IObservedAbility> IntoEvent;
        event Action<IObservedAbility> LeftEvent;
    }
}
