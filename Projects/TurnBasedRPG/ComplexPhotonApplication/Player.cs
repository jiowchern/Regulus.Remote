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
            _DBActorInfomation.Property.Position.X = x;
            _DBActorInfomation.Property.Position.Y = y;
        }

        void IPlayer.SetVision(int vision)
        {
            _DBActorInfomation.Property.Vision = vision;
        }
        
        void IPlayer.Stop()
        {
            var mover = FindAbility<IMoverAbility>();
            if (mover != null)
            {

                mover.Act(ActionStatue.idle, 0,Direction );
            }
        }
        void IPlayer.Walk(float direction)
        {
            var mover = FindAbility<IMoverAbility>();
            if (mover != null)
            {
                mover.Act(ActionStatue.run, _DBActorInfomation.Property.Speed, direction);
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
    }
}
