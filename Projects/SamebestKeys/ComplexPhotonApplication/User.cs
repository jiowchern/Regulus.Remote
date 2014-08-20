using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace Regulus.Project.SamebestKeys
{
    internal class User : Regulus.Utility.IUpdatable
    {        
        public Remoting.ISoulBinder Provider { get; private set; }

        Regulus.Game.StageMachine _Machine ;
        public event OnNewUser VerifySuccessEvent;
        
		IWorld _World;
        IStorage _Storage;
        public User(Remoting.ISoulBinder provider, IWorld world , IStorage storage)
        {
            _Storage = storage;

            _Machine = new Regulus.Game.StageMachine();
            Provider = provider;
            provider.BreakEvent += Quit;
			_World = world;

            
        }

        Serializable.AccountInfomation _AccountInfomation;
        internal void OnLoginSuccess(Serializable.AccountInfomation obj)
        {            
            VerifySuccessEvent(obj.Id);
            _AccountInfomation = obj;
            _Storage.CreateConsumptionPlayer(_AccountInfomation.Id);
            ToParking();
        }

        public void ToParking()
        {
            if (_AccountInfomation != null)
            {
                Actor = null;
                _Machine.Push(new ParkingStage(_Storage , this));
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
        
        internal void ToLogout()
        {
            _AccountInfomation = null;
            _ClearActor(); 
            _Machine.Push(new VerifyStage(_Storage ,this)); 
        }
        public Serializable.DBEntityInfomation Actor { get; private set; }
        

        private void _InitialActor(Serializable.DBEntityInfomation obj)
        {
            Actor = obj;
            if (Actor.Property.Id == Guid.Empty)
            {
                Actor.Property.Id = Guid.NewGuid();
            }
            if (Actor.Property.Vision == 0.0)
            {
                Actor.Property.Vision = 100;
            }
            if (Actor.Property.Speed == 0.0)
            {
                Actor.Property.Speed = 5;
            }
            if (!(Actor.Property.Position.X >= 0 && Actor.Property.Position.X <= 100))
            {
                //Actor.Property.Position = Types.Vector2.FromPoint(Regulus.Utility.Random.Instance.R.Next(0, 100), Actor.Property.Position.Y);
            }
            if (!(Actor.Property.Position.Y >= 0 && Actor.Property.Position.Y <= 100))
            {
                //Actor.Property.Position = Types.Vector2.FromPoint(Actor.Property.Position.X, Regulus.Utility.Random.Instance.R.Next(0, 100));
            }
            if (!(Actor.Look.Shell > 0 && Actor.Look.Shell < 14))
            {
                Actor.Look.Shell = Regulus.Utility.Random.Next(1, 13);
            }
            if (Actor.Property.Skills.Count == 0)
            {
                Actor.Property.Skills.Add(new Serializable.Skill() { Id = 1 });
                Actor.Property.Skills.Add(new Serializable.Skill() { Id = 2 });
            }

            if (Actor.Record == null)
            {
                Actor.Record = new Serializable.Record();                
            }
        }
       

        void Regulus.Framework.ILaunched.Launch()
        {
            _AccountInfomation = null;
            _ClearActor();            
            _Machine.Push(new VerifyStage(_Storage,this)); 
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

        internal void OnKick(Guid account)
        {
            if (_AccountInfomation != null && _AccountInfomation.Id == account)
                ToLogout();
        }

        internal void ToFirst(Serializable.DBEntityInfomation obj)
        {
            _InitialActor(obj);            
            _JumpRealm("Room");
        }        
        private void _ToRealm(Regulus.Project.SamebestKeys.Dungeons.IScene realm)        
        {
            if (realm != null)
            {
                var stage = new Regulus.Project.SamebestKeys.Dungeons.RealmStage(Provider, realm, new[] { Actor } , new Belongings(_Storage, _AccountInfomation.Id));
                stage.ExitWorldEvent += ToParking;
                stage.LogoutEvent += ToLogout;
                stage.ChangeRealmEvent += _JumpRealm;
                stage.QuitEvent += ToIdle;
                _Machine.Push(stage);
            }
            else
            {
                ToParking();
            }
        }

        private void ToIdle()
        {
            var stage = new Regulus.Project.SamebestKeys.Dungeons.IdleStage(Provider);
            stage.GotoRealmEvent += _JumpRealm;
            _Machine.Push(stage);         
        }        

        private void _JumpRealm(string realm)
        {
            var result = _World.Query(realm);
            result.OnValue += _ToRealm;
        }
    }
}
