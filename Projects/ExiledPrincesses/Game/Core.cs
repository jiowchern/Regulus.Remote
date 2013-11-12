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
        IMap _Zone;
        Battle.IZone _Battle;
        

		public Regulus.Remoting.ISoulBinder Binder { get { return _Binder; }}
		Regulus.Game.StageMachine _StageMachine;
        public Core(Regulus.Remoting.ISoulBinder binder, IStorage storage, IMap zone , Battle.IZone battle)
		{
            _Battle = battle;
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
            stage.LoginSuccessEvent += _ToParking;
            
            _StatusEvent(UserStatus.Verify);
        }

        AccountInfomation _AccountInfomation;
        void _ToParking(AccountInfomation account_infomation)
        {
            var stage = new Regulus.Project.ExiledPrincesses.Game.Stage.Parking(Binder , account_infomation);
            stage.SelectCompiledEvent += _ToAdventure;
            stage.VerifyEvent += _ToVerify;
            _StageMachine.Push(stage);
            _AccountInfomation = account_infomation;
            
            _StatusEvent(UserStatus.Parking);
        }

        ActorInfomation _ActorInfomation;
        void _ToAdventure(ActorInfomation actor_infomation)
        {

            var stage = new Regulus.Project.ExiledPrincesses.Game.Stage.Adventure(actor_infomation , Binder, _Zone);
            stage.ToBattleStageEvent += _ToBattle;
            stage.ParkingEvent += () => { _ToParking(_AccountInfomation); };
            _StageMachine.Push(stage);

            _ActorInfomation = actor_infomation;
            _StatusEvent(UserStatus.Adventure);
        }

        void _ToBattle(IBattleAdmissionTickets battle_admission_tickets)
        {
            var stage = new Regulus.Project.ExiledPrincesses.Game.Stage.Battle(battle_admission_tickets, _ActorInfomation, Binder, Storage);
            stage.EndEvent += () =>
            {
                _ToAdventure(_ActorInfomation);
            };
            _StageMachine.Push(stage);
            _StatusEvent(UserStatus.Battle);
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
