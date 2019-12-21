using System;
using System.Collections.Generic;
using System.Linq;


using Regulus.Framework;
using Regulus.Remote;
using Regulus.Utility;


using Console = Regulus.Utility.Console;

namespace Regulus.Game
{
	public abstract partial class Framework<TUser> : IUpdatable
		where TUser : IUpdatable
	{
		public delegate void BuildCompiled(TUser controller);

		public class ControllerProvider
		{
			public string Command;

			public Func<IController> Spawn;
		}

		private Console _Console;

		protected Console.IInput _Input;

		private Updater _Loops;

		private bool _Runable;

		private StageMachine _StageMachine;

		protected Console.IViewer _Viewer;

		public Command Command
		{
			get { return _Console.Command; }
		}

		public Console.IViewer Viewer
		{
			get { return _Viewer; }
		}

		public Framework(Console.IViewer viewer, Console.IInput input, Console console)
		{
			_Viewer = viewer;
			_Input = input;
			_Loops = new Updater();
			_Console = console;
		}

		public Framework(Console.IViewer viewer, Console.IInput input)
		{
			_Viewer = viewer;
			_Input = input;
			_Loops = new Updater();
			_Console = new Console(_Input, _Viewer);
		}

		void IBootable.Launch()
		{
			_Runable = true;

			_StageMachine = _CreateStage();

			_Launch(_Loops);
		}

		bool IUpdatable.Update()
		{
			_Loops.Working();
			_StageMachine.Update();
			return _Runable;
		}

		void IBootable.Shutdown()
		{
			_Shutdown(_Loops);
			_Stop();
		}

		public interface IController : IUpdatable
		{
			string Name { get; set; }

			void Look();

			void NotLook();

			TUser GetUser();
		}

		protected abstract ControllerProvider[] _ControllerProvider();

		private StageMachine _CreateStage()
		{
			var stageMachine = new StageMachine();
			var sss = new StageSelectSystem(_Viewer, _ControllerProvider(), Command);
			sss.SelectSystemEvent += SelectSystemEvent;
			sss.SelectedEvent += _OnSelectedSystem;
			stageMachine.Push(sss);
			return stageMachine;
		}

		private IUserRequester _OnSelectedSystem(ControllerProvider controller_provider)
		{
			_Viewer.WriteLine("啟動系統");
			var ssr = new StageSystemReady(_Viewer, controller_provider, Command);
			_StageMachine.Push(ssr);

			return ssr;
		}

		public void SetLogMessage(Console.LogFilter flag)
		{
			_Console.SetLogFilter(flag);
		}

		public void Stop()
		{
			_Stop();
		}

		private void _Stop()
		{
			_Runable = false;
			_Loops.Shutdown();
			_StageMachine.Termination();
			_Loops = null;

			_StageMachine = null;
			_Viewer = null;
			_Input = null;
			_Console = null;
		}

		protected virtual void _Launch(Updater updater)
		{
		}

		protected virtual void _Shutdown(Updater updater)
		{
		}
	}

	public abstract partial class Framework<TUser>
	{
		public delegate void OnSelectSystem(ISystemSelector system_selector);

		public event OnSelectSystem SelectSystemEvent;

		private class StageSelectSystem : IStage, ISystemSelector
		{
			public event Func<ControllerProvider, IUserRequester> SelectedEvent;

			public event OnSelectSystem SelectSystemEvent;

			private class SystemSelector : ISystemSelector
			{
				private WeakReference _SystemSelector;

				public SystemSelector(ISystemSelector system_selector)
				{
					_SystemSelector = new WeakReference(system_selector);
				}

				Value<IUserRequester> ISystemSelector.Use(string system)
				{
					if(_SystemSelector != null && _SystemSelector.IsAlive)
					{
						var val = (_SystemSelector.Target as ISystemSelector).Use(system);
						_SystemSelector = null;
						return val;
					}

					return null;
				}
			}

			private readonly Command _Command;

			private readonly ControllerProvider[] _SystemProviders;

			private readonly Console.IViewer _Viewer;

			public StageSelectSystem(Console.IViewer viewer, ControllerProvider[] system_provider, Command command)
			{
				SelectSystemEvent = system_selector => { };
				_Viewer = viewer;
				_SystemProviders = system_provider;
				_Command = command;
			}

			Value<IUserRequester> ISystemSelector.Use(string system)
			{
				var p = (from provider in _SystemProviders where provider.Command == system select provider).FirstOrDefault();
				if(p != null)
				{
					return new Value<IUserRequester>(SelectedEvent(p));
				}

				_Viewer.WriteLine("錯誤的系統名稱.");
				return null;
			}

