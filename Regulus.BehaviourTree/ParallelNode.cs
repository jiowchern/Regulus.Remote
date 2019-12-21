using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.BehaviourTree
{
    class ParallelNode : IParent
    {
        private readonly bool _SameIsSuccess;

        class Item
        {
            private readonly ITicker _Child;
            private TICKRESULT _Result;
            public Item(ITicker child)
            {
                _Child = child;
                _Result = TICKRESULT.RUNNING;
            }

            public TICKRESULT Result => _Result;

            public void Tick(float delta)
            {
                if (_Result == TICKRESULT.RUNNING)
                {
                    var result = _Child.Tick(delta);
                    _Result = result;
                }                
            }

            

            public void Reset()
            {
                _Child.Reset();
                _Result = TICKRESULT.RUNNING;
                
            }

            public void Clear()
            {
                _Child.Reset();
                _Result = TICKRESULT.RUNNING;
            }
        }
        private readonly List<Item> _Childs;
        private readonly List<ITicker> _Tickers;
        private readonly Guid _Id;
        private readonly string _Tag;


        public ParallelNode(bool same_is_success)
        {
            _Id = Guid.NewGuid();
            _Tag = "Parallel";
            _SameIsSuccess = same_is_success;
            _Childs = new List<Item>();
            _Tickers = new List<ITicker>();
        }


        Guid ITicker.Id { get { return _Id; } }
        string ITicker.Tag { get { return _Tag; } }


        void ITicker.Reset()
        {
            foreach (var ticker in _Childs)
            {
                ticker.Reset();
            }
        }

        ITicker[] ITicker.GetChilds()
        {

            return _Tickers.ToArray();
        }

        void ITicker.GetPath(ref List<Guid> nodes)
        {
            nodes.Add(_Id);
            foreach (var child in _Tickers)
            {
                child.GetPath(ref nodes);
            }            
        }

        TICKRESULT ITicker.Tick(float delta)
        {

            foreach (var child in _Childs)
            {
                child.Tick(delta);
            }
            var numChildrenSuceeded = _Childs.Count(c => c.Result == TICKRESULT.SUCCESS);
            var numChildrenFailed = _Childs.Count(c => c.Result == TICKRESULT.FAILURE);

            if( numChildrenSuceeded + numChildrenFailed != _Childs.Count)
                return TICKRESULT.RUNNING;

            foreach (var child in _Childs)
            {
                child.Clear();
            }

            if (numChildrenSuceeded > numChildrenFailed)
                return TICKRESULT.SUCCESS;

            if (numChildrenSuceeded < numChildrenFailed)
                return TICKRESULT.FAILURE;

            if(_SameIsSuccess)
                return TICKRESULT.SUCCESS;
            return TICKRESULT.FAILURE;
        }

        void IParent.Add(ITicker child)
        {
            _Childs.Add(new Item(child));
            _Tickers.Add(child);
        }
    }
}
