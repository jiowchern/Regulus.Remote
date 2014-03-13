using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    class PlayerObservedAbility : IObservedAbility
    {
        Regulus.Project.SamebestKeys.Serializable.DBEntityInfomation _Infomation;
        Actor _Actor;
        
        public PlayerObservedAbility(Actor actor,Regulus.Project.SamebestKeys.Serializable.DBEntityInfomation info )
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

        float IObservedAbility.Direction
        {
            get { return _Infomation.Property.Direction; }
        }


        internal void Say(string message)
        {
            if (_SayEvent != null)
                _SayEvent(message);
        }

        event Action<string> _SayEvent;
        event Action<string> IObservedAbility.SayEvent
        {
            add { _SayEvent += value; }
            remove { _SayEvent -= value; }
        }

        string IObservedAbility.Name
        {
            get { return _Infomation.Look.Name; }
        }
    }
}
