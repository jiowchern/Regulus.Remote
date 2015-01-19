using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imdgame.RunLocusts
{
    class User : Regulus.Game.IUser
    {
        private Regulus.Remoting.ISoulBinder _Binder;
        Regulus.Utility.StageMachine _Machine;


        public User(Regulus.Remoting.ISoulBinder binder)
        {            
            this._Binder = binder;
            _Machine = new Regulus.Utility.StageMachine();
        }

        void Regulus.Game.IUser.OnKick(Guid id)
        {
            if (id == _Record.Id)
            {
                _ToVerify();
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
            var stage = new Verify(_Binder);
            stage.DoneEvent += _ToKerb;
            _Machine.Push(stage);
        }


        Data.Record _Record;
        private void _ToKerb(Data.Record record)
        {
            _Record = record;
            _VerifySuccessEvent(record.Id);
            var stage = new Kerb(_Binder);
            stage.DoneEvent += _ToPlay;
            _Machine.Push(stage);
        }

        private void _ToPlay()
        {
            
        }

        void Regulus.Framework.ILaunched.Shutdown()
        {
            _Machine.Termination();
        }
    }
}
