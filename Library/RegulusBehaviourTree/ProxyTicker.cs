using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Regulus.BehaviourTree.ActionHelper
{
    public class ProxyTicker : ITicker
    {
        
        private readonly Func<float, TICKRESULT> _Func;
        

        public ProxyTicker(Func<float, TICKRESULT> expression)
        {        
            _Func = expression;
        }

        void ITicker.GetInfomation(ref List<Infomation> nodes)
        {
            nodes.Add(new Infomation() { Tag = _Func.Method.Name});
            
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