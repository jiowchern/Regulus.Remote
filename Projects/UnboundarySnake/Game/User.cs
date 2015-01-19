using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.UnboundarySnake
{
    class User : Regulus.Game.IUser
    {
        Zone _Zone;
        Regulus.Utility.StageMachine _Machine;
        
        IStorage _Storage;
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
        private Remoting.ISoulBinder _Binder;        

        public User(Remoting.ISoulBinder binder, IStorage _Storage , Zone zone)
        {            
            this._Binder = binder;
            _Zone = zone;
            _Binder.BreakEvent += _Break;
            this._Storage = _Storage;
            _Machine = new Regulus.Utility.StageMachine();
        }
        
        event Regulus.Game.OnQuit Regulus.Game.IUser.QuitEvent
        {
            add { _QuitEvent += value; }
            remove { _QuitEvent -= value; }
        }

        bool Utility.IUpdatable.Update()
        {
            return true;
        }

        void Framework.ILaunched.Launch()
        {
            _ToVerify();
        }

        private void _ToVerify()
        {
            var stage = new VerifyStage(_Binder, _Storage);
            stage.DoneEvent += _ToPlay;
            _Machine.Push(stage);
        }

        private void _ToPlay(Account account)
        {
            SnakeRecord record = account.Record;

            _VerifySuccessEvent(account.Id);
            var stage = new PlayStage(_Binder, new Snake(), _Zone);
            stage.EndEvent += _ToEnd;
            _Machine.Push(stage);
        }

        private void _ToEnd()
        {
            _QuitEvent();
        }

        private void _Break()
        {
            _QuitEvent();
        }

        void Framework.ILaunched.Shutdown()
        {
            
        }
    }
}
