using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
	/// <summary>
	/// 玩家自己
	/// </summary>
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
        ICrossAbility _CrossAbility;

		/// <summary>
		/// 設定功能
		/// </summary>
		/// <param name="abilitys">現有功能Dict</param>
        protected override void _SetAbility(Entity.AbilitySet abilitys)
        {
            _ObservedAbility = new PlayerObservedAbility(this, _DBActorInfomation);            
            abilitys.AttechAbility<IObservedAbility>(_ObservedAbility);

            _ObserveAbility = new PlayerObserveAbility( _DBActorInfomation);
            abilitys.AttechAbility<IObserveAbility>(_ObserveAbility);

            _CrossAbility = new CrossAbility();
            abilitys.AttechAbility<ICrossAbility>(_CrossAbility);

            //_CrossAbility.MoveEvent += _CrossAbility_MoveEvent;
            
            base._SetAbility(abilitys);
        }

        /*public event Action<string, Types.Vector2, string, Types.Vector2> CrossEvent;		
        void _CrossAbility_MoveEvent(string target_map, Types.Vector2 target_position)
        {
            if (CrossEvent != null)
                CrossEvent(target_map, target_position, _DBActorInfomation.Property.Map, _DBActorInfomation.Property.Position );
            CrossEvent = null;
        }*/

		/// <summary>
		/// 移除功能
		/// </summary>
		/// <param name="abilitys">現有功能Dict</param>
        protected override void _RiseAbility(Entity.AbilitySet abilitys)
        {
            abilitys.DetechAbility<IObserveAbility>();
            abilitys.DetechAbility<IObservedAbility>();
            abilitys.DetechAbility<ICrossAbility>();

            base._RiseAbility(abilitys);
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
            var commander = FindAbility<IBehaviorCommandAbility>();
            if (commander != null)
            {
                commander.Invoke(new BehaviorCommand.Stop());
            }
        }
        void IPlayer.Walk(float direction)
        {

            var commander = FindAbility<IBehaviorCommandAbility>();
            if (commander != null)
            {
                commander.Invoke(new BehaviorCommand.Move(direction));
            }
            
        }

        void IPlayer.BodyMovements(ActionStatue action_statue)
        {
            var mover = FindAbility<IMoverAbility>();
            if (mover != null)
            {
                mover.Act(new Serializable.ActionCommand() { Command = action_statue, Turn = true });
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

		
        


        void IPlayer.Goto(string map, float x, float y)
        {
            _CrossAbility.Move(map, new Types.Vector2(x, y));
        }


        void IPlayer.ChangeMode()
        {
            var commander = FindAbility<IBehaviorCommandAbility>();
            if (commander != null)
            {
                commander.Invoke(new BehaviorCommand.Skill(1));
            }            
        }


        void IPlayer.Cast(int skill)
        {
            var commander = FindAbility<IBehaviorCommandAbility>();
            if (commander != null)
            {
                commander.Invoke(new BehaviorCommand.Skill(skill));
            }            
        }



        internal void ClearField()
        {
            _ObserveAbility.Clear();
        }

        internal string[] QueryPlayableScenes()
        {
            var results = ( from dataScene in GameData.Instance.Scenes
                            join clearanceScene in _DBActorInfomation.Record.Clearances 
                            on dataScene.Front equals clearanceScene 
                            select dataScene.Name).ToArray();

            return results;    
        }
    }
}
