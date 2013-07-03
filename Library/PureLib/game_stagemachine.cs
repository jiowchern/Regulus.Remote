using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Game
{
    public class StageMachine
    { 
        System.Collections.Generic.Queue<Regulus.Game.IStage> _StandBys;
        Regulus.Game.IStage _Current;

        public StageMachine()
        {
            _StandBys = new Queue<IStage>();
        }
        public void Push(Regulus.Game.IStage new_stage)
        {
            _StandBys.Enqueue(new_stage);
        }

        public bool Update()
        {
            foreach (var stage in _StandBys)
            {
                if (_Current != null)
                    _Current.Leave();

                if (stage != null)
                    stage.Enter();

                _Current = stage;
            }
            _StandBys.Clear();

            if (_Current != null)
            {
                _Current.Update();
                return true;
            }
            return false;
        }
        
    }

	public class StageMachine<T> 
	{
		System.Collections.Generic.Queue<Regulus.Game.IStage<T>> _StandBys;        
		Regulus.Game.IStage<T> _Current;
		T						_Param;
		public StageMachine(T par)
		{
			_Param = par;
			_StandBys = new Queue<IStage<T>>();
		}
		public void Push(Regulus.Game.IStage<T> new_stage)
		{
			_StandBys.Enqueue(new_stage);
		}

		public bool Update()
		{
            var standBy = _StandBys;

            
            while ( standBy.Count > 0)
            {
                var stage = standBy.Dequeue();

                if (_Current != null)
                    _Current.Leave(_Param);

                if (stage != null)
                    stage.Enter(_Param);

                _Current = stage;
            }            
			
			if (_Current != null)
			{
				_Current.Update(_Param);
				return true;
			}
			return false;
		}

        public void Termination()
        {
            _StandBys.Clear();
            if (_Current != null)
            {
                _Current.Leave(_Param);
                _Current = null;
            }
        }
		
	}
}
