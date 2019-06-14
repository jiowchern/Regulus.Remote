using System;

using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;

using Regulus.Remote.Extension;
using Regulus.Utility;

namespace Regulus.Remote
{
	internal abstract class CommandRegister
	{
        
		private readonly Command _Command;

	    private readonly string _Name;
        private readonly string _Args;


        protected CommandRegister(Command command , LambdaExpression exp)
        {
            
            _Command = command;
		    _Build(exp,ref _Name ,ref _Args);
		}

		public void Register(int sn ,object instance)
		{
            var command = _BuildName(_Name, sn, _Args);
            _RegisterAction(_Command , command, instance);			
		}

		

		protected abstract void _RegisterAction(Command command,string command_name, object instance);
		

		public void Unregister(int sn)
		{
            var command = _BuildName(_Name, sn, _Args);
            _Command.Unregister(new Command.Analysis(command).Command);
		}

        private string _BuildName(string name, int sn, string args)
        {
            return string.Format("{0}{1} [{2}]" , sn , name , args);
        }

        private void _Build(LambdaExpression exp,ref string command_name ,ref string command_args)
        {


            if (exp.Body.NodeType == ExpressionType.Call)
            {

                var methodCall = exp.Body as MethodCallExpression;
                var method = methodCall.Method;
                string methodName = method.Name;

                
                var argNames = from par in exp.Parameters.Skip(1) select par.Name;

                command_name = methodName;
                command_args = string.Join(",", argNames.ToArray());
                return;
            }
            if (exp.Body.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression outerMember = (MemberExpression)exp.Body;
                PropertyInfo outerProp = (PropertyInfo)outerMember.Member;
                command_name = outerProp.Name;
                command_args = string.Empty;
                return;
            }
            throw new ArgumentException();
            

        }
        
    }

	internal class CommandRegister<T> : CommandRegister
	{
		private readonly Expression<Action<T>> _Expression;

		public CommandRegister(			
			Command command,
            Expression<Action<T>> exp)
			: base(command , exp )
		{
            
            _Expression = exp;
		}

	    

	    protected override void _RegisterAction(Command command,string command_name, object instance)
		{		    			
	        Action function = _Build(_Expression , instance);
            command.Register(command_name, function  );		    
		}

	    private Action _Build(Expression<Action<T>> expression, object instance)
	    {
            if (expression.Body.NodeType == ExpressionType.Call)
            {

                var methodCall = expression.Body as MethodCallExpression;
                var method = methodCall.Method;               
                var functiohn = Delegate.CreateDelegate(typeof (Action), instance, method);
                return (Action)functiohn;
            }            
            throw new NotSupportedException(expression.Body.NodeType.ToString());
        }
	}

	internal class CommandRegister<T, T1> : CommandRegister
	{
		private readonly Expression<Action<T, T1>> _Expression;

		public CommandRegister(
			
			Command command, 
			Expression<Action<T, T1>> exp)
			: base(command , exp)
		{
			_Expression = exp;
		}

        protected override void _RegisterAction(Command command, string command_name, object instance)
        {
            Action<T1> function = _Build(_Expression, instance);
            command.Register(command_name, function);
        }

	    private Action<T1> _Build(Expression<Action<T, T1>> expression, object instance)
	    {
            if (expression.Body.NodeType == ExpressionType.Call)
            {

                var methodCall = expression.Body as MethodCallExpression;
                var method = methodCall.Method;
                var functiohn = Delegate.CreateDelegate(typeof(Action<T1>), instance, method );
                return (Action<T1>)functiohn;
            }
            throw new NotSupportedException(expression.Body.NodeType.ToString());
        }
	}

	internal class CommandRegister<T, T1, T2> : CommandRegister
	{
		private readonly Expression<Action<T, T1, T2>> _Expression;

		public CommandRegister(
			Command command, 
			Expression<Action<T, T1, T2>> exp)
			: base(command , exp)
		{
			_Expression = exp;
		}
        protected override void _RegisterAction(Command command, string command_name, object instance)
        {
            Action<T1,T2> function = _Build(_Expression, instance);
            command.Register(command_name, function);
        }

        private Action<T1,T2> _Build(Expression<Action<T, T1,T2>> expression, object instance)
        {
            if (expression.Body.NodeType == ExpressionType.Call)
            {

                var methodCall = expression.Body as MethodCallExpression;
                var method = methodCall.Method;
                var functiohn = Delegate.CreateDelegate(typeof(Action<T1,T2>), instance, method);
                return (Action<T1,T2>)functiohn;
            }
            throw new NotSupportedException(expression.Body.NodeType.ToString());
        }


    }

	internal class CommandRegister<T, T1, T2, T3> : CommandRegister
	{
		private readonly Expression<Action<T, T1, T2, T3>> _Expression;

		public CommandRegister(
			
			Command command, 
			Expression<Action<T, T1, T2, T3>> exp)
			: base(command , exp)
		{
			_Expression = exp;
		}

        protected override void _RegisterAction(Command command, string command_name, object instance)
        {
            Action<T1, T2,T3> function = _Build(_Expression, instance);
            command.Register(command_name, function);
        }

