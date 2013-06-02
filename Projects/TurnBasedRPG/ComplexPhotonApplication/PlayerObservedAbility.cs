using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
    class PlayerObservedAbility : IObservedAbility
    {
        Regulus.Project.TurnBasedRPG.Serializable.DBEntityInfomation _Infomation;
        public PlayerObservedAbility(Regulus.Project.TurnBasedRPG.Serializable.DBEntityInfomation info )
        {
            _Infomation = info;
        }
        Guid IObservedAbility.Id
        {
            get { return _Infomation.Property.Id ; }
        }

        Types.Vector2 IObservedAbility.Position
        {
            get { return _Infomation.Property.Position; }
        }
    }
}
