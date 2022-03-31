namespace Regulus.Remote.Extensions
{
    static class ExpressionsExtensions
    {
        
        public static System.Reflection.MethodInfo Execute(this System.Linq.Expressions.Expression<GetObjectAccesserMethod> arg )
        {
            var e0 = arg.Reduce();
            System.Linq.Expressions.LambdaExpression e1;            
            if (e0.IsNotType(out e1))
            {
                throw new System.Exception($"{e0} is not {nameof(System.Linq.Expressions.LambdaExpression)}");
            }
            System.Linq.Expressions.UnaryExpression e2; 
            if(e1.Body.IsNotType(out e2))
            {
                throw new System.Exception($"{e1} is not {nameof(System.Linq.Expressions.UnaryExpression)}");
            }
            System.Linq.Expressions.MethodCallExpression e3;
            
            if (e2.Operand.IsNotType(out e3))
            {
                throw new System.Exception($"{e2} is not {nameof(System.Linq.Expressions.MethodCallExpression)}");
            }
           
         
            System.Linq.Expressions.ConstantExpression e6;
            if(e3.Object.IsNotType(out e6))
            {
                throw new System.Exception($"{e3} is not {nameof(System.Linq.Expressions.ConstantExpression)}");
            }
            System.Reflection.MethodInfo info;
            if(e6.Value.IsNotType(out info))
            {
                throw new System.Exception($"{e6} is not {nameof(System.Reflection.MethodInfo)}");
            }

            return info;
        }

    }
}
