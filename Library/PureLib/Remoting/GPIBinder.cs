// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GPIBinder.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the IGPIBinder type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Regulus.Framework;
using Regulus.Remoting.Extension;
using Regulus.Utility;

#endregion

namespace Regulus.Remoting
{
	public interface IGPIBinder : IBootable
	{
	}


	public class GPIBinder<T> : IGPIBinder
		where T : class
	{
		public event OnSourceHandler SupplyEvent;

		public event OnSourceHandler UnsupplyEvent;

		private readonly Command _Command;

		private readonly List<Source> _GPIs;

		private readonly List<Data> _Handlers;

		private readonly List<CommandRegister> _InvokeDatas;

		private readonly INotifier<T> _Notice;

		private int _Sn;

		public GPIBinder(INotifier<T> notice, Command command)
		{
			this._Command = command;
			this._Notice = notice;
			this._GPIs = new List<Source>();
			this._Handlers = new List<Data>();
			this._InvokeDatas = new List<CommandRegister>();
		}

		void IBootable.Launch()
		{
			this._Notice.Supply += this._Notice_Supply;
			this._Notice.Unsupply += this._Notice_Unsupply;
		}

		void IBootable.Shutdown()
		{
			this._Notice.Unsupply -= this._Notice_Unsupply;
			this._Notice.Supply -= this._Notice_Supply;

			foreach (var gpi in this._GPIs.ToArray())
			{
				this._Notice_Unsupply(gpi.GPI);
			}
		}

		public delegate CommandParam OnBuilder(T source);

		public delegate void OnSourceHandler(T source);

		public struct Data
		{
			private readonly OnBuilder _Builder;

			private readonly string _Name;

			public OnBuilder Builder
			{
				get { return this._Builder; }
			}

			public string Name
			{
				get { return this._Name; }
			}

			public string UnregisterName
			{
				get { return new Command.Analysis(this._Name).Command; }
			}

			public Data(string name, OnBuilder builder)
			{
				this._Name = name;
				this._Builder = builder;
			}
		}

		private struct Source
		{
			public T GPI;

			public int Sn;
		}

		private void _Notice_Supply(T obj)
		{
			var sn = this._Checkin(obj);
			this._Register(obj, sn);

			if (this.SupplyEvent != null)
			{
				this.SupplyEvent(obj);
			}
		}

		private void _Notice_Unsupply(T obj)
		{
			if (this.UnsupplyEvent != null)
			{
				this.UnsupplyEvent(obj);
			}

			var sn = this._Checkout(obj);
			this._Unregister(sn);
		}

		private void _Register(T obj, int sn)
		{
			foreach (var handler in this._Handlers)
			{
				var param = handler.Builder(obj);
				this._Command.Register(GPIBinder<T>._BuileName(sn, handler.Name), param);
			}

			foreach (var id in this._InvokeDatas)
			{
				id.Register(obj);
			}
		}

		private void _Unregister(int sn)
		{
			foreach (var id in this._InvokeDatas)
			{
				id.Unregister();
			}

			foreach (var handler in this._Handlers)
			{
				this._Command.Unregister(GPIBinder<T>._BuileName(sn, handler.UnregisterName));
			}
		}

		private int _Checkin(T obj)
		{
			var sn = this._GetSn();
			this._GPIs.Add(new Source
			{
				GPI = obj, 
				Sn = sn
			});
			return sn;
		}

		private int _Checkout(T obj)
		{
			var source = this._Find(obj);
			this._GPIs.Remove(source);
			return source.Sn;
		}

		private Source _Find(T obj)
		{
			return (from source in this._GPIs where source.GPI == obj select source).SingleOrDefault();
		}

		private int _GetSn()
		{
			return this._Sn++;
		}

		private static string _BuileName(int sn, string name)
		{
			return sn + name;
		}

		public void Bind(string name, OnBuilder builder)
		{
			this._Handlers.Add(new Data(name, builder));
		}

		public void Bind(Expression<Action<T>> exp)
		{
			var name = this._GetName(exp);
			this._InvokeDatas.Add(new CommandRegister<T>(name[0], name.Skip(1).ToArray(), this._Command, exp));
		}

		public void Bind<T1>(Expression<Action<T, T1>> exp)
		{
			var name = this._GetName(exp);
			this._InvokeDatas.Add(new CommandRegister<T, T1>(name[0], name.Skip(1).ToArray(), this._Command, exp));
		}

		public void Bind<T1, T2>(Expression<Action<T, T1, T2>> exp)
		{
			var name = this._GetName(exp);
			this._InvokeDatas.Add(new CommandRegister<T, T1, T2>(name[0], name.Skip(1).ToArray(), this._Command, exp));
		}

		public void Bind<T1, T2, T3>(Expression<Action<T, T1, T2, T3>> exp)
		{
			var name = this._GetName(exp);
			this._InvokeDatas.Add(new CommandRegister<T, T1, T2, T3>(name[0], name.Skip(1).ToArray(), this._Command, exp));
		}

		public void Bind<T1, T2, T3, T4>(Expression<Callback<T, T1, T2, T3, T4>> exp)
		{
			var name = this._GetName(exp);
			this._InvokeDatas.Add(new CommandRegister<T, T1, T2, T3, T4>(name[0], name.Skip(1).ToArray(), this._Command, exp));
		}

		public void Bind<TR>(Expression<Func<T, TR>> exp, Action<TR> ret)
		{
			var name = this._GetName(exp);
			this._InvokeDatas.Add(new CommandRegisterReturn<T, TR>(name[0], name.Skip(1).ToArray(), this._Command, exp, ret));
		}

		public void Bind<T1, TR>(Expression<Func<T, T1, TR>> exp, Action<TR> ret)
		{
			var name = this._GetName(exp);
			this._InvokeDatas.Add(new CommandRegisterReturn<T, T1, TR>(name[0], name.Skip(1).ToArray(), this._Command, exp, ret));
		}

		public void Bind<T1, T2, TR>(Expression<Func<T, T1, T2, TR>> exp, Action<TR> ret)
		{
			var name = this._GetName(exp);
			this._InvokeDatas.Add(new CommandRegisterReturn<T, T1, T2, TR>(name[0], name.Skip(1).ToArray(), this._Command, exp, 
				ret));
		}

		public void Bind<T1, T2, T3, TR>(Expression<Func<T, T1, T2, T3, TR>> exp, Action<TR> ret)
		{
			var name = this._GetName(exp);
			this._InvokeDatas.Add(new CommandRegisterReturn<T, T1, T2, T3, TR>(name[0], name.Skip(1).ToArray(), this._Command, 
				exp, ret));
		}

		public void Bind<T1, T2, T3, T4, TR>(Expression<Callback<T, T1, T2, T3, T4, TR>> exp, Action<TR> ret)
		{
			var name = this._GetName(exp);
			this._InvokeDatas.Add(new CommandRegisterReturn<T, T1, T2, T3, T4, TR>(name[0], name.Skip(1).ToArray(), this._Command, 
				exp, ret));
		}

		private string[] _GetName(LambdaExpression exp)
		{
			string methodName;

			if (exp.Body.NodeType != ExpressionType.Call)
			{
				throw new ArgumentException();
			}

			var methodCall = exp.Body as MethodCallExpression;
			var method = methodCall.Method;
			methodName = method.Name;

			var argNames = from par in exp.Parameters.Skip(1) select par.Name;
			if (method.ReturnType == null)
			{
				return new[]
				{
					methodName
				}.Concat(argNames).ToArray();
			}

			return new[]
			{
				methodName
			}.Concat(new[]
			{
				"return"
			}).Concat(argNames).ToArray();
		}
	}
}