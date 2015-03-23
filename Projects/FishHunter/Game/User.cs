using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter
{
    class User : Regulus.Game.IUser
    {
        Regulus.Utility.StageMachine _Machine;
        Regulus.Remoting.ISoulBinder _Binder;
        VGame.Project.FishHunter.IStorage _Storage;
        public User(Regulus.Remoting.ISoulBinder binder , VGame.Project.FishHunter.IStorage storage)
        {
            _Storage = storage;
            _Binder = binder;
            _Machine = new Regulus.Utility.StageMachine();
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
            var verify = new VGame.Project.FishHunter.Verify(_Storage);
            return verify;
        }

        private void _AddVerifyToStage(Verify verify)
        {
            var stage = new VGame.Project.FishHunter.Stage.Verify(_Binder, verify);
            stage.DoneEvent += _VerifySuccess;
            _Machine.Push(stage);
        }

        Data.Account _Account;
        private void _VerifySuccess(Data.Account account)
        {
            _VerifySuccessEvent(account.Id);
            _Account = account;            
        }
    }
}
