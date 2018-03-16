using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Regulus.BehaviourTree.ActionHelper
{
    public class Coroutine : ITicker
    {
        private readonly IEnumerable<TICKRESULT> _Provider;
        private IEnumerator<TICKRESULT> _Action;
        
        private Infomation _Infomation;
        public Coroutine(Expression<Func<IEnumerable<TICKRESULT>>>  expression)
        {

            var methodCallExpression = expression.Body as MethodCallExpression;
            if(methodCallExpression == null)
                throw new NotSupportedException(string.Format("The expression is a {0} , must a {1}" , expression.Body.NodeType  , ExpressionType.Call));

            var tag = methodCallExpression.Method.Name;
            _Infomation = new Infomation() ;
            _Infomation.Tag = tag;
            _Provider = expression.Compile()();
            _Action = _Provider.GetEnumerator();
        }


        void ITicker.GetInfomation(ref List<Infomation> nodes)
        {
            nodes.Add(_Infomation);

        }

        void ITicker.Reset()
        {
            _Action = _Provider.GetEnumerator();
        }

        TICKRESULT ITicker.Tick(float delta)
        {            
            
            if (_Action.MoveNext())
            {
                var result = _Action.Current;
                return result;
            }
            return TICKRESULT.FAILURE;            
        }

        
    }
}