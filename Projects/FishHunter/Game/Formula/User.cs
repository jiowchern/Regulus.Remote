using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter.Formula
{
    class User : Regulus.Game.IUser
    {
        
        Regulus.Utility.StageMachine _Machine;
        Regulus.Remoting.ISoulBinder _Binder;
        Data.Account _Account;
        private User(Regulus.Remoting.ISoulBinder binder)
        {            
            _Machine = new Regulus.Utility.StageMachine();
            _Binder = binder;
        }

        public User(Regulus.Remoting.ISoulBinder binder, StorageController _Controller) : this(binder)
        {                        
            this._Controller = _Controller;
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
        private Regulus.Remoting.ISoulBinder binder;
        private StorageController _Controller;
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
            var verify = new VGame.Project.FishHunter.Verify(_Controller.AccountFinder);
            var stage = new VGame.Project.FishHunter.Stage.Verify(_Binder, verify);
            stage.DoneEvent += _VerifySuccess;
            _Machine.Push(stage);
        }

        private void _VerifySuccess(Data.Account account)
        {
            if(account.IsFormulaQueryer())
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
