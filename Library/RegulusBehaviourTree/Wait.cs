using System;
using System.Collections.Generic;

namespace Regulus.BehaviourTree.ActionHelper
{
    public class Coroutine : IAction
    {
        private readonly IEnumerable<TICKRESULT> _Provider;
        private IEnumerator<TICKRESULT> _Action;

        public Coroutine(IEnumerable<TICKRESULT> provider)
        {
            _Provider = provider;
            
        }
        void ITicker.Reset()
        {
            _Action = _Provider.GetEnumerator();
        }

        TICKRESULT ITicker.Tick(float delta)
        {            
            var result = _Action.Current;
            if(_Action.MoveNext() == false)
                return TICKRESULT.FAILURE;
            return result;
        }

        void IAction.Start()
        {
            _Action = _Provider.GetEnumerator();
        }

        void IAction.End()
        {            
        }
    }
    public class Wait : IAction
    {
        private readonly float _Timeup;
        private float _Counter;
        private readonly Func<bool> _Condition;

        public Wait(float timeup, Func<bool> condition)
        {
            _Timeup = timeup;
            _Condition = condition;
        }
        void ITicker.Reset()
        {
            _Counter = 0;
        }

        TICKRESULT ITicker.Tick(float delta)
        {
            _Counter += delta;
            if (_Counter > _Timeup)
                return TICKRESULT.FAILURE;

            if (_Condition.Invoke())
                return TICKRESULT.SUCCESS;
            return TICKRESULT.RUNNING;
        }

        void IAction.Start()
        {
            _Counter = 0;
        }

        void IAction.End()
        {

        }
    }
}