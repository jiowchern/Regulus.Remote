using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
    class Player : Actor , Common.IPlayer  
    {
        private Serializable.DBEntityInfomation _DBActorInfomation;

        public Player(Serializable.DBEntityInfomation dB_actorInfomation)
            : base(dB_actorInfomation)
        {            
            _DBActorInfomation = dB_actorInfomation;
        }

        private void _LeftEntity(Entity obj)
        {
            if (_LeftEvent != null)
                _LeftEvent(obj.Id);
        }

        private void _IntoEntity(Entity obj)
        {
            if (_IntoEvent != null)
            {
                Serializable.EntityInfomation info = new Serializable.EntityInfomation();
                info.Id = obj.Id;
                info.Position = obj.Position;
                _IntoEvent(info);
            }
            
        }

        public event Action LogoutEvent;
        void Common.IPlayer.Logout()
        {
            if (LogoutEvent != null)
            {
                LogoutEvent();
            }
        }

        public event Action ExitWorldEvent;
        void Common.IPlayer.ExitWorld()
        {
            if (ExitWorldEvent != null)
            {
                ExitWorldEvent();
            }
        }

        Samebest.Remoting.Value<int> Common.IPlayer.SetData(int i)
        {
            _DBActorInfomation.TestData = ++i;
            return _DBActorInfomation.TestData;
        }

        Samebest.Remoting.Value<int> Common.IPlayer.GetData()
        {
            return _DBActorInfomation.TestData;
        }

        event Action<Serializable.EntityInfomation> _IntoEvent;
        event Action<Serializable.EntityInfomation> Common.IPlayer.IntoEvent
        {
            add { _IntoEvent += value; }
            remove { _IntoEvent -= value; }
        }

        event Action<Guid> _LeftEvent;
        event Action<Guid> Common.IPlayer.LeftEvent
        {
            add { _LeftEvent += value; }
            remove { _LeftEvent -= value; }
        }

        internal void Initialize()
        {
            Field.IntoEvent += _IntoEntity;
            Field.LeftEvent += _LeftEntity;
        }

        internal void Finialize()
        {
            Field.IntoEvent -= _IntoEntity;
            Field.LeftEvent -= _LeftEntity;
        }
    }
}
