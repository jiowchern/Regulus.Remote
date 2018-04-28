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

            public void GetInfomation(ref List<Infomation> nodes)
            {
                _Child.GetInfomation(ref nodes);
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

        

        public ParallelNode(bool same_is_success)
        {
            _SameIsSuccess = same_is_success;
            _Childs = new List<Item>();
        }


        void ITicker.GetInfomation(ref List<Infomation> nodes)
        {
            foreach (var ticker in _Childs)
            {
                ticker.GetInfomation(ref nodes);
            }
        }

        void ITicker.Reset()
        {
            foreach (var ticker in _Childs)
            {
                ticker.Reset();
            }
        }

        public TICKRESULT Tick(float delta)
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

        public void Add(ITicker child)
        {
            _Childs.Add(new Item(child));
        }
    }
}
