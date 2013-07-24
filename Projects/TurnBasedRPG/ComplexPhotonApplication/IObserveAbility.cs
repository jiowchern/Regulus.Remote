using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
    interface IObserveAbility
    {
        void Update(PhysicalAbility[] observeds, List<IObservedAbility> lefts);
        Regulus.Types.Vector2 Position { get; }
        System.Windows.Rect Vision { get; }
        event Action<IObservedAbility> IntoEvent;
        event Action<IObservedAbility> LeftEvent;
	}
}
