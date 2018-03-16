using System;
using System.Collections.Generic;

namespace Regulus.BehaviourTree
{
    internal class InvertNode  : IParent
    {
        private ITicker _Ticker;

        public InvertNode()
        {            

        }


        void ITicker.GetInfomation(ref  List<Infomation> nodes)
        {
            _Ticker.GetInfomation(ref nodes);
        }

        void ITicker.Reset()
        {
            _Ticker.Reset();
        }

        TICKRESULT ITicker.Tick(float delta)
        {
            var result = _Ticker.Tick(delta);
            if(result == TICKRESULT.FAILURE)
                return TICKRESULT.SUCCESS;
            else if(result == TICKRESULT.SUCCESS)
                return TICKRESULT.FAILURE;
            return result;
        }

        void IParent.Add(ITicker ticker)
        {
            if(_Ticker != null)
                throw new Exception("Inverse nodes only support a single node.");
            _Ticker = ticker;
        }
    }
}