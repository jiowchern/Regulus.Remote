// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandRegister.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the CommandRegister type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

using Regulus.Remoting.Extension;
using Regulus.Utility;

#endregion

namespace Regulus.Remoting
{
	internal abstract class CommandRegister
	{
		private readonly Command _Command;

		private readonly string[] _ParamNames;

		public string Name { get; private set; }

		protected CommandRegister(string name, string[] param_names, Command command)
		{
			this._ParamNames = param_names;
			this._Command = command;
			this.Name = name;
		}

		protected class RegisterData : CommandParam
		{
		}

		public void Register(object instance)
		{
			var data = this._RegisterAction(instance);
			this._Command.Register(this._BuildName(this.Name, this._ParamNames), data);
		}

		private string _BuildName(string _Name, string[] _ParamNames)
		{
			return string.Format("{0}[{1}]", _Name, string.Join(",", _ParamNames));
		}

		protected abstract RegisterData _RegisterAction(object instance);

		protected Type[] _GetTypes(ReadOnlyCollection<ParameterExpression> readOnlyCollection)
		{
			return (from pe in readOnlyCollection.Skip(1) select pe.Type).ToArray();
		}

		public void Unregister()
		{
			this._Command.Unregister(new Command.Analysis(this.Name).Command);
		}
	}

	internal class CommandRegister<T> : CommandRegister
	{
		private readonly Expression<Action<T>> _Expression;

		public CommandRegister(
			string method_name, 
			string[] param_names, 
			Command command, 
			Expression<Action<T>> exp)
			: base(method_name, param_names, command)
		{
			this._Expression = exp;
		}

		protected override RegisterData _RegisterAction(object instance)
		{
			var callback = this._Expression.Compile();
			var paramTypes = this._GetTypes(this._Expression.Parameters);

			return new RegisterData
			{
				Callback = new Action(() => { callback((T)instance); }), 
				Return = null, 
				ReturnType = null, 
				Types = paramTypes
			};
		}
	}

	internal class CommandRegister<T, T1> : CommandRegister
	{
		private readonly Expression<Action<T, T1>> _Expression;

		public CommandRegister(
			string method_name, 
			string[] param_names, 
			Command command, 
			Expression<Action<T, T1>> exp)
			: base(method_name, param_names, command)
		{
			this._Expression = exp;
		}

		protected override RegisterData _RegisterAction(object instance)
		{
			var callback = this._Expression.Compile();
			var paramTypes = this._GetTypes(this._Expression.Parameters);

			return new RegisterData
			{
				Callback = new Action<T1>(t1 => { callback((T)instance, t1); }), 
				Return = null, 
				ReturnType = null, 
				Types = paramTypes
			};
		}
	}

	internal class CommandRegister<T, T1, T2> : CommandRegister
	{
		private readonly Expression<Action<T, T1, T2>> _Expression;

		public CommandRegister(
			string method_name, 
			string[] param_names, 
			Command command, 
			Expression<Action<T, T1, T2>> exp)
			: base(method_name, param_names, command)
		{
			this._Expression = exp;
		}

		protected override RegisterData _RegisterAction(object instance)
		{
			var callback = this._Expression.Compile();
			var paramTypes = this._GetTypes(this._Expression.Parameters);

			return new RegisterData
			{
				Callback = new Action<T1, T2>((t1, t2) => { callback((T)instance, t1, t2); }), 
				Return = null, 
				ReturnType = null, 
				Types = paramTypes
			};
		}
	}

	internal class CommandRegister<T, T1, T2, T3> : CommandRegister
	{
		private readonly Expression<Action<T, T1, T2, T3>> _Expression;

		public CommandRegister(
			string method_name, 
			string[] param_names, 
			Command command, 
			Expression<Action<T, T1, T2, T3>> exp)
			: base(method_name, param_names, command)
		{
			this._Expression = exp;
		}

		protected override RegisterData _RegisterAction(object instance)
		{
			var callback = this._Expression.Compile();
			var paramTypes = this._GetTypes(this._Expression.Parameters);

			return new RegisterData
			{
				Callback = new Action<T1, T2, T3>((t1, t2, t3) => { callback((T)instance, t1, t2, t3); }), 
				Return = null, 
				ReturnType = null, 
				Types = paramTypes
			};
		}
	}

	public delegate void Callback<T, T1, T2, T3, T4>(T instance, T1 arg1, T2 arg2, T3 arg3, T4 arg4);


	internal class CommandRegister<T, T1, T2, T3, T4> : CommandRegister
	{
		private readonly Expression<Callback<T, T1, T2, T3, T4>> _Expression;

		public CommandRegister(
			string method_name, 
			string[] param_names, 
			Command command, 
			Expression<Callback<T, T1, T2, T3, T4>> exp)
			: base(method_name, param_names, command)
		{
			this._Expression = exp;
		}

		protected override RegisterData _RegisterAction(object instance)
		{
			var callback = this._Expression.Compile();
			var paramTypes = this._GetTypes(this._Expression.Parameters);

			return new RegisterData
			{
				Callback = new Action<T1, T2, T3, T4>((t1, t2, t3, t4) => { callback((T)instance, t1, t2, t3, t4); }), 
				Return = null, 
				ReturnType = null, 
				Types = paramTypes
			};
		}
	}


	internal class CommandRegisterReturn<T, TR> : CommandRegister
	{
		private readonly Expression<Func<T, TR>> _Expression;

		private readonly Action<TR> _ReturnCallback;

		public CommandRegisterReturn(
			string method_name, 
			string[] param_names, 
			Command command, 
			Expression<Func<T, TR>> exp, 
			Action<TR> return_callback)
			: base(method_name, param_names, command)
		{
			this._ReturnCallback = return_callback;
			this._Expression = exp;
		}

		protected override RegisterData _RegisterAction(object instance)
		{
			var callback = this._Expression.Compile();
			var paramTypes = this._GetTypes(this._Expression.Parameters);
			return new RegisterData
			{
				Callback = new Func<TR>(() => { return callback((T)instance); }), 
				Return = this._ReturnCallback, 
				ReturnType = typeof (TR), 
				Types = paramTypes
			};
		}
	}

	internal class CommandRegisterReturn<T, T1, TR> : CommandRegister
	{
		private readonly Expression<Func<T, T1, TR>> _Expression;

		private readonly Action<TR> _ReturnCallback;

		public CommandRegisterReturn(
			string method_name, 
			string[] param_names, 
			Command command, 
			Expression<Func<T, T1, TR>> exp, 
			Action<TR> return_callback)
			: base(method_name, param_names, command)
		{
			this._ReturnCallback = return_callback;
			this._Expression = exp;
		}

		protected override RegisterData _RegisterAction(object instance)
		{
			var callback = this._Expression.Compile();
			var paramTypes = this._GetTypes(this._Expression.Parameters);
			return new RegisterData
			{
				Callback = new Func<T1, TR>(t1 => { return callback((T)instance, t1); }), 
				Return = this._ReturnCallback, 
				ReturnType = typeof (TR), 
				Types = paramTypes
			};
		}
	}

	internal class CommandRegisterReturn<T, T1, T2, TR> : CommandRegister
	{
		private readonly Expression<Func<T, T1, T2, TR>> _Expression;

		private readonly Action<TR> _ReturnCallback;

		public CommandRegisterReturn(
			string method_name, 
			string[] param_names, 
			Command command, 
			Expression<Func<T, T1, T2, TR>> exp, 
			Action<TR> return_callback)
			: base(method_name, param_names, command)
		{
			this._ReturnCallback = return_callback;
			this._Expression = exp;
		}

		protected override RegisterData _RegisterAction(object instance)
		{
			var callback = this._Expression.Compile();
			var paramTypes = this._GetTypes(this._Expression.Parameters);
			return new RegisterData
			{
				Callback = new Func<T1, T2, TR>((t1, t2) => { return callback((T)instance, t1, t2); }), 
				Return = this._ReturnCallback, 
				ReturnType = typeof (TR), 
				Types = paramTypes
			};
		}
	}

	internal class CommandRegisterReturn<T, T1, T2, T3, TR> : CommandRegister
	{
		private readonly Expression<Func<T, T1, T2, T3, TR>> _Expression;

		private readonly Action<TR> _ReturnCallback;

		public CommandRegisterReturn(
			string method_name, 
			string[] param_names, 
			Command command, 
			Expression<Func<T, T1, T2, T3, TR>> exp, 
			Action<TR> return_callback)
			: base(method_name, param_names, command)
		{
			this._ReturnCallback = return_callback;
			this._Expression = exp;
		}

		protected override RegisterData _RegisterAction(object instance)
		{
			var callback = this._Expression.Compile();
			var paramTypes = this._GetTypes(this._Expression.Parameters);
			return new RegisterData
			{
				Callback = new Func<T1, T2, T3, TR>((t1, t2, t3) => { return callback((T)instance, t1, t2, t3); }), 
				Return = this._ReturnCallback, 
				ReturnType = typeof (TR), 
				Types = paramTypes
			};
		}
	}

	public delegate TR Callback<T, T1, T2, T3, T4, TR>(T instance, T1 arg1, T2 arg2, T3 arg3, T4 arg4);

	internal class CommandRegisterReturn<T, T1, T2, T3, T4, TR> : CommandRegister
	{
		private readonly Expression<Callback<T, T1, T2, T3, T4, TR>> _Expression;

		private readonly Action<TR> _ReturnCallback;

		public CommandRegisterReturn(
			string method_name, 
			string[] param_names, 
			Command command, 
			Expression<Callback<T, T1, T2, T3, T4, TR>> exp, 
			Action<TR> return_callback)
			: base(method_name, param_names, command)
		{
			this._ReturnCallback = return_callback;
			this._Expression = exp;
		}

		protected override RegisterData _RegisterAction(object instance)
		{
			var callback = this._Expression.Compile();
			var paramTypes = this._GetTypes(this._Expression.Parameters);
			return new RegisterData
			{
				Callback = new Func<T1, T2, T3, T4, TR>((t1, t2, t3, t4) => { return callback((T)instance, t1, t2, t3, t4); }), 
				Return = this._ReturnCallback, 
				ReturnType = typeof (TR), 
				Types = paramTypes
			};
		}
	}
}