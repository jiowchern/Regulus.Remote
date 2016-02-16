using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.BehaviourTree
{
    class ParallelNode : ITicker , IParent
    {
        private readonly List<ITicker> _Childs;        

        private readonly int _NumRequiredToFail;

        private readonly int _NumRequiredToSucceed;

        public ParallelNode(int num_required_to_fail, int num_required_to_succeed)
        {
            _Childs = new List<ITicker>();
            this._NumRequiredToFail = num_required_to_fail;
            this._NumRequiredToSucceed = num_required_to_succeed;
        }

        public TICKRESULT Tick(float delta)
        {
            var numChildrenSuceeded = 0;
            var numChildrenFailed = 0;

            foreach (var child in _Childs)
            {
                var childStatus = child.Tick(delta);
                switch (childStatus)
                {
                    case TICKRESULT.SUCCESS: ++numChildrenSuceeded; break;
                    case TICKRESULT.FAILURE: ++numChildrenFailed; break;
                }
            }

            if (_NumRequiredToSucceed > 0 && numChildrenSuceeded >= _NumRequiredToSucceed)
            {
                return TICKRESULT.SUCCESS;
            }

            if (_NumRequiredToFail > 0 && numChildrenFailed >= _NumRequiredToFail)
            {
                return TICKRESULT.FAILURE;
            }

            return TICKRESULT.RUNNING;
        }

        public void Add(ITicker child)
        {
            _Childs.Add(child);
        }
    }
}
