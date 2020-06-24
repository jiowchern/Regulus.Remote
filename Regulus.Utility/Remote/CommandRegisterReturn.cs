using System;
using System.Linq.Expressions;
using System.Reflection;

using Regulus.Remote.Extension;
using Regulus.Utility;

namespace Regulus.Remote
{
    internal class CommandRegisterReturn<T, TR> : CommandRegister
	{
		private readonly Expression<Func<T, TR>> _Expression;

		private readonly Action<TR> _ReturnCallback;

		public CommandRegisterReturn(
			
			Command command, 
			Expression<Func<T, TR>> exp, 
			Action<TR> return_callback)
			: base(command , exp)
		{
			_ReturnCallback = return_callback;
			_Expression = exp;
		}
        protected override void _RegisterAction(Command command, string command_name, object instance)
        {
            Func<TR> function = _Build(_Expression, instance);
            command.Register(command_name, function , _ReturnCallback);
        }

        private Func<TR> _Build(Expression<Func<T, TR>> expression, object instance)
        {
            if (expression.Body.NodeType == ExpressionType.Call)
            {
                var methodCall = expression.Body as MethodCallExpression;
                var method = methodCall.Method;
                var functiohn = Delegate.CreateDelegate(typeof(Func<TR>), instance, method);
                return (Func<TR>)functiohn;
            }
            if (expression.Body.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression outerMember = (MemberExpression)expression.Body;
                PropertyInfo outerProp = (PropertyInfo)outerMember.Member;

                return new Func<TR>( ()=> (TR)outerProp.GetValue(instance , new object[0]) ); 
            }
            throw new NotSupportedException(expression.Body.NodeType.ToString());
        }

    }

    internal class CommandRegisterReturn<T, T1, TR> : CommandRegister
    {
        private readonly Expression<Func<T, T1, TR>> _Expression;

        private readonly Action<TR> _ReturnCallback;

        public CommandRegisterReturn(

            Command command,
            Expression<Func<T, T1, TR>> exp,
            Action<TR> return_callback)
            : base(command, exp)
        {
            _ReturnCallback = return_callback;
            _Expression = exp;
        }
        protected override void _RegisterAction(Command command, string command_name, object instance)
        {
            Func<T1, TR> function = _Build(_Expression, instance);
            command.Register(command_name, function, _ReturnCallback);
        }

        private Func<T1, TR> _Build(Expression<Func<T, T1, TR>> expression, object instance)
        {
            if (expression.Body.NodeType == ExpressionType.Call)
            {
                var methodCall = expression.Body as MethodCallExpression;
                var method = methodCall.Method;
                var functiohn = Delegate.CreateDelegate(typeof(Func<T1, TR>), instance, method);
                return (Func<T1, TR>)functiohn;
            }
            throw new NotSupportedException(expression.Body.NodeType.ToString());
        }

    }

    internal class CommandRegisterReturn<T, T1, T2, TR> : CommandRegister
    {
        private readonly Expression<Func<T, T1, T2, TR>> _Expression;

        private readonly Action<TR> _ReturnCallback;

        public CommandRegisterReturn(

            Command command,
            Expression<Func<T, T1, T2, TR>> exp,
            Action<TR> return_callback)
            : base(command, exp)
        {
            _ReturnCallback = return_callback;
            _Expression = exp;
        }

        protected override void _RegisterAction(Command command, string command_name, object instance)
        {
            Func<T1, T2, TR> function = _Build(_Expression, instance);
            command.Register(command_name, function, _ReturnCallback);
        }

        private Func<T1, T2, TR> _Build(Expression<Func<T, T1, T2, TR>> expression, object instance)
        {
            if (expression.Body.NodeType == ExpressionType.Call)
            {
                var methodCall = expression.Body as MethodCallExpression;
                var method = methodCall.Method;
                var functiohn = Delegate.CreateDelegate(typeof(Func<T1, T2, TR>), instance, method);
                return (Func<T1, T2, TR>)functiohn;
            }
            throw new NotSupportedException(expression.Body.NodeType.ToString());
        }
    }

