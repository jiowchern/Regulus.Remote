using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter.Formula
{
    class User : Regulus.Game.IUser
    {
        IStorage _Storage;
        Regulus.Utility.StageMachine _Machine;
        Regulus.Remoting.ISoulBinder _Binder;
        Data.Account _Account;
        public User(Regulus.Remoting.ISoulBinder binder)
        {
            _Storage = new DummyStorage();
            _Machine = new Regulus.Utility.StageMachine();
            _Binder = binder;
        }
        void Regulus.Game.IUser.OnKick(Guid id)
        {
            if (_Account != null && _Account.Id == id)
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

        private void _ToVerify()
        {
            _Account = null;
            var verify = new VGame.Project.FishHunter.Verify(_Storage);
            var stage = new VGame.Project.FishHunter.Stage.Verify(_Binder, verify);
            stage.DoneEvent += _VerifySuccess;
            _Machine.Push(stage);
        }

        private void _VerifySuccess(Data.Account account)
        {
            if(account.IsGameServer())
            {
                _Account = account;
                _VerifySuccessEvent(_Account.Id);
                _ToFishStage();
            }
            else
            {
                _ToVerify();
            }
        }

        private void _ToFishStage()
        {            
            var stage = new VGame.Project.FishHunter.Stage.QueryFishStage(_Binder);
            stage.DoneEvent += _ToFormula;
            _Machine.Push(stage);
        }

        private void _ToFormula(long account , int stage_id ,HitBase formula)
        {
            var stage = new VGame.Project.FishHunter.Stage.Formula(_Binder, account, stage_id, formula);
            stage.DoneEvent += _ToFishStage;
            _Machine.Push(stage);
        }

        

        void Regulus.Framework.ILaunched.Shutdown()
        {
            _Machine.Termination();
        }
    }
}
