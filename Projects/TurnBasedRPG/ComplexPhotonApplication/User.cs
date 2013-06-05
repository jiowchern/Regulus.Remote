using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Project.TurnBasedRPG
{
    class User : Samebest.Game.IFramework
    {
        public  Samebest.Remoting.Soul.SoulProvider Provider {get ; private set;}

        Samebest.Game.StageMachine<User> _Machine ;
        Regulus.Project.TurnBasedRPG.UserRoster _UserRoster;
        public User(Samebest.Remoting.Soul.SoulProvider provider , Regulus.Project.TurnBasedRPG.UserRoster user_roster)
        {
            _UserRoster = user_roster;
            _Machine = new Samebest.Game.StageMachine<User>(this);
            Provider = provider;
            provider.BreakEvent += Quit;
        }

        Serializable.AccountInfomation _AccountInfomation;
        internal void OnLoginSuccess(Serializable.AccountInfomation obj)
        {
            _AccountInfomation = obj;
            ToParking();
        }

        public void ToParking()
        {
            if (_AccountInfomation != null)
            {
                Actor = null;
                _Machine.Push(new ParkingStage());
            }
        }


        public string Name
        {
            get
            {
                if (_AccountInfomation != null)
                    return _AccountInfomation.Name;
                return null;
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
            _Machine.Push(new VerifyStage(_UserRoster)); 
        }
        public Serializable.DBEntityInfomation Actor { get; private set; }
        internal void EnterWorld(Serializable.DBEntityInfomation obj)
        {
            Actor = obj;
            if (Actor.Property.Id == Guid.Empty)
            {
                Actor.Property.Id = Guid.NewGuid();
            }
            if (Actor.Property.Speed == 0.0)            
            {
                Actor.Property.Speed = 1;
            }
            if ( !(Actor.Property.Position.X >= 0 && Actor.Property.Position.X <= 100))
            {
                Actor.Property.Position.X = Regulus.Utility.Random.Instance.R.Next(0, 100);
            }
            if (!(Actor.Property.Position.Y >= 0 && Actor.Property.Position.Y <= 100))
            {
                Actor.Property.Position.Y = Regulus.Utility.Random.Instance.R.Next(0, 100);
            }
            _Machine.Push(new AdventureStage());
        }

        void Samebest.Game.IFramework.Launch()
        {
            _AccountInfomation = null;
            _ClearActor();            
            _Machine.Push(new VerifyStage(_UserRoster)); 
        }

        private void _ClearActor()
        {
            if (Actor != null)
            {
                Samebest.Utility.Singleton<Storage>.Instance.SaveActor(Actor);
                Actor = null;
            }
        }

        bool Samebest.Game.IFramework.Update()
        {
            return _Machine.Update();
        }

        void Samebest.Game.IFramework.Shutdown()
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
    }
}