			void IStage.Enter()
			{
				_Viewer.WriteLine("選擇系統");

				foreach(var provider in _SystemProviders)
				{
					_Viewer.WriteLine(provider.Command);

					_Command.Register(
						provider.Command, 
						() =>
						{
							if(SelectedEvent != null)
							{
								SelectedEvent(provider);
							}
						});
				}

				SelectSystemEvent(new SystemSelector(this));
			}

			void IStage.Leave()
			{
				foreach(var provider in _SystemProviders)
				{
					_Command.Unregister(provider.Command);
				}
			}

			void IStage.Update()
			{
			}
		}

		public interface ISystemSelector
		{
			Value<IUserRequester> Use(string system);
		}
	}

	public abstract partial class Framework<TUser>
	{
		public delegate void OnUserRequester(IUserRequester user_requester);

		public class UserRequester : IUserRequester
		{
			private readonly WeakReference _UserRequester;

			public UserRequester(IUserRequester user_requester)
			{
				_UserRequester = new WeakReference(user_requester);
			}

			Value<TUser> IUserRequester.Spawn(string name, bool look)
			{
				if(_UserRequester.IsAlive)
				{
					return (_UserRequester.Target as IUserRequester).Spawn(name, look);
				}

				return null;
			}

			void IUserRequester.Unspawn(string name)
			{
				if(_UserRequester.IsAlive)
				{
					(_UserRequester.Target as IUserRequester).Unspawn(name);
				}
			}
		}

		private class StageSystemReady : IStage, IUserRequester
		{
			public event OnUserRequester UserRequesterEvent;

			private readonly Command _Command;

			private readonly ControllerProvider _ControllerProvider;

			private readonly List<IController> _Controlls;

			private readonly Updater _Loops;

			private readonly List<IController> _SelectedControlls;

			private readonly Console.IViewer _Viewer;

			public StageSystemReady(Console.IViewer view, ControllerProvider controller_provider, Command command)
			{
				UserRequesterEvent = user_requester => { };
				_Viewer = view;
				_ControllerProvider = controller_provider;
				_Command = command;

				_SelectedControlls = new List<IController>();
				_Controlls = new List<IController>();
				_Loops = new Updater();
			}

			Value<TUser> IUserRequester.Spawn(string name, bool look)
			{
				var val = _SpawnController(name);
				if(look)
				{
					_SelectController(name);
				}

				return val;
			}

			void IUserRequester.Unspawn(string name)
			{
				_UnspawnController(name);
			}

			void IStage.Enter()
			{
				_Command.Register<string, TUser>("SpawnController", _SpawnController, user => { });
				_Command.Register<string>("SelectController", _SelectController);
				_Command.Register<string>("UnspawnController", _UnspawnController);

				UserRequesterEvent(new UserRequester(this));
			}

			void IStage.Leave()
			{
				_Command.Unregister("SelectController");
				_Command.Unregister("UnsawnController");
				_Command.Unregister("SpawnController");
				_Loops.Shutdown();
			}

			void IStage.Update()
			{
				_Loops.Working();
			}

			private void _UnspawnController(string name)
			{
				var controllers = (from controller in _Controlls where controller.Name == name select controller).ToArray();
				foreach(var c in controllers)
				{
					_SelectedControlls.Remove(c);
					_Loops.Remove(c);
					_Controlls.Remove(c);
					_Viewer.WriteLine("控制者[" + name + "] 移除.");
				}
			}

			private TUser _SpawnController(string name)
			{
				var value = new Value<TUser>();
				var controller = _ControllerProvider.Spawn();

				controller.Name = name;

				_Controlls.Add(controller);
				_Loops.Add(controller);

				_Viewer.WriteLine("控制者[" + name + "] 增加.");
				return controller.GetUser();
			}

			private void _SelectController(string name)
			{
				foreach(var controller in _SelectedControlls)
				{
					controller.NotLook();
				}

				_SelectedControlls.Clear();
				_SelectedControlls.AddRange(from controller in _Controlls where controller.Name == name select controller);

				foreach(var controller in _SelectedControlls)
				{
					controller.Look();
				}

				_Viewer.WriteLine("選擇控制者[" + name + "]x" + _SelectedControlls.Count());
			}
		}

		public interface IUserRequester
		{
			Value<TUser> Spawn(string name, bool look);

			void Unspawn(string name);
		}
	}
}
