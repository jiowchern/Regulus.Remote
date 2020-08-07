using System;
using System.Collections.Generic;

namespace Regulus.BehaviourTree
{
    class SelectorNode : ITicker, IParent
    {
        private readonly List<ITicker> _Childs;
        private readonly Queue<ITicker> _Queue;


        private ITicker _RunninTicker;
        private ITicker _CurrentTicker;
        private readonly Guid _Id;
        private readonly string _Tag;

        public SelectorNode()
        {
            _Tag = "Selector";
            _Id = Guid.NewGuid();
            _Childs = new List<ITicker>();
            _Queue = new Queue<ITicker>();
        }

        Guid ITicker.Id { get { return _Id; } }
        string ITicker.Tag { get { return _Tag; } }

        ITicker[] ITicker.GetChilds()
        {
            return _Childs.ToArray();
        }

        void ITicker.GetPath(ref List<Guid> nodes)
        {
            nodes.Add(_Id);
            if (_CurrentTicker != null)
                _CurrentTicker.GetPath(ref nodes);
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
                TICKRESULT resultRunner = _RunninTicker.Tick(delta);

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
            TICKRESULT result = _CurrentTicker.Tick(delta);
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

            foreach (ITicker ticker in _Childs)
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