        private Action<T1, T2, T3> _Build(Expression<Action<T, T1, T2, T3>> expression, object instance)
        {
            if (expression.Body.NodeType == ExpressionType.Call)
            {

                var methodCall = expression.Body as MethodCallExpression;
                var method = methodCall.Method;
                var functiohn = Delegate.CreateDelegate(typeof(Action<T1, T2, T3>), instance, method);
                return (Action<T1, T2, T3>)functiohn;
            }
            throw new NotSupportedException(expression.Body.NodeType.ToString());
        }

    }

	public delegate void Callback<T, T1, T2, T3, T4>(T instance, T1 arg1, T2 arg2, T3 arg3, T4 arg4);

	internal class CommandRegister<T, T1, T2, T3, T4> : CommandRegister
	{
		private readonly Expression<Callback<T, T1, T2, T3, T4>> _Expression;

		public CommandRegister(
			
			Command command, 
			Expression<Callback<T, T1, T2, T3, T4>> exp)
			: base(command , exp)
		{
			_Expression = exp;
		}

        protected override void _RegisterAction(Command command, string command_name, object instance)
        {
            Action<T1, T2, T3, T4> function = _Build(_Expression, instance);
            command.Register(command_name, function);
        }

        private Action<T1, T2, T3, T4> _Build(Expression<Callback<T, T1, T2, T3, T4>> expression, object instance)
        {
            if (expression.Body.NodeType == ExpressionType.Call)
            {

                var methodCall = expression.Body as MethodCallExpression;
                var method = methodCall.Method;
                var functiohn = Delegate.CreateDelegate(typeof(Action<T1, T2, T3, T4>), instance, method);
                return (Action<T1, T2, T3, T4>)functiohn;
            }
            throw new NotSupportedException(expression.Body.NodeType.ToString());
        }
    }

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
			: base(command , exp)
		{
			_ReturnCallback = return_callback;
			_Expression = exp;
		}
        protected override void _RegisterAction(Command command, string command_name, object instance)
        {
            Func<T1,TR> function = _Build(_Expression, instance);
            command.Register(command_name, function, _ReturnCallback);
        }

        private Func<T1,TR> _Build(Expression<Func<T,T1, TR>> expression, object instance)
        {
            if (expression.Body.NodeType == ExpressionType.Call)
            {
                var methodCall = expression.Body as MethodCallExpression;
                var method = methodCall.Method;
                var functiohn = Delegate.CreateDelegate(typeof(Func<T1, TR>), instance, method);
                return (Func<T1,TR>)functiohn;
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
			: base(command , exp)
		{
			_ReturnCallback = return_callback;
			_Expression = exp;
		}

        protected override void _RegisterAction(Command command, string command_name, object instance)
        {
            Func<T1,T2, TR> function = _Build(_Expression, instance);
            command.Register(command_name, function, _ReturnCallback);
        }

        private Func<T1,T2, TR> _Build(Expression<Func<T, T1,T2, TR>> expression, object instance)
        {
            if (expression.Body.NodeType == ExpressionType.Call)
            {
                var methodCall = expression.Body as MethodCallExpression;
                var method = methodCall.Method;
                var functiohn = Delegate.CreateDelegate(typeof(Func<T1, T2,TR>), instance, method);
                return (Func<T1,T2, TR>)functiohn;
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
			: base(command , exp)
		{
			_ReturnCallback = return_callback;
			_Expression = exp;
		}

        protected override void _RegisterAction(Command command, string command_name, object instance)
        {
            Func<T1, T2,T3, TR> function = _Build(_Expression, instance);
            command.Register(command_name, function, _ReturnCallback);
        }

        private Func<T1, T2,T3, TR> _Build(Expression<Func<T, T1, T2,T3, TR>> expression, object instance)
        {
            if (expression.Body.NodeType == ExpressionType.Call)
            {
                var methodCall = expression.Body as MethodCallExpression;
                var method = methodCall.Method;
                var functiohn = Delegate.CreateDelegate(typeof(Func<T1, T2,T3, TR>), instance, method);
                return (Func<T1, T2,T3, TR>)functiohn;
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
			: base(command , exp)
		{
			_ReturnCallback = return_callback;
			_Expression = exp;
		}

        protected override void _RegisterAction(Command command, string command_name, object instance)
        {
            Func<T1, T2, T3,T4, TR> function = _Build(_Expression, instance);
            command.Register(command_name, function, _ReturnCallback);
        }

        private Func<T1, T2, T3,T4, TR> _Build(Expression<Callback<T, T1, T2, T3, T4, TR>> expression, object instance)
        {
            if (expression.Body.NodeType == ExpressionType.Call)
            {
                var methodCall = expression.Body as MethodCallExpression;
                var method = methodCall.Method;
                var functiohn = Delegate.CreateDelegate(typeof(Func<T1, T2, T3,T4, TR>), instance, method);
                return (Func<T1, T2, T3,T4, TR>)functiohn;
            }
            throw new NotSupportedException(expression.Body.NodeType.ToString());
        }
    }
}
