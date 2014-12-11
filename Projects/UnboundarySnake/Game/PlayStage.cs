using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.UnboundarySnake
{
    class PlayStage : Regulus.Game.IStage
    {
        private Remoting.ISoulBinder _Binder;
        private Snake _Snake;
        public delegate void EndCallnback();
        public event EndCallnback EndEvent;
        Zone _Zone;
        SnakeListener _Listener;
        public PlayStage(Remoting.ISoulBinder binder, Snake snake , Zone zone)
        {            
            this._Binder = binder;
            this._Snake = snake;
            _Zone = zone;
        }

        void Regulus.Game.IStage.Enter()
        {
            _Zone.Join(_Snake);
            
            

            _BindListener(_Listener);
        }

        private void _BindListener(SnakeListener _Listener)
        {
            throw new NotImplementedException();
        }

        void Regulus.Game.IStage.Leave()
        {
            _Zone.Left(_Snake);
            _UnbindListener(_Listener);
        }

        private void _UnbindListener(SnakeListener _Listener)
        {
            throw new NotImplementedException();
        }

        void Regulus.Game.IStage.Update()
        {
            
        }
    }
}
