using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter.Play
{
    class User : Regulus.Game.IUser, IAccountStatus
    {
        Regulus.Utility.StageMachine _Machine;
        
        Regulus.Remoting.ISoulBinder _Binder;
        IAccountFinder _AccountFinder;
        IFishStageQueryer _FishStageQueryer;
        IRecordQueriers _RecordQueriers;
        ITradeNotes _TradeAccount;
        
        Data.Account _Account;
        Data.Record _Record;
        
        

        event Regulus.Game.OnNewUser _VerifySuccessEvent;
        event Regulus.Game.OnNewUser Regulus.Game.IUser.VerifySuccessEvent
        {
            add { _VerifySuccessEvent += value; }
            remove { _VerifySuccessEvent -= value; }
        }

        event Regulus.Game.OnQuit _QuitEvent;
        event Regulus.Game.OnQuit Regulus.Game.IUser.QuitEvent
        {
            add { _QuitEvent += value; }
            remove { _QuitEvent -= value; }
        }

        event Action _KickEvent;
        event Action IAccountStatus.KickEvent
        {
            add { _KickEvent += value; }
            remove { _KickEvent -= value; }
        }

        StageTicketInspector _StageTicketInspector;
        
        public User(Regulus.Remoting.ISoulBinder binder , 
            IAccountFinder account_finder,
            IFishStageQueryer queryer,
            IRecordQueriers record_queriers,
            ITradeNotes trade_account)
        {
            _Machine = new Regulus.Utility.StageMachine();

            _Binder = binder;
            _AccountFinder = account_finder;
            _FishStageQueryer = queryer;
            var locks = new Data.StageLock[] { new Data.StageLock { KillCount = 200 , Stage = 3 } };
            _StageTicketInspector = new StageTicketInspector(new VGame.Project.FishHunter.Play.StageGate(locks));
            
            _RecordQueriers = record_queriers;
            
            _TradeAccount = trade_account;        
        }

        void Regulus.Game.IUser.OnKick(Guid id)
        {
            if(_Account != null && _Account.Id == id)
            {
                if (_KickEvent != null)
                    _KickEvent();
                _ToVerify();
            }
        }

        bool Regulus.Utility.IUpdatable.Update()
        {
            _Machine.Update();
            return true;
        }

        void Regulus.Framework.IBootable.Launch()
        {
            _Binder.BreakEvent += _Quit ;
            _Binder.Bind<IAccountStatus>(this);
            _ToVerify();
        }

        private void _Quit()
        {            
            _QuitEvent();
        }

        void Regulus.Framework.IBootable.Shutdown()
        {
            _SaveRecord();
            _Binder.Unbind<IAccountStatus>(this);
            _Machine.Termination();
            _Binder.BreakEvent -= _Quit;
        }

        private void _SaveRecord()
        {
            if (_Record != null)
                _RecordQueriers.Save(_Record);
        }

        private void _ToVerify()
        {
            var verify = _CreateVerify();
            _AddVerifyToStage(verify);
        }

        private Verify _CreateVerify()
        {
            _Account = null;
            var verify = new VGame.Project.FishHunter.Verify(_AccountFinder);
            return verify;
        }

        private void _AddVerifyToStage(Verify verify)
        {
            var stage = new VGame.Project.FishHunter.Stage.Verify(_Binder, verify);
            stage.DoneEvent += _VerifySuccess;
            _Machine.Push(stage);
        }

        private void _VerifySuccess(Data.Account account)
        {
            _VerifySuccessEvent(account.Id);
            _Account = account;
            _ToQueryRecord();
        }

        

        private void _ToQueryRecord()
        {
            _RecordQueriers.Load(_Account.Id).OnValue += (obj)=>
            {
                _Record = obj;
                _StageTicketInspector.Initial(new Data.Stage[] { new Data.Stage { Id = 1, Pass = true }, new Data.Stage { Id = 2, Pass = false } });
                _ToLoadTradeNotes();
            };
        }

        private void _ToLoadTradeNotes()
        {
            _TradeAccount.GetTotalMoney(_Account.Id).OnValue +=(money) =>
            {
                _Record.Money += money;
                _ToSelectStage();
            };
        }

        private void _ToSelectStage()
        {
            var stage = new VGame.Project.FishHunter.Play.SelectStage(_StageTicketInspector.PlayableStages , _Binder, _FishStageQueryer);
            stage.DoneEvent += _ToPlayStage;
            _Machine.Push(stage);            
        }

        private void _ToPlayStage(IFishStage fish_stage)
        {
            var stage = new VGame.Project.FishHunter.Play.PlayStage(_Binder, fish_stage, _Record);
            stage.PassEvent += _Pass;
            stage.KillEvent += _Kill;
            _Machine.Push(stage);            
        }

        private void _Kill(int kill_count)
        {
            _StageTicketInspector.Kill(kill_count);
            _ToSelectStage();
        }        

        private void _Pass(int pass_stage)
        {
            _StageTicketInspector.Pass(pass_stage);
            _ToSelectStage();
        }
    }
}
