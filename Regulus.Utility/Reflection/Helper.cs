using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Regulus.Utility.Reflection
{

    public class TypeMethodCatcher
    {
        public readonly MethodInfo Method;

        public TypeMethodCatcher(LambdaExpression expression)
        {
            if (expression.NodeType != ExpressionType.Lambda)
                throw new SystemException("must a lambda");
            MethodCallExpression callExpression = expression.Body as MethodCallExpression;

            if (callExpression == null)
                throw new SystemException("must a call");
            Method = callExpression.Method;
        }


    }


}
