using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Game
{
    interface ITeammate
    { 
    }
    class Contingent
    {
        public string Map; 
        public ITeammate[] Teammates;
    }
}
namespace Regulus.Project.ExiledPrincesses.Game.Stage
{
    
    partial class Adventure : Regulus.Game.IStage
    {
        Regulus.Game.StageMachine _StageMachine;
        private Remoting.ISoulBinder _Binder;
        IZone _Zone;
        Contingent _Contingent;
        public Adventure(Contingent contingent , Remoting.ISoulBinder binder, Zone zone )
        {
            _Contingent = contingent;
            _Zone = zone;            
            this._Binder = binder;            
        }

        void Regulus.Game.IStage.Enter()
        {
            _StageMachine = new Regulus.Game.StageMachine();

            _ToQueryMap(_Contingent.Map);
        }

        private void _ToQueryMap(string map)
        {
            var qms = new EnterMapStage(map , _Zone);            
            _StageMachine.Push(qms);
        }

        void Regulus.Game.IStage.Leave()
        {
            _StageMachine.Termination();                                    
        }

        void Regulus.Game.IStage.Update()
        {
            _StageMachine.Update();
        }
        
        
    }

    partial class Adventure
    {
        class EnterMapStage : Regulus.Game.IStage
        {
            private string _Map;
            private IZone _Zone;

            public EnterMapStage(string map, IZone _Zone)
            {
                // TODO: Complete member initialization
                this._Map = map;
                this._Zone = _Zone;
            }
            void Regulus.Game.IStage.Enter()
            {
                var result =  _Zone.Query(_Map);
                result.OnValue += _QueryResult;
            }

            void _QueryResult(IMap map)
            {
                
            }

            void Regulus.Game.IStage.Leave()
            {
                
            }

            void Regulus.Game.IStage.Update()
            {
                
            }
        }
    }
}
