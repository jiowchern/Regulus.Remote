using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Utility
{

    public class EmptyStage : IStage
    {
        void IStage.Enter()
        {
            
        }

        void IStage.Leave()
        {
            
        }

        void IStage.Update()
        {
            
        }
    }
    
    public class StageMachine
    {
        Regulus.Collection.Queue<Regulus.Utility.IStage> _StandBys;
        
        Regulus.Utility.IStage _Current;


        public Regulus.Utility.IStage Current { get 
        {
            
            return _Current;

        } }
        public StageMachine()
        {
            _StandBys = new Collection.Queue<IStage>();
        }
        public void Push(Regulus.Utility.IStage new_stage)
        {            
            _StandBys.Enqueue(new_stage);
        }
        
        public bool Update()
        {
            
            
            _SetCurrentStage();
            _UpdateCurrentStage();
            
            return _Current != null;
        }

        private void _SetCurrentStage()
        {
            IStage stage;
            if (_StandBys.TryDequeue(out stage))
            {
                if (_Current != null)
                {
                    _Current.Leave();
                }

                stage.Enter();
                _Current = stage;
            }                
            
        }

        private void _UpdateCurrentStage()
        {
            if (_Current != null)
            {
                _Current.Update();
            }
        }

        public void Termination()
        {
            _StandBys.DequeueAll();
            if (_Current != null)
            {
                _Current.Leave();
                _Current = null;
            }            
        }

        public void Empty()
        {
            Push(new EmptyStage());
        }
    }

	
}
