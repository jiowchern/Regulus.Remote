using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

using Regulus.Utility;

namespace Regulus.BehaviourTree
{
    class ActionNode<T> : ITicker , IDeltaTimeRequester
    {
        
        private float _Delta;
        

        private IEnumerator<TICKRESULT> _Iterator;

        private Action _Start;

        private Func<float, TICKRESULT> _Tick;

        private Action _End;

        enum STATUS { NONE ,START , RUNNING , END}

        private STATUS _Status;

        private readonly Expression<Func<T>> _InstanceProvider;

        private readonly Expression<Func<T, Func<float, TICKRESULT>>> _TickExpression;

        private readonly Expression<Func<T, Action>> _StartExpression;

        private readonly Expression<Func<T, Action>> _EndExpression;

        private bool _LazyInitial;

        public readonly string _Tag;
        private readonly Guid _Id;
        

        public ActionNode(Expression<Func<T>> instance_provider
            , Expression<Func<T,Func<float , TICKRESULT>>> tick
            , Expression<Func<T, Action >> start
            , Expression<Func<T, Action>> end
             )
        {
            _Id = Guid.NewGuid();
            
            _InstanceProvider = instance_provider;
            _TickExpression = tick;
            _StartExpression = start;
            _EndExpression = end;

            var unaryExpression = _TickExpression.Body as UnaryExpression;
            if (unaryExpression == null)
                throw new NotSupportedException(string.Format("The expression is a {0} , must a {1}", _TickExpression.Body.NodeType, ExpressionType.Lambda));
            var methodCallExpression =  unaryExpression.Operand as MethodCallExpression;
            if (methodCallExpression == null)
                throw new NotSupportedException(string.Format("The expression is a {0} , must a {1}", methodCallExpression.NodeType, ExpressionType.Call));
            var constantExpression  =  methodCallExpression.Object as ConstantExpression;
            if (constantExpression == null)
                throw new NotSupportedException(string.Format("The expression is a {0} , must a {1}", constantExpression.NodeType, ExpressionType.Constant));

            var method = constantExpression.Value as MethodInfo;
            if(method==null)
                throw new NotSupportedException(string.Format("The expression is a {0} , must a {1}", constantExpression.Type, typeof(MethodInfo)));

            _Tag = method.Name;

            _Reset();
        }

        IEnumerable<TICKRESULT> _GetIterator()
        {
            if (_LazyInitial == false)
            {
                _LazyInitial = true;
                var instance = _InstanceProvider.Compile()();
                _Start = _StartExpression.Compile()(instance);
                _Tick = _TickExpression.Compile()(instance);
                _End = _EndExpression.Compile()(instance);

                
            }
            
            while (true)
            {
                _Start();
                _Status = STATUS.START;
                TICKRESULT result;

                do
                {
                    result = _Tick(_RequestDelta());
                    _Status = STATUS.RUNNING;
                    if (result == TICKRESULT.RUNNING)
                        yield return result;
                }
                while (result == TICKRESULT.RUNNING);

                _End();
                yield return result;
                _Status = STATUS.END;
            }
            
            
        }


        Guid ITicker.Id { get { return _Id; } }
        string ITicker.Tag { get { return _Tag; } }

        ITicker[] ITicker.GetChilds()
        {
            return new ITicker[0];
        }

        void ITicker.GetPath(ref List<Guid> nodes)
        {
            nodes.Add(_Id);
        }

        /*ITicker[] ITicker.GetChilds()
        {
            return new ITicker[0];
        }*/

        void ITicker.Reset()
        {
            if (_Status != STATUS.NONE || _Status != STATUS.END ) 
            {
                _End();
            }

            _Reset();
        }

        private void _Reset()
        {
            _Status = STATUS.NONE;
            _Iterator = _GetIterator().GetEnumerator();            
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
