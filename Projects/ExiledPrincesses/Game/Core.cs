using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Game
{
    public class Core : Regulus.Game.IFramework, IUserStatus
	{
		Regulus.Remoting.ISoulBinder _Binder;
		public IStorage	Storage {get ; private set;}
        IZone _Zone;
        
        

		public Regulus.Remoting.ISoulBinder Binder { get { return _Binder; }}
		Regulus.Game.StageMachine _StageMachine;
        public Core(Regulus.Remoting.ISoulBinder binder, IStorage storage, IZone zone )
		{

            _Zone = zone; 
			Storage = storage;
			_Binder = binder;
            _Binder.Bind<IUserStatus>(this);
			_StageMachine = new Regulus.Game.StageMachine();

			binder.BreakEvent += _OnInactive;
            _StatusEvent += (s) => { };
		}
		~Core()
		{
			_Binder.BreakEvent -= _OnInactive;
		}
		void _OnInactive()
		{
			if (InactiveEvent != null)
				InactiveEvent();			
		}

		public void Launch()
		{
			
		}

        private void _ToVerify()
        {
            var stage = new Regulus.Project.ExiledPrincesses.Game.Stage.Verify(this);
            _StageMachine.Push(stage);
            stage.LoginSuccessEvent += _ToAdventure;
            
            _StatusEvent(UserStatus.Verify);
        }

        private void _ToAdventure(AccountInfomation account_infomation)
        {
            if (account_infomation.Record == null)
            {
                account_infomation.Record = new GameRecord() { Map = "Teaching" };
                account_infomation.Record.Actors = new ActorInfomation[] { new ActorInfomation() { Prototype = 1, Id = Guid.NewGuid(), Exp = 0, Level = 1 } };
                var contingent = new Regulus.Project.ExiledPrincesses.Contingent();
                contingent.Formation = Regulus.Project.ExiledPrincesses.Contingent.FormationType.Auxiliary;
                contingent.Members = new Guid[] { account_infomation.Record.Actors[0].Id };
                account_infomation.Record.Contingent = contingent;
                
                
            }
            
        }

        AccountInfomation _AccountInfomation;
        void _ToParking(AccountInfomation account_infomation)
        {
            
            var stage = new Regulus.Project.ExiledPrincesses.Game.Stage.Parking(Binder , account_infomation);
            
            stage.VerifyEvent += _ToVerify;
            _StageMachine.Push(stage);
            _AccountInfomation = account_infomation;
            
            _StatusEvent(UserStatus.Pub);
        }
        

		public bool Update()
		{
			_StageMachine.Update();
			return true;
		}
		public void Shutdown()
		{
			_StageMachine.Termination();
		}

		public event Action InactiveEvent;

        event Action<UserStatus> _StatusEvent;
        event Action<UserStatus> IUserStatus.StatusEvent
        {
            add { _StatusEvent += value; }
            remove { _StatusEvent -= value; }
        }

        void IUserStatus.Ready()
        {
            _ToVerify();
        }
    }
}
