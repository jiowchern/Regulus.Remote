using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
    class Player : Actor , IPlayer  
    {

        private Serializable.DBEntityInfomation _DBActorInfomation;

        public Player(Serializable.DBEntityInfomation dB_actorInfomation)
            : base(dB_actorInfomation.Property.Id)
        {            
            _DBActorInfomation = dB_actorInfomation;
        }

        public event Action LogoutEvent;
        void IPlayer.Logout()
        {
            if (LogoutEvent != null)
            {
                LogoutEvent();
            }
        }

        public event Action ExitWorldEvent;
        void IPlayer.ExitWorld()
        {
            if (ExitWorldEvent != null)
            {
                ExitWorldEvent();
            }
        }

        PlayerObserveAbility _ObserveAbility;
        PlayerObservedAbility _ObservedAbility;
        internal void Initialize()
        {
            _ObservedAbility = new PlayerObservedAbility(_DBActorInfomation);
            _AttechAbility<IObservedAbility>(_ObservedAbility);

            _ObserveAbility = new PlayerObserveAbility(_ObservedAbility, _DBActorInfomation);
            _AttechAbility<IObserveAbility>(_ObserveAbility);


			_MoverAbility = new PlayerMoverAbility(_DBActorInfomation);
        }

        internal void Finialize()
        {
            _DetechAbility<IObserveAbility>();
            _DetechAbility<IObservedAbility>();
        }

        public event Action ReadyEvent;
		private PlayerMoverAbility _MoverAbility;
        void IPlayer.Ready()
        {
            if (ReadyEvent != null)
                ReadyEvent();
        }

        void IPlayer.SetPosition(float x, float y)
        {
            _DBActorInfomation.Property.Position.X = x;
            _DBActorInfomation.Property.Position.Y = y;
        }

        void IPlayer.SetVision(int vision)
        {
            _DBActorInfomation.Property.Vision = vision;
        }
		
	}
}
