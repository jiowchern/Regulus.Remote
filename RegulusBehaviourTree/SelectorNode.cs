using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.BehaviourTree
{
    class SelectorNode : ITicker , IParent
    {
        private List<ITicker> _Childs;
        private readonly Queue<ITicker> _Queue;


        private ITicker _RunninTicker;

        public SelectorNode()
        {
            _Childs = new List<ITicker>();
            _Queue = new Queue<ITicker>();
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

            var ticker = _Queue.Dequeue();
            var result = ticker.Tick(delta);
            if (result == TICKRESULT.SUCCESS)
            {
                _Queue.Clear();
                return TICKRESULT.SUCCESS;
            }

            if (result == TICKRESULT.RUNNING)
            {
                _RunninTicker = ticker;
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
