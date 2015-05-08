using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter.Play
{
    class User : Regulus.Game.IUser
    {
        Regulus.Utility.StageMachine _Machine;
        Regulus.Remoting.ISoulBinder _Binder;
        VGame.Project.FishHunter.IAccountFinder _AccountFinder;
        VGame.Project.FishHunter.IFishStageQueryer _FishStageQueryer;
        Data.Account _Account;
        Data.Record _Record;
        public User(Regulus.Remoting.ISoulBinder binder , 
            VGame.Project.FishHunter.IAccountFinder account_finder,
            VGame.Project.FishHunter.IFishStageQueryer queryer)
        {
            _AccountFinder = account_finder;
            _Binder = binder;
            _Machine = new Regulus.Utility.StageMachine();
            _FishStageQueryer = queryer;

            _Record = new Data.Record();
            _Record.Money = 1000;
        }
        void Regulus.Game.IUser.OnKick(Guid id)
        {
            if(_Account != null && _Account.Id == id)
            {
                _QuitEvent();
            }
        }

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

        bool Regulus.Utility.IUpdatable.Update()
        {
            _Machine.Update();
            return true;
        }

        void Regulus.Framework.ILaunched.Launch()
        {

            _ToVerify();
        }

        void Regulus.Framework.ILaunched.Shutdown()
        {
            _Machine.Termination();
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

            _ToSelectStage();

        }

        private void _ToSelectStage()
        {
            var stage = new VGame.Project.FishHunter.Play.SelectStage(_Binder, _FishStageQueryer);
            stage.DoneEvent += _ToPlayStage;
            _Machine.Push(stage);            
        }

        private void _ToPlayStage(IFishStage fish_stage)
        {
            var stage = new VGame.Project.FishHunter.Play.PlayStage(_Binder, fish_stage, _Record.Money);
            stage.DoneEvent += _Save;
            _Machine.Push(stage);            
        }

        private void _Save(int money)
        {
            _Record.Money = money;

            _ToSelectStage();
        }

        
    }
}
