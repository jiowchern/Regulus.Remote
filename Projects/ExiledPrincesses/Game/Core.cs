using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Game
{
    public partial class Core : Regulus.Game.IFramework, IUserStatus, IController
	{
		Regulus.Remoting.ISoulBinder _Binder;
		public IStorage	Storage {get ; private set;}
        IZone _Zone;
		public Regulus.Remoting.ISoulBinder Binder { get { return _Binder; }}
		Regulus.Game.StageMachine _StageMachine;
        AccountInfomation _AccountInfomation;
        
        public Core(Regulus.Remoting.ISoulBinder binder, IStorage storage, IZone zone )
		{
            _Zone = zone; 
			Storage = storage;
			_Binder = binder;
            
			_StageMachine = new Regulus.Game.StageMachine();

			binder.BreakEvent += _OnInactive;
            _StatusEvent += (s) => { };
            
            
		}
		~Core()
		{
			_Binder.BreakEvent -= _OnInactive;
		}
        public void OnKick(Guid account)
        {
            if (_AccountInfomation != null && _AccountInfomation.Id == account)
                _OnInactive();
        }

		void _OnInactive()
		{
			if (InactiveEvent != null)
				InactiveEvent();			
		}

		public void Launch()
		{
            _InitialController(_Binder);
            _Binder.Bind<Regulus.Remoting.ITime>( LocalTime.Instance );
            _Binder.Bind<IUserStatus>(this);

		}

        private void _ToVerify()
        {            
            var stage = new Regulus.Project.ExiledPrincesses.Game.Stage.Verify(this);
            stage.LoginSuccessEvent += _ToFirst;
            _StageMachine.Push(stage);
            _StatusEvent(UserStatus.Verify);
        }

        public event OnNewUser VerifySuccessEvent;
        private void _ToFirst(AccountInfomation account_infomation)
        {
            _AccountInfomation = account_infomation;
            if (VerifySuccessEvent != null)
                VerifySuccessEvent(_AccountInfomation.Id);
            if (account_infomation.Record == null)
            {
                GameRecord record = _BuildFirstRecord();
                Adventurer adv = _BuildAdventurer("Teaching" , record);

                _ToFirstAdventure(record, adv);
            }
            else
            {
                _ToTown(account_infomation.Record.Tone);
            }
            
        }

        private void _ToFirstAdventure(GameRecord record, Adventurer adv)
        {
            _StatusEvent(UserStatus.Adventure);
            var stage = new Regulus.Project.ExiledPrincesses.Game.Stage.Adventure(adv, _Binder, _Zone);
            stage.ToToneEvent += (tone) =>
            {
                _AccountInfomation.Record = record;
                _ToTown(tone);
            };
            _StageMachine.Push(stage);
        }
        
        private Adventurer _BuildAdventurer(string map ,GameRecord record)
        {
            ActorInfomation[] actors = record.GetContingentActors();

            var teammates = (from actor in actors select new Teammate(actor)).ToArray();
            var adv = new Adventurer();
            adv.Map = map;
            adv.Teammates = teammates;
            adv.Formation = Contingent.FormationType.Auxiliary;
            adv.Controller = this;
            return adv;
        }

        private GameRecord _BuildFirstRecord()
        {
            var record = new GameRecord();
            record.Actors = new ActorInfomation[] { new ActorInfomation() { Prototype = 1, Id = Guid.NewGuid(), Exp = 0} };
            var contingent = new Regulus.Project.ExiledPrincesses.Contingent();
            contingent.Formation = Regulus.Project.ExiledPrincesses.Contingent.FormationType.Auxiliary;
            contingent.Members = new Guid[] { record.Actors[0].Id };
            record.Contingent = contingent;
            return record;
        }

        private void _ToAdventure(Adventurer adventurer)
        {
            _StatusEvent(UserStatus.Adventure);
            var stage = new Regulus.Project.ExiledPrincesses.Game.Stage.Adventure(adventurer , _Binder , _Zone);
            stage.ToToneEvent += _ToTown ;
            _StageMachine.Push(stage);
        }

        private void _ToTown(string name)
        {
            _StatusEvent(UserStatus.Town);
            TownPrototype prototype = Regulus.Project.ExiledPrincesses.Game.Stage.TownResource.Instance.Find(name);
            if (prototype != null)
            {
                var stage = new Regulus.Project.ExiledPrincesses.Game.Stage.Town(name , prototype, _Binder);
                stage.ToLevelsEvent += _ToLevels;
                _StageMachine.Push(stage);
            }
            else
            {
                _AccountInfomation.Record = null;
                _ToFirst(_AccountInfomation);
            }
        }

        void _ToLevels(string name)
        {
            _ToAdventure( _BuildAdventurer(name, _AccountInfomation.Record ));
        }
        

		public bool Update()
		{
			_StageMachine.Update();
			return true;
		}
		public void Shutdown()
		{
            _Binder.Unbind<IUserStatus>(this);
            _Binder.Unbind<Regulus.Remoting.ITime>(LocalTime.Instance);
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

        Remoting.Value<long> IUserStatus.QueryTime()
        {
            return LocalTime.Instance.Ticks;
        }


        void _InitialController(Regulus.Remoting.ISoulBinder binder)
        {
            _AdventureIdleBinder = new OnesBinder<IAdventureIdle>(binder);
            _AdventureGoBinder = new OnesBinder<IAdventureGo>(binder);
            _AdventureChoiceBinder = new OnesBinder<IAdventureChoice>(binder);
            _Actor = new PluralBinder<IActor>(binder , new IActor[0]);
            _Enemy = new PluralBinder<IActor>(binder, new IActor[0]);
            _Team = new PluralBinder<ITeam>(binder, new ITeam[0]);
            _CombatController = new PluralBinder<ICombatController>(binder, new ICombatController[0]);
        }


        OnesBinder<IAdventureIdle> _AdventureIdleBinder;
        void IController.SetIdleController(IAdventureIdle adventure_idle)
        {
            _AdventureIdleBinder.Set(adventure_idle);
        }

        OnesBinder<IAdventureGo> _AdventureGoBinder;        
        void IController.SetGoController(IAdventureGo adventure_go)
        {
            _AdventureGoBinder.Set(adventure_go);
        }

        OnesBinder<IAdventureChoice> _AdventureChoiceBinder;       
        void IController.SetChoiceController(IAdventureChoice adventure_choice)
        {
            _AdventureChoiceBinder.Set(adventure_choice);
        }

        PluralBinder<IActor> _Actor;        
        void IController.SetComrades(IActor[] actors)
        {
            _Actor.Differences(actors);
        }
        PluralBinder<ITeam> _Team;
        void IController.SetTeamss(ITeam[] teams)
        {
            _Team.Differences(teams);
        }

        PluralBinder<IActor> _Enemy;        
        void IController.SetEnemys(IActor[] actors)
        {
            _Enemy.Differences(actors);
        }

        PluralBinder<ICombatController> _CombatController;                
        void IController.SetCombatController(ICombatController[] controllers)
        {
            _CombatController.Differences(controllers);
        }

        void IController.BattleBegins()
        {
            _BattleBegin();
        }

        void IController.BattleEnd()
        {
            _EndBegin();
        }


        event Action _BattleBegin;
        event Action IUserStatus.BattleBegin
        {
            add { _BattleBegin += value; }
            remove { _BattleBegin -= value; }
        }
        event Action _EndBegin;
        event Action IUserStatus.EndBegin
        {
            add { _EndBegin += value; }
            remove { _EndBegin -= value; }
        }
    }
}
