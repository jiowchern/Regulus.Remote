using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Regulus.Remoting.Extension;
using Regulus.Utility;

namespace Regulus.Remoting
{
	internal abstract class CommandRegister
	{		

		private readonly Command _Command;

	    private readonly string _Name;


        protected CommandRegister(Command command , LambdaExpression exp)
        {
            
            _Command = command;
		    _Name = _BuildName(exp);
		}

		public void Register(object instance)
		{		    
            _RegisterAction(_Command , _Name, instance);			
		}

		

		protected abstract void _RegisterAction(Command command,string command_name, object instance);
		

		public void Unregister()
		{
			_Command.Unregister(new Command.Analysis(_Name).Command);
		}


        private string _BuildName(LambdaExpression exp)
        {


            if (exp.Body.NodeType == ExpressionType.Call)
            {

                var methodCall = exp.Body as MethodCallExpression;
                var method = methodCall.Method;
                string methodName = method.Name;

                var argNames = from par in exp.Parameters.Skip(1) select par.Name;

                return string.Format("{0} [{1}]" , methodName , string.Join("," , argNames.ToArray()));
            }
            if (exp.Body.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression outerMember = (MemberExpression)exp.Body;
                PropertyInfo outerProp = (PropertyInfo)outerMember.Member;

                return outerProp.Name;
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
			: base(command , exp)
		{
			_Expression = exp;
		}

		protected override void _RegisterAction(Command command,string command_name, object instance)
		{
		    
			var callback = _Expression.Compile();					    
            command.Register(command_name, () => { callback((T)instance); });
		    
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
            var callback = _Expression.Compile();
            command.Register(command_name, (T1 t1) => { callback((T)instance , t1); });
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
            var callback = _Expression.Compile();
            command.Register(command_name, (T1 t1 , T2 t2) => { callback((T)instance, t1 , t2); });
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
            var callback = _Expression.Compile();
            command.Register(command_name, (T1 t1, T2 t2 , T3 t3) => { callback((T)instance, t1, t2 , t3); });
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
            var callback = _Expression.Compile();
            command.Register(command_name, (T1 t1, T2 t2, T3 t3 , T4 t4) => { callback((T)instance, t1, t2, t3 , t4); });
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
            var callback = _Expression.Compile();
            command.Register(command_name, () => { return callback((T)instance); } , _ReturnCallback);
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
            var callback = _Expression.Compile();
            command.Register(command_name, (T1 t1) => { return callback((T)instance , t1); }, _ReturnCallback);
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
            var callback = _Expression.Compile();
            command.Register(command_name, (T1 t1,T2 t2) => { return callback((T)instance, t1 , t2); }, _ReturnCallback);
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
            var callback = _Expression.Compile();
            command.Register(command_name, (T1 t1, T2 t2,T3 t3) => { return callback((T)instance, t1, t2,t3); }, _ReturnCallback);
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
            var callback = _Expression.Compile();
            command.Register(command_name, (T1 t1, T2 t2, T3 t3,T4 t4) => { return callback((T)instance, t1, t2, t3,t4); }, _ReturnCallback);
        }
    }
}
