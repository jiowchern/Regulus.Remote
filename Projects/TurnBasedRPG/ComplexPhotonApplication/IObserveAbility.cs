using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
    interface IObserveAbility
    {
        void Update(Regulus.Project.TurnBasedRPG.Map.EntityInfomation[] observeds, List<IObservedAbility> lefts);
        Regulus.CustomType.Vector2 Position { get; }
        Regulus.CustomType.Rect Vision { get; }
        event Action<IObservedAbility> IntoEvent;
        event Action<IObservedAbility> LeftEvent;
	}
}
