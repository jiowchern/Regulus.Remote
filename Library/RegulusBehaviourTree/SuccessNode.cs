using System;

namespace Regulus.BehaviourTree
{
    public class SuccessNode : IParent
    {
        private ITicker _Ticker;

        void ITicker.Reset()
        {
            _Ticker.Reset();
        }

        TICKRESULT ITicker.Tick(float delta)
        {
            _Ticker.Tick(delta);
            return TICKRESULT.SUCCESS;
        }

        void IParent.Add(ITicker ticker)
        {
            if (_Ticker != null)
                throw new Exception("Inverse nodes only support a single node.");

            _Ticker = ticker;
        }
    }
}