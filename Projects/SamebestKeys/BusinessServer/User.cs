using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessServer
{
    class User : Regulus.Utility.IUpdatable
    {
        private Regulus.Remoting.ISoulBinder _Binder;
        private bool _Enable;
        Regulus.Game.StageMachine _Machine;
        public event OnVerifySucess VerifySuccessEvent;
        Storage _Storage;
        public User(Regulus.Remoting.ISoulBinder binder , Storage storage)
        {
            _Storage = storage;
            _Enable = true;
            this._Binder = binder; 
        }

        private void _Break()
        {
            _Enable = false;
        }

        bool Regulus.Utility.IUpdatable.Update()
        {
            return _Enable;
        }

        void Regulus.Framework.ILaunched.Launch()
        {
            _Binder.BreakEvent += _Break;
            _Machine = new Regulus.Game.StageMachine();


            _ToVerify();
        }

        private void _ToVerify()
        {
            Id = null;
            var stage = new Verify(_Binder, _Storage);
            stage.DoneEvent += _ToService;
            _Machine.Push(stage);
        }

        private void _ToService(int account_id)
        {
            Id = account_id;

            var stage = new Service(_Binder, _Storage);

            stage.DoneEvent += _ToVerify;
            _Machine.Push(stage);
        }

        void Regulus.Framework.ILaunched.Shutdown()
        {
            _Machine.Termination();
            _Binder.BreakEvent -= _Break;
        }

        public int? Id { get; private set; }
    }
}
