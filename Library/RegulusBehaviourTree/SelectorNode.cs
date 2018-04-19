using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.BehaviourTree
{
    class SelectorNode : ITicker , IParent
    {
        private readonly List<ITicker> _Childs;
        private readonly Queue<ITicker> _Queue;


        private ITicker _RunninTicker;
        private ITicker _CurrentTicker;

        public SelectorNode()
        {
            _Childs = new List<ITicker>();
            _Queue = new Queue<ITicker>();
        }

        void  ITicker.GetInfomation(ref List<Infomation> nodes)
        {
            if(_CurrentTicker != null)
                _CurrentTicker.GetInfomation(ref nodes);            
        }

        void ITicker.Reset()
        {
            if (_RunninTicker != null)
            {
                _RunninTicker.Reset();
                _RunninTicker = null;
            }
            _Queue.Clear();
        }

        public TICKRESULT Tick(float delta)
        {
            if (_RunninTicker != null)
            {
                var resultRunner = _RunninTicker.Tick(delta);

                if (resultRunner == TICKRESULT.RUNNING)
                    return TICKRESULT.RUNNING;

                _RunninTicker = null;
                if (resultRunner == TICKRESULT.SUCCESS)
                {
                    _Queue.Clear();
                    return TICKRESULT.SUCCESS;
                }

                if (_Queue.Count == 0)
                {
                    return TICKRESULT.FAILURE;
                }

            }
            if (_Queue.Count == 0)
            {
                _Reload();
            }

            _CurrentTicker = _Queue.Dequeue();
            var result = _CurrentTicker.Tick(delta);
            if (result == TICKRESULT.SUCCESS)
            {
                _Queue.Clear();
                return TICKRESULT.SUCCESS;
            }

            if (result == TICKRESULT.RUNNING)
            {
                _RunninTicker = _CurrentTicker;
                return TICKRESULT.RUNNING;
            }

            if (_Queue.Count == 0)
                return TICKRESULT.FAILURE;
            return TICKRESULT.RUNNING;
        }
        private void _Reload()
        {
            
            foreach (var ticker in _Childs)
            {                
                _Queue.Enqueue(ticker);
            }
        }
        public void Add(ITicker ticker)
        {
            _Childs.Add(ticker);
        }
    }
}
