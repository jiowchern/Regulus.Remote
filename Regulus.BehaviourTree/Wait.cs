using System;

namespace Regulus.BehaviourTree.ActionHelper
{
    public class Wait 
    {
        private readonly float _Timeup;
        private float _Counter;
        

        public Wait(float timeup)
        {
            _Timeup = timeup;
        
        }        
        

        public TICKRESULT Tick(float delta)
        {
            _Counter += delta;
            if (_Counter > _Timeup)
            {
                _Counter = 0;
                return TICKRESULT.SUCCESS;
            }
                
        
            return TICKRESULT.RUNNING;
        }

        public void Start()
        {
            _Counter = 0;
        }

        public void End()
        {

        }
    }
}