    internal class CommandRegisterReturn<T, T1, T2, T3, TR> : CommandRegister
    {
        private readonly Expression<Func<T, T1, T2, T3, TR>> _Expression;

        private readonly Action<TR> _ReturnCallback;

        public CommandRegisterReturn(

            Command command,
            Expression<Func<T, T1, T2, T3, TR>> exp,
            Action<TR> return_callback)
            : base(command, exp)
        {
            _ReturnCallback = return_callback;
            _Expression = exp;
        }

        protected override void _RegisterAction(Command command, string command_name, object instance)
        {
            Func<T1, T2, T3, TR> function = _Build(_Expression, instance);
            command.Register(command_name, function, _ReturnCallback);
        }

        private Func<T1, T2, T3, TR> _Build(Expression<Func<T, T1, T2, T3, TR>> expression, object instance)
        {
            if (expression.Body.NodeType == ExpressionType.Call)
            {
                var methodCall = expression.Body as MethodCallExpression;
                var method = methodCall.Method;
                var functiohn = Delegate.CreateDelegate(typeof(Func<T1, T2, T3, TR>), instance, method);
                return (Func<T1, T2, T3, TR>)functiohn;
            }
            throw new NotSupportedException(expression.Body.NodeType.ToString());
        }
    }

    public delegate TR Callback<T, T1, T2, T3, T4, TR>(T instance, T1 arg1, T2 arg2, T3 arg3, T4 arg4);

    internal class CommandRegisterReturn<T, T1, T2, T3, T4, TR> : CommandRegister
    {
        private readonly Expression<Callback<T, T1, T2, T3, T4, TR>> _Expression;

        private readonly Action<TR> _ReturnCallback;

        public CommandRegisterReturn(

            Command command,
            Expression<Callback<T, T1, T2, T3, T4, TR>> exp,
            Action<TR> return_callback)
            : base(command, exp)
        {
            _ReturnCallback = return_callback;
            _Expression = exp;
        }

        protected override void _RegisterAction(Command command, string command_name, object instance)
        {
            Func<T1, T2, T3, T4, TR> function = _Build(_Expression, instance);
            command.Register(command_name, function, _ReturnCallback);
        }

        private Func<T1, T2, T3, T4, TR> _Build(Expression<Callback<T, T1, T2, T3, T4, TR>> expression, object instance)
        {
            if (expression.Body.NodeType == ExpressionType.Call)
            {
                var methodCall = expression.Body as MethodCallExpression;
                var method = methodCall.Method;
                var functiohn = Delegate.CreateDelegate(typeof(Func<T1, T2, T3, T4, TR>), instance, method);
                return (Func<T1, T2, T3, T4, TR>)functiohn;
            }
            throw new NotSupportedException(expression.Body.NodeType.ToString());
        }
    }

    internal class CommandRegisterStaticReturn<T, T1, T2, TR> : CommandRegister
    {
        private readonly Expression<Func<T, T1, T2, TR>> _Expression;

        private readonly Action<TR> _ReturnCallback;

        public CommandRegisterStaticReturn(

            Command command,
            Expression<Func<T, T1, T2, TR>> exp,
            Action<TR> return_callback)
            : base(command, exp)
        {
            _ReturnCallback = return_callback;
            _Expression = exp;
        }

        protected override void _RegisterAction(Command command, string command_name, object instance)
        {
            Func<T1, T2, TR> function = _Build(_Expression, instance);
            command.Register(command_name, function, _ReturnCallback);
        }

        private Func<T1, T2, TR> _Build(Expression<Func<T, T1, T2, TR>> expression, object instance)
        {
            if (expression.Body.NodeType == ExpressionType.Call)
            {
                var methodCall = expression.Body as MethodCallExpression;
                var method = methodCall.Method;

                return new Func<T1, T2, TR>((t1, t2) => { return (TR)method.Invoke(null, new object[] { instance, t1, t2 }); });
            }
            throw new NotSupportedException(expression.Body.NodeType.ToString());
        }
    }
}
