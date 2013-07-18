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
            : base(dB_actorInfomation.Property, dB_actorInfomation.Look)
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

        protected override void _SetAbility(Entity.AbilitySet abilitys)
        {
            _ObservedAbility = new PlayerObservedAbility(this, _DBActorInfomation);            
            abilitys.AttechAbility<IObservedAbility>(_ObservedAbility);

            _ObserveAbility = new PlayerObserveAbility(_ObservedAbility, _DBActorInfomation);
            abilitys.AttechAbility<IObserveAbility>(_ObserveAbility);

            base._SetAbility(abilitys);
        }
        protected override void _RiseAbility(Entity.AbilitySet abilitys)
        {
            abilitys.DetechAbility<IObserveAbility>();
            abilitys.DetechAbility<IObservedAbility>();

            base._RiseAbility(abilitys);
        }

        public event Action ReadyEvent;		
        void IPlayer.Ready()
        {
            if (ReadyEvent != null)
                ReadyEvent();
        }

        void IPlayer.SetPosition(float x, float y)
        {
            base.SetPosition(x , y);
            
        }

        void IPlayer.SetVision(int vision)
        {
            _DBActorInfomation.Property.Vision = vision;
        }
        
        void IPlayer.Stop(float dir)
        {
            var mover = FindAbility<IMoverAbility>();
            if (mover != null)
            {
                mover.Act(ActionStatue.Idle, 0, dir);
            }
        }
        void IPlayer.Walk(float direction)
        {
            var mover = FindAbility<IMoverAbility>();
            if (mover != null)
            {
                mover.Act(ActionStatue.Run, _DBActorInfomation.Property.Speed, direction);
            }
        }

        void IPlayer.BodyMovements(ActionStatue action_statue)
        {
            var mover = FindAbility<IMoverAbility>();
            if (mover != null)
            {
                mover.Act(action_statue, 0, 0);
            }
        }


        Guid IPlayer.Id
        {
            get { return _DBActorInfomation.Property.Id; }
        }

        float IPlayer.Speed
        {
            get { return _DBActorInfomation.Property.Speed; }
        }


        

        void IPlayer.SetSpeed(float speed)
        {
            _DBActorInfomation.Property.Speed = speed;
        }
        

        void IPlayer.Say(string message)
        {
            _ObservedAbility.Say(message);
        }


        string IPlayer.Name
        {
            get { return _DBActorInfomation.Look.Name; }
        }

        float IPlayer.Direction
        {
            get { return _DBActorInfomation.Property.Direction; }
        }

		public string Map
		{
			get { return _DBActorInfomation.Property.Map; }
		}
		string IPlayer.Map
		{
			get { return _DBActorInfomation.Property.Map; }
		}
	}
}
