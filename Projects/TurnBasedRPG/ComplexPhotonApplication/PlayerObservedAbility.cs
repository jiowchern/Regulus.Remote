﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
    class PlayerObservedAbility : IObservedAbility
    {
        Regulus.Project.TurnBasedRPG.Serializable.DBEntityInfomation _Infomation;
        Actor _Actor;
        public PlayerObservedAbility(Actor actor,Regulus.Project.TurnBasedRPG.Serializable.DBEntityInfomation info )
        {
            _Actor = actor;
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

        event Action<Serializable.MoveInfomation> IObservedAbility.ShowActionEvent
        {
            add { _Actor.ShowActionEvent += value; }
            remove { _Actor.ShowActionEvent -= value; ; }
        }
    }
}
