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
        class StageData
        {
            public Regulus.Utility.IStage Stage;            
        }
        System.Collections.Generic.Queue<Regulus.Utility.IStage> _StandBys;
        StageData _Current;


        public Regulus.Utility.IStage Current { get 
        {
            if (_Current != null)
                return _Current.Stage;
            return null;
        } }
        public StageMachine()
        {
            
            _StandBys = new Queue<IStage>();
            _Current = new StageData();
            _Handle = _HandleStandByEnter;
        }
        public void Push(Regulus.Utility.IStage new_stage)
        {
            lock (_StandBys)
                _StandBys.Enqueue(new_stage);
        }

        Action _Handle;
        void _HandleStandByEnter()
        {
            if (_StandBys.Count > 0)
            {
                if (_Current.Stage != null)
                    _Current.Stage.Leave();


                IStage stage;
                lock(_StandBys)
                {
                    stage = _StandBys.Dequeue();
                }
                

                
                if (stage != null)
                {
                    stage.Enter();                    
                }
                _Current.Stage = stage;
            }

            _Handle = _HandleCurrentStage;
        }
        void _HandleCheckStabdBy()
        {
            if (_StandBys.Count > 0)
            {
                _Handle = _HandleCurrentStageWait;
            }
            else
                _Handle = _HandleCurrentStage;
        }
        void _HandleCurrentStageWait()
        {            
            _Handle = _HandleStandByEnter;
        }
        void _HandleCurrentStage()
        {
            if (_Current.Stage != null)
            {
                _Current.Stage.Update();
            }

            _Handle = _HandleCheckStabdBy;
        }
        public bool Update()
        {
            //_Handle();

            if (_StandBys.Count > 0)
            {
                if (_Current.Stage != null)
                    _Current.Stage.Leave();

                IStage stage;
                lock (_StandBys)
                {
                    stage = _StandBys.Dequeue();
                }


                if (stage != null)
                {
                    stage.Enter();
                }
                _Current.Stage = stage;
            }

            if (_Current.Stage != null)
            {
                _Current.Stage.Update();
            }

            return _Current.Stage != null;
        }

        public void Termination()
        {
            _StandBys.Clear();
            if (_Current != null && _Current.Stage != null)
            {
                _Current.Stage.Leave();
                _Current.Stage = null;
            }
        }

        public void Empty()
        {
            Push(new EmptyStage());
        }
        
    }

	public class StageMachine<T> 
	{
        class StageData
        {
            public Regulus.Utility.IStage<T> Stage;
            public StageLock Lock;
        }
		System.Collections.Generic.Queue<Regulus.Utility.IStage<T>> _StandBys;
        StageData _Current;
		T						_Param;
		public StageMachine(T par)
		{
			_Param = par;
			_StandBys = new Queue<IStage<T>>();
            _Current = new StageData();
            _Handle = _HandleStandByEnter;
		}
		public void Push(Regulus.Utility.IStage<T> new_stage)
		{
			_StandBys.Enqueue(new_stage);
		}

        Action _Handle;
        void _HandleStandByEnter()
        {
            if (_StandBys.Count > 0)
            {
                if (_Current.Stage != null)
                    _Current.Stage.Leave(_Param);

                var stage = _StandBys.Dequeue();

                var unlock = new StageLock();
                unlock.Unlock();

                _Current.Lock = unlock;
                if (stage != null)
                {
                    var stageLock = stage.Enter(_Param);
                    if (stageLock != null)
                    {
                        _Current.Lock = stageLock;
                    }
                }
                _Current.Stage = stage;
            }
            
            _Handle = _HandleCurrentStage;
        }
        void _HandleCheckStabdBy()
        {
            if (_StandBys.Count > 0)
            {                
                _Handle = _HandleCurrentStageWait;                                    
            }
            else
                _Handle = _HandleCurrentStage;                   
        }
        void _HandleCurrentStageWait()
        {
            if (_Current.Stage != null)
            {                
                if (_Current.Lock.Current == StageLock.Status.Unlock)
                {
                    _Handle = _HandleStandByEnter;
                }
            }
            else
            {
                _Handle = _HandleStandByEnter;
            }
        }
        void _HandleCurrentStage()
        {
            if (_Current.Stage != null)
            {
                _Current.Stage.Update(_Param);
            }

            _Handle = _HandleCheckStabdBy;
        }
		public bool Update()
		{
            _Handle();

            return _Current.Stage != null;
		}

        public void Termination()
        {
            _StandBys.Clear();
            if (_Current.Stage != null)
            {
                _Current.Stage.Leave(_Param);
                _Current = null;
            }
        }
		
	}

    
}
