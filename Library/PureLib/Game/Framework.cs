// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Framework.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Framework type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.Collections.Generic;
using System.Linq;

using Regulus.Framework;
using Regulus.Remoting;
using Regulus.Utility;

using Console = Regulus.Utility.Console;

#endregion

namespace Regulus.Game
{
	public abstract partial class Framework<TUser> : IUpdatable
		where TUser : IUpdatable
	{
		private Console _Console;

		protected Console.IInput _Input;

		private Updater _Loops;

		private bool _Runable;

		private StageMachine _StageMachine;

		protected Console.IViewer _Viewer;

		public Command Command
		{
			get { return this._Console.Command; }
		}

		public Console.IViewer Viewer
		{
			get { return this._Viewer; }
		}

		public Framework(Console.IViewer viewer, Console.IInput input, Console console)
		{
			this._Viewer = viewer;
			this._Input = input;
			this._Loops = new Updater();
			this._Console = console;
		}

		public Framework(Console.IViewer viewer, Console.IInput input)
		{
			this._Viewer = viewer;
			this._Input = input;
			this._Loops = new Updater();
			this._Console = new Console(this._Input, this._Viewer);
		}

		void IBootable.Launch()
		{
			this._Runable = true;

			this._StageMachine = this._CreateStage();

			this._Launch(this._Loops);
		}

		bool IUpdatable.Update()
		{
			this._Loops.Working();
			this._StageMachine.Update();
			return this._Runable;
		}

		void IBootable.Shutdown()
		{
			this._Shutdown(this._Loops);
			this._Stop();
		}

		public interface IController : IUpdatable
		{
			string Name { get; set; }

			void Look();

			void NotLook();

			TUser GetUser();
		}

		public delegate void BuildCompiled(TUser controller);

		public class ControllerProvider
		{
			public string Command;

			public Func<IController> Spawn;
		}

		protected abstract ControllerProvider[] _ControllerProvider();

		private StageMachine _CreateStage()
		{
			var stageMachine = new StageMachine();
			var sss = new StageSelectSystem(this._Viewer, this._ControllerProvider(), this.Command);
			sss.SelectSystemEvent += this.SelectSystemEvent;
			sss.SelectedEvent += this._OnSelectedSystem;
			stageMachine.Push(sss);
			return stageMachine;
		}

		private IUserRequester _OnSelectedSystem(ControllerProvider controller_provider)
		{
			this._Viewer.WriteLine("啟動系統");
			var ssr = new StageSystemReady(this._Viewer, controller_provider, this.Command);
			this._StageMachine.Push(ssr);

			return ssr;
		}

		public void SetLogMessage(Console.LogFilter flag)
		{
			this._Console.SetLogFilter(flag);
		}

		public void Stop()
		{
			this._Stop();
		}

		private void _Stop()
		{
			this._Runable = false;
			this._Loops.Shutdown();
			this._StageMachine.Termination();
			this._Loops = null;

			this._StageMachine = null;
			this._Viewer = null;
			this._Input = null;
			this._Console = null;
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
		public event OnSelectSystem SelectSystemEvent;

		public delegate void OnSelectSystem(ISystemSelector system_selector);

		public interface ISystemSelector
		{
			Value<IUserRequester> Use(string system);
		}

		private class StageSelectSystem : IStage, ISystemSelector
		{
			public event Func<ControllerProvider, IUserRequester> SelectedEvent;

			public event OnSelectSystem SelectSystemEvent;

			private readonly Command _Command;

			private readonly ControllerProvider[] _SystemProviders;

			private readonly Console.IViewer _Viewer;

			public StageSelectSystem(Console.IViewer viewer, ControllerProvider[] system_provider, Command command)
			{
				this.SelectSystemEvent = system_selector => { };
				this._Viewer = viewer;
				this._SystemProviders = system_provider;
				this._Command = command;
			}

			Value<IUserRequester> ISystemSelector.Use(string system)
			{
				var p = (from provider in this._SystemProviders where provider.Command == system select provider).FirstOrDefault();
				if (p != null)
				{
					return new Value<IUserRequester>(this.SelectedEvent(p));
				}

				this._Viewer.WriteLine("錯誤的系統名稱.");
				return null;
			}

			void IStage.Enter()
			{
				this._Viewer.WriteLine("選擇系統");

				foreach (var provider in this._SystemProviders)
				{
					this._Viewer.WriteLine(provider.Command);

					this._Command.Register(provider.Command, () =>
					{
						if (this.SelectedEvent != null)
						{
							this.SelectedEvent(provider);
						}
					});
				}

				this.SelectSystemEvent(new SystemSelector(this));
			}

			void IStage.Leave()
			{
				foreach (var provider in this._SystemProviders)
				{
					this._Command.Unregister(provider.Command);
				}
			}

			void IStage.Update()
			{
			}

			private class SystemSelector : ISystemSelector
			{
				private WeakReference _SystemSelector;

				public SystemSelector(ISystemSelector system_selector)
				{
					this._SystemSelector = new WeakReference(system_selector);
				}

				Value<IUserRequester> ISystemSelector.Use(string system)
				{
					if (this._SystemSelector != null && this._SystemSelector.IsAlive)
					{
						var val = (this._SystemSelector.Target as ISystemSelector).Use(system);
						this._SystemSelector = null;
						return val;
					}

					return null;
				}
			}
		}
	}

	public abstract partial class Framework<TUser>
	{
		public delegate void OnUserRequester(IUserRequester user_requester);

		public interface IUserRequester
		{
			Value<TUser> Spawn(string name, bool look);

			void Unspawn(string name);
		}

		public class UserRequester : IUserRequester
		{
			private readonly WeakReference _UserRequester;

			public UserRequester(IUserRequester user_requester)
			{
				this._UserRequester = new WeakReference(user_requester);
			}

			Value<TUser> IUserRequester.Spawn(string name, bool look)
			{
				if (this._UserRequester.IsAlive)
				{
					return (this._UserRequester.Target as IUserRequester).Spawn(name, look);
				}

				return null;
			}

			void IUserRequester.Unspawn(string name)
			{
				if (this._UserRequester.IsAlive)
				{
					(this._UserRequester.Target as IUserRequester).Unspawn(name);
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
				this.UserRequesterEvent = user_requester => { };
				this._Viewer = view;
				this._ControllerProvider = controller_provider;
				this._Command = command;

				this._SelectedControlls = new List<IController>();
				this._Controlls = new List<IController>();
				this._Loops = new Updater();
			}

			Value<TUser> IUserRequester.Spawn(string name, bool look)
			{
				var val = this._SpawnController(name);
				if (look)
				{
					this._SelectController(name);
				}

				return val;
			}

			void IUserRequester.Unspawn(string name)
			{
				this._UnspawnController(name);
			}

			void IStage.Enter()
			{
				this._Command.Register<string, TUser>("SpawnController", this._SpawnController, user => { });
				this._Command.Register<string>("SelectController", this._SelectController);
				this._Command.Register<string>("UnspawnController", this._UnspawnController);

				this.UserRequesterEvent(new UserRequester(this));
			}

			void IStage.Leave()
			{
				this._Command.Unregister("SelectController");
				this._Command.Unregister("UnsawnController");
				this._Command.Unregister("SpawnController");
				this._Loops.Shutdown();
			}

			void IStage.Update()
			{
				this._Loops.Working();
			}

			private void _UnspawnController(string name)
			{
				var controllers = (from controller in this._Controlls where controller.Name == name select controller).ToArray();
				foreach (var c in controllers)
				{
					this._SelectedControlls.Remove(c);
					this._Loops.Remove(c);
					this._Controlls.Remove(c);
					this._Viewer.WriteLine("控制者[" + name + "] 移除.");
				}
			}

			private TUser _SpawnController(string name)
			{
				var value = new Value<TUser>();
				var controller = this._ControllerProvider.Spawn();

				controller.Name = name;

				this._Controlls.Add(controller);
				this._Loops.Add(controller);

				this._Viewer.WriteLine("控制者[" + name + "] 增加.");
				return controller.GetUser();
			}

			private void _SelectController(string name)
			{
				foreach (var controller in this._SelectedControlls)
				{
					controller.NotLook();
				}

				this._SelectedControlls.Clear();
				this._SelectedControlls.AddRange(from controller in this._Controlls where controller.Name == name select controller);

				foreach (var controller in this._SelectedControlls)
				{
					controller.Look();
				}

				this._Viewer.WriteLine("選擇控制者[" + name + "]x" + this._SelectedControlls.Count());
			}
		}
	}
}