using System;
using System.Collections.Generic;

namespace Regulus.BehaviourTree
{
    internal class InvertNode  : IParent
    {
        private ITicker _Ticker;
        private readonly Guid _Id;
        private string _Tag;

        public InvertNode()
        {
            _Id = Guid.NewGuid();

            _Tag = "Invert";
        }


        public Guid Id { get { return _Id; } }

        string ITicker.Tag { get { return _Tag; } }

        ITicker[] ITicker.GetChilds()
        {
            return new [] { _Ticker };
        }

        void ITicker.GetPath(ref List<Guid> nodes)
        {
            nodes.Add(_Id);
            _Ticker.GetPath(ref nodes);
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