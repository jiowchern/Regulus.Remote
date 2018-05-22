using System;
using System.Collections.Generic;

namespace Regulus.BehaviourTree
{
    public class SuccessNode : IParent
    {
        private ITicker _Ticker;
        private readonly Guid _Id;
        private readonly string _Tag;


        public SuccessNode()
        {
            _Tag = "Success";
            _Id = Guid.NewGuid();
        }
        public Guid Id { get { return _Id; } }
        string ITicker.Tag { get { return _Tag; } }
        ITicker[] ITicker.GetChilds()
        {
            return new ITicker[]{ _Ticker };
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