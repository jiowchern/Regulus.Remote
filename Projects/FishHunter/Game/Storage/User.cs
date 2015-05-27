using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter.Storage
{
    class User : Regulus.Game.IUser
    {


        Regulus.Utility.StageMachine _Machine;
        private Regulus.Remoting.ISoulBinder _Binder;
        IStorage _Storage;
        public User(Regulus.Remoting.ISoulBinder binder, IStorage storage)
        {
            _Storage = storage;
            this._Binder = binder;
            _Machine = new Regulus.Utility.StageMachine();
        }
        Data.Account _Account;
        void Regulus.Game.IUser.OnKick(Guid id)
        {
            
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

        void Regulus.Framework.IBootable.Launch()
        {
            _ToVerify();
        }

        void Regulus.Framework.IBootable.Shutdown()
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

        private void _VerifySuccess(Data.Account account)
        {
            _VerifySuccessEvent(account.Id);
            _Account = account;
            _ToRelease(account);
        }

        private void _ToRelease(Data.Account account)
        {
            var stage = new VGame.Project.FishHunter.Stage.StroageAccess(_Binder, account , _Storage);
            stage.DoneEvent += _ToVerify;
            _Machine.Push(stage);
        }


        
    }
}
