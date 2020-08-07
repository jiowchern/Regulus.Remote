using System;
using System.Collections.Generic;

namespace Regulus.BehaviourTree.ActionHelper
{
    public class ProxyTicker : ITicker
    {

        private readonly Func<float, TICKRESULT> _Func;
        private readonly Guid _Id;
        private readonly string _Tag;

        public ProxyTicker(Func<float, TICKRESULT> expression)
        {
            _Id = Guid.NewGuid();
            _Tag = expression.Method.Name;
            _Func = expression;
        }

        public Guid Id { get { return _Id; } }
        string ITicker.Tag { get { return _Tag; } }
        ITicker[] ITicker.GetChilds()
        {

            return new ITicker[0];
        }

        void ITicker.GetPath(ref List<Guid> nodes)
        {
            nodes.Add(_Id);
        }

        void ITicker.Reset()
        {

        }

        TICKRESULT ITicker.Tick(float delta)
        {
            return _Func(delta);
        }
    }
}