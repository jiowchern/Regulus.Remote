using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;


using Regulus.Utiliey;
using Regulus.Remote.Extension;
using Regulus.Utility;

namespace Regulus.Remote
{
	public interface IGPIBinder : IBootable
	{
	}

	public class GPIBinder<T> : IGPIBinder
		where T : class
	{
		public delegate CommandParam OnBuilder(T source);

		public delegate void OnSourceHandler(T source);

		public event OnSourceHandler SupplyEvent;

		public event OnSourceHandler UnsupplyEvent;

		public struct Data
		{
			private readonly OnBuilder _Builder;

			private readonly string _Name;

			public OnBuilder Builder
			{
				get { return _Builder; }
			}

			public string Name
			{
				get { return _Name; }
			}

			public string UnregisterName
			{
				get { return new Command.Analysis(_Name).Command; }
			}

			public Data(string name, OnBuilder builder)
			{
				_Name = name;
				_Builder = builder;
			}
		}

		private struct Source
		{
			public T GPI;

			public int Sn;
		}

		private readonly Command _Command;

		private readonly List<Source> _GPIs;

		private readonly List<Data> _Handlers;

		private readonly List<CommandRegister> _InvokeDatas;

		private readonly INotifier<T> _Notice;

		private int _Sn;

		public GPIBinder(INotifier<T> notice, Command command)
		{
			_Command = command;
			_Notice = notice;
			_GPIs = new List<Source>();
			_Handlers = new List<Data>();
			_InvokeDatas = new List<CommandRegister>();
		}

		void IBootable.Launch()
		{
			_Notice.Supply += _Notice_Supply;
			_Notice.Unsupply += _Notice_Unsupply;
		}

		void IBootable.Shutdown()
		{
			_Notice.Unsupply -= _Notice_Unsupply;
			_Notice.Supply -= _Notice_Supply;

			foreach(var gpi in _GPIs.ToArray())
			{
				_Notice_Unsupply(gpi.GPI);
			}
		}

		private void _Notice_Supply(T obj)
		{
			var sn = _Checkin(obj);
			_Register(obj, sn);

			if(SupplyEvent != null)
			{
				SupplyEvent(obj);
			}
		}

		private void _Notice_Unsupply(T obj)
		{
			if(UnsupplyEvent != null)
			{
				UnsupplyEvent(obj);
			}

			var sn = _Checkout(obj);
			_Unregister(sn);
		}

		private void _Register(T obj, int sn)
		{
			foreach(var handler in _Handlers)
			{
				var param = handler.Builder(obj);
				_Command.Register(GPIBinder<T>._BuileName(sn, handler.Name), param);
			}

			foreach(var id in _InvokeDatas)
			{
				id.Register(sn , obj);
			}
		}

		private void _Unregister(int sn)
		{
			foreach(var id in _InvokeDatas)
			{
				id.Unregister(sn);
			}

			foreach(var handler in _Handlers)
			{
				_Command.Unregister(GPIBinder<T>._BuileName(sn, handler.UnregisterName));
			}
		}

		private int _Checkin(T obj)
		{
			var sn = _GetSn();
			_GPIs.Add(
				new Source
				{
					GPI = obj, 
					Sn = sn
				});
			return sn;
		}

		private int _Checkout(T obj)
		{
			var source = _Find(obj);
			_GPIs.Remove(source);
			return source.Sn;
		}

		private Source _Find(T obj)
		{
			return (from source in _GPIs where source.GPI == obj select source).SingleOrDefault();
		}

		private int _GetSn()
		{
			return _Sn++;
		}

		private static string _BuileName(int sn, string name)
		{
			return sn + name;
		}

		

		public void Bind( Expression<Action<T>> exp)
		{			
			_InvokeDatas.Add(new CommandRegister<T>(_Command, exp));
		}

		public void Bind<T1>(Expression<Action<T, T1>> exp)
		{
			
			_InvokeDatas.Add(new CommandRegister<T, T1>(_Command, exp));
		}

		public void Bind<T1, T2>(Expression<Action<T, T1, T2>> exp)
		{
			
			_InvokeDatas.Add(new CommandRegister<T, T1, T2>( _Command, exp));
		}

        public void BindStatic<T1, T2>(Expression<Action<T, T1, T2>> exp)
        {

            _InvokeDatas.Add(new CommandRegisterStatic<T, T1, T2>(_Command, exp));
        }



        public void Bind<T1, T2, T3>(Expression<Action<T, T1, T2, T3>> exp)
		{
			
			_InvokeDatas.Add(new CommandRegister<T, T1, T2, T3>(_Command, exp));
		}

		public void Bind<T1, T2, T3, T4>(Expression<Callback<T, T1, T2, T3, T4>> exp)
		{
			
			_InvokeDatas.Add(new CommandRegister<T, T1, T2, T3, T4>(_Command, exp));
		}

		public void Bind<TR>(Expression<Func<T, TR>> exp, Action<TR> ret)
		{
			
			_InvokeDatas.Add(new CommandRegisterReturn<T, TR>(_Command, exp, ret));
		}

		public void Bind<T1, TR>(Expression<Func<T, T1, TR>> exp, Action<TR> ret)
		{
			
			_InvokeDatas.Add(new CommandRegisterReturn<T, T1, TR>(_Command, exp, ret));
		}

		public void Bind<T1, T2, TR>(Expression<Func<T, T1, T2, TR>> exp, Action<TR> ret)
		{
			
			_InvokeDatas.Add(
				new CommandRegisterReturn<T, T1, T2, TR>(
					
					_Command, 
					exp, 
					ret));
		}

        public void BindStatic<T1, T2, TR>(Expression<Func<T, T1, T2, TR>> exp, Action<TR> ret)
        {

            _InvokeDatas.Add(
                new CommandRegisterStaticReturn<T, T1, T2, TR>(

                    _Command,
                    exp,
                    ret));
        }

        

        public void Bind<T1, T2, T3, TR>(Expression<Func<T, T1, T2, T3, TR>> exp, Action<TR> ret)
		{
			
			_InvokeDatas.Add(
				new CommandRegisterReturn<T, T1, T2, T3, TR>(
					
					_Command, 
					exp, 
					ret));
		}

		public void Bind<T1, T2, T3, T4, TR>(Expression<Callback<T, T1, T2, T3, T4, TR>> exp, Action<TR> ret)
		{
			
			_InvokeDatas.Add(
				new CommandRegisterReturn<T, T1, T2, T3, T4, TR>(
					
					_Command, 
					exp, 
					ret));
		}

		
	}
}
