using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Regulus.Utility;

namespace Regulus.BehaviourTree
{
    class ActionNode<T> : ITicker , IDeltaTimeRequester
    {
        private readonly Expression<Func<T>> _InstnaceProvider;

        private readonly Expression<Func<T, Func<float, TICKRESULT>>> _Tick;

        private readonly Expression<Func<T, Action>> _Start;

        private readonly Expression<Func<T, Action>> _End;
        
        
        private float _Delta;
        

        private IEnumerator<TICKRESULT> _Iterator;

        public ActionNode(Expression<Func<T>> instance_provider
            , Expression< Func<T,Func<float , TICKRESULT> > > tick
            , Expression<Func<T, Action >> start
            , Expression<Func<T, Action>> end
             )
        {
            _InstnaceProvider = instance_provider;
            _Tick = tick;
            _Start = start;
            _End = end;

            _Iterator = _Update().GetEnumerator();
        }

        IEnumerable<TICKRESULT> _Update()
        {
            var instance = _InstnaceProvider.Compile()();
            var start = _Start.Compile()(instance);
            var tick = _Tick.Compile()(instance);
            var end = _End.Compile()(instance);
            while (true)
            {
                start();

                TICKRESULT result ;
                do
                {
                    result = tick(_RequestDelta());
                    if(result == TICKRESULT.RUNNING)
                        yield return result;
                }
                while (result == TICKRESULT.RUNNING);

                end();
                yield return result;
            }
        }


        TICKRESULT ITicker.Tick(float delta)
        {
            _Delta += delta;

            _Iterator.MoveNext();
            var result =  _Iterator.Current;            
            return result;
        }

        float IDeltaTimeRequester.Request()
        {
            return _RequestDelta();
        }

        private float _RequestDelta()
        {
            var d = _Delta;
            _Delta = 0f;
            return d;
        }
    }
}
