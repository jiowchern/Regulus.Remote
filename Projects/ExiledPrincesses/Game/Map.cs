using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Game
{
    interface IContingent
    {
        Guid Id { get; set; }
        event Action GoForwardEvent; 
    }

    partial class Map : IMap
    {
        float _Position;
        IContingent _Contingent;
        Regulus.Game.StageMachine _StageMachine;
        public Map()
        {
            _Position = 0.0f;            
        }

        bool Regulus.Game.IFramework.Update()
        {
            _StageMachine.Update();
            return true;
        }

        void Regulus.Game.IFramework.Launch()
        {
            _StageMachine = new Regulus.Game.StageMachine();
            _ToIdle();
        }

        private void _ToIdle()
        {
            var stage = new IdleStage(_Contingent);
            stage.GoForwardEvent += _ToGoForward;
            _StageMachine.Push(stage);
        }

        void _ToGoForward()
        {
            //var stage = new GoForwardStage(_Contingent);
            //_StageMachine.Push(stage);
        }

        void Regulus.Game.IFramework.Shutdown()
        {
            _StageMachine.Termination();
        }
    }

    partial class Map
    {
        class IdleStage : Regulus.Game.IStage
        {
            public delegate void OnGoForward();
            public event OnGoForward GoForwardEvent;
            private IContingent _Contingent;

            public IdleStage(IContingent contingent)
            {                
                this._Contingent = contingent;
            }
            void Regulus.Game.IStage.Enter()
            {
                _Contingent.GoForwardEvent += _OnGoForwar;
            }

            void _OnGoForwar()
            {
                if (GoForwardEvent != null)
                    GoForwardEvent();
            }

            void Regulus.Game.IStage.Leave()
            {
                _Contingent.GoForwardEvent -= _OnGoForwar;
            }

            void Regulus.Game.IStage.Update()
            {
                
            }
        }
    }
}
