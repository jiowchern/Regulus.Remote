using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
	/// <summary>
	/// 玩家觀察功能
	/// </summary>
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

        internal void Speak(float[] voice_stream)
        {
            if (_Speakvent != null)
                _Speakvent(voice_stream);
        }

        event Action<float[]> _Speakvent;
        event Action<float[]> IObservedAbility.SpeakEvent
        {
            add { _Speakvent += value; }
            remove { _Speakvent -= value; }
        }


        string IObservedAbility.Name
        {
            get { return _Infomation.Look.Name; }
        }


        int IObservedAbility.Shell
        {
            get { return _Infomation.Look.Shell; }
        }

        ActorMode IObservedAbility.Mode
        {
            get { return _Actor.Mode; }
        }







        
    }
}
