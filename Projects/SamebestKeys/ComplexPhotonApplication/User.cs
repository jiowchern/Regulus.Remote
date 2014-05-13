using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace Regulus.Project.SamebestKeys
{
    internal class User : Regulus.Utility.IUpdatable
    {
        public Remoting.ISoulBinder Provider { get; private set; }

        Regulus.Game.StageMachine<User> _Machine ;
        public event OnNewUser VerifySuccessEvent;
        
		IWorld _World;
        IStorage _Storage;
        public User(Remoting.ISoulBinder provider, IWorld world , IStorage storage)
        {
            _Storage = storage;

            _Machine = new Regulus.Game.StageMachine<User>(this);
            Provider = provider;
            provider.BreakEvent += Quit;
			_World = world;
        }

        Serializable.AccountInfomation _AccountInfomation;
        internal void OnLoginSuccess(Serializable.AccountInfomation obj)
        {
            _AccountInfomation = obj;
            VerifySuccessEvent(obj.Id);            
            ToParking();
        }

        public void ToParking()
        {
            if (_AccountInfomation != null)
            {
                Actor = null;
                _Machine.Push(new ParkingStage(_Storage));
            }
        }
        
        public Guid Id 
        { 
            get 
            {
                if (_AccountInfomation != null)
                    return _AccountInfomation.Id;
                return Guid.Empty;
            } 
        }
        
        internal void Logout()
        {
            _AccountInfomation = null;
            _ClearActor();            
            _Machine.Push(new VerifyStage(_Storage)); 
        }
        public Serializable.DBEntityInfomation Actor { get; private set; }
        internal void EnterWorld(Serializable.DBEntityInfomation obj , string level)
        {
            Actor = obj;
            if (Actor.Property.Id == Guid.Empty)
            {
                Actor.Property.Id = Guid.NewGuid();
            }
            if (Actor.Property.Vision == 0.0)
            {
                Actor.Property.Vision = 30;
            }
            if (Actor.Property.Speed == 0.0)
            {
                Actor.Property.Speed = 10;
            }
            if ( !(Actor.Property.Position.X >= 0 && Actor.Property.Position.X <= 100))
            {
                //Actor.Property.Position = Types.Vector2.FromPoint(Regulus.Utility.Random.Instance.R.Next(0, 100), Actor.Property.Position.Y);
            }
            if (!(Actor.Property.Position.Y >= 0 && Actor.Property.Position.Y <= 100))
            {                
                //Actor.Property.Position = Types.Vector2.FromPoint(Actor.Property.Position.X, Regulus.Utility.Random.Instance.R.Next(0, 100));
            }
            if ( !(Actor.Look.Shell > 0 && Actor.Look.Shell <14))
            {
                Actor.Look.Shell = Regulus.Utility.Random.Next(1 , 13);
            }
            if (Actor.Property.Skills.Count == 0)
            {
                Actor.Property.Skills.Add(new Serializable.Skill() { Id = 1 });
                Actor.Property.Skills.Add(new Serializable.Skill() { Id = 2 });
            }
            

            ToCross(level, Actor.Property.Position, "Test", new Types.Vector2(50, 50));
			
        }

        private void _ToAdventure(string map_string)
        {
            var mapVal = _World.Create(map_string);
            mapVal.OnValue += (map) =>
            {
                _Machine.Push(new AdventureStage(map, _Storage));
            };
        }

        
        void _ToAdventure(string map , Types.Vector2 position)
        {
            if (Actor != null)
            {
                Actor.Property.Map = map;
                Actor.Property.Position = position;
                _ToAdventure(Actor.Property.Map);
            }            
        }

        void Regulus.Framework.ILaunched.Launch()
        {
            _AccountInfomation = null;
            _ClearActor();            
            _Machine.Push(new VerifyStage(_Storage)); 
        }
        
        private void _ClearActor()
        {
            if (Actor != null)
            {
                _Storage.SaveActor(Actor);
                Actor = null;
            }
        }

		bool Regulus.Utility.IUpdatable.Update()
        {
            return _Machine.Update();
        }

		void Regulus.Framework.ILaunched.Shutdown()
        {
            _ClearActor();
            _Machine.Termination();    
        }

        public event Action QuitEvent;
        internal void Quit()
        {
            
            if (QuitEvent != null)
            {
                QuitEvent();
                QuitEvent = null;
            }
        }

        internal void ToCross(string target_map, Types.Vector2 target_position, string current_map, Types.Vector2 current_position)
        {
            var stage = new CrossStage(Provider , _World, target_map, target_position, current_map, current_position);
            stage.ResultEvent += _ToAdventure;
            _Machine.Push(stage);             
        }



        internal void OnKick(Guid account)
        {
            if (_AccountInfomation != null && _AccountInfomation.Id == account)
                Quit();
        }

        

        
    }
}
