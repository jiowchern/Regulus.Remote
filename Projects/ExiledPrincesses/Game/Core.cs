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
        AccountInfomation _AccountInfomation;
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
            stage.LoginSuccessEvent += _ToFirst;
            
            _StatusEvent(UserStatus.Verify);
        }

        private void _ToFirst(AccountInfomation account_infomation)
        {
            _AccountInfomation = account_infomation;
            if (account_infomation.Record == null)
            {
                GameRecord record = _BuildFirstRecord();
                Adventurer adv = _BuildAdventurer("Teaching" , record);

                _ToFirstAdventure(record, adv);
            }
            else
            {
                _ToTone(account_infomation.Record.Tone);
            }
            
        }

        private void _ToFirstAdventure(GameRecord record, Adventurer adv)
        {
            var stage = new Regulus.Project.ExiledPrincesses.Game.Stage.Adventure(adv, _Binder, _Zone);
            stage.ToToneEvent += (tone) =>
            {
                _AccountInfomation.Record = record;
                _ToTone(tone);
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
            return adv;
        }

        private GameRecord _BuildFirstRecord()
        {
            var record = new GameRecord();
            record.Actors = new ActorInfomation[] { new ActorInfomation() { Prototype = 1, Id = Guid.NewGuid(), Exp = 0, Level = 1 } };
            var contingent = new Regulus.Project.ExiledPrincesses.Contingent();
            contingent.Formation = Regulus.Project.ExiledPrincesses.Contingent.FormationType.Auxiliary;
            contingent.Members = new Guid[] { record.Actors[0].Id };
            record.Contingent = contingent;
            return record;
        }

        private void _ToAdventure(Adventurer adventurer)
        {
            var stage = new Regulus.Project.ExiledPrincesses.Game.Stage.Adventure(adventurer , _Binder , _Zone);
            stage.ToToneEvent += _ToTone ;
            _StageMachine.Push(stage);
        }

        private void _ToTone(string name)
        {
            var stage = new Regulus.Project.ExiledPrincesses.Game.Stage.Tone();
            stage.ToMapEvent += _ToMap;
            _StageMachine.Push(stage);
        }

        void _ToMap(string name)
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
