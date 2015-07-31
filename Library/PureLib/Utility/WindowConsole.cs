// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowConsole.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the WindowConsole type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System.Runtime.InteropServices;

using Regulus.Framework;
using Regulus.Utility.WindowConsoleStand;

#endregion

namespace Regulus.Utility
{
	public abstract class WindowConsole : IUpdatable
	{
		public event QuitCallback QuitEvent;

		private readonly Console _Console;

		private readonly ConsoleInput _Input;

		private readonly Updater _Updater;

		public Command Command
		{
			get { return this._Console.Command; }
		}

		public Console.IViewer Viewer { get; private set; }

		protected WindowConsole()
		{
			this.Viewer = new ConsoleViewer();
			this._Input = new ConsoleInput(this.Viewer);
			this._Console = new Console(this._Input, this.Viewer);
			this._Updater = new Updater();
		}

		void IBootable.Launch()
		{
			WindowConsole.SetConsoleCtrlHandler(this.ConsoleCtrlCheck, true);

			this._HideLog();

			this._Updater.Add(this._Input);

			this._Launch();
		}

		void IBootable.Shutdown()
		{
			this._Shutdown();
			this._Updater.Shutdown();
			Singleton<Log>.Instance.RecordEvent -= this._RecordView;
		}

		bool IUpdatable.Update()
		{
			this._Update();
			this._Updater.Working();
			return true;
		}

		public delegate void QuitCallback();

		protected abstract void _Launch();

		protected abstract void _Update();

		protected abstract void _Shutdown();

		private void _HideLog()
		{
			Singleton<Log>.Instance.RecordEvent -= this._RecordView;
			this._Console.Command.Unregister("HideLog");
			this._Console.Command.Register("ShowLog", this._ShowLog);
		}

		private void _ShowLog()
		{
			Singleton<Log>.Instance.RecordEvent += this._RecordView;
			this._Console.Command.Unregister("ShowLog");
			this._Console.Command.Register("HideLog", this._HideLog);
		}

		private void _RecordView(string message)
		{
			this.Viewer.WriteLine(message);
		}

		#region unmanaged

		// Declare the SetConsoleCtrlHandler function
		// as external and receiving a delegate.
		[DllImport("Kernel32")]
		public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);

		// A delegate type to be used as the handler routine
		// for SetConsoleCtrlHandler.
		public delegate bool HandlerRoutine(CtrlTypes CtrlType);

		// An enumerated type for the control messages
		// sent to the handler routine.
		public enum CtrlTypes
		{
			CTRL_C_EVENT = 0, 

			CTRL_BREAK_EVENT, 

			CTRL_CLOSE_EVENT, 

			CTRL_LOGOFF_EVENT = 5, 

			CTRL_SHUTDOWN_EVENT
		}

		#endregion

		private bool ConsoleCtrlCheck(CtrlTypes ctrlType)
		{
			// Put your own handler here
			switch (ctrlType)
			{
				case CtrlTypes.CTRL_C_EVENT:
					this.QuitEvent();

					break;

				case CtrlTypes.CTRL_BREAK_EVENT:
					this.QuitEvent();

					break;

				case CtrlTypes.CTRL_CLOSE_EVENT:
					this.QuitEvent();

					break;

				case CtrlTypes.CTRL_LOGOFF_EVENT:
				case CtrlTypes.CTRL_SHUTDOWN_EVENT:
					this.QuitEvent();

					break;
			}

			return true;
		}
	}

	namespace WindowConsoleStand
	{
		public static class StandExtension
		{
			public static void Launch(this WindowConsole windowconsole)
			{
				IUpdatable updater = windowconsole;
				updater.Launch();
			}

			public static void Update(this WindowConsole windowconsole)
			{
				IUpdatable updater = windowconsole;
				updater.Update();
			}

			public static void Shutdown(this WindowConsole windowconsole)
			{
				IUpdatable updater = windowconsole;
				updater.Shutdown();
			}
		}
	}

	namespace WindowConsoleAppliction
	{
		public static class ApplictionExtension
		{
			public static void Run(this WindowConsole windowconsole)
			{
				var run = true;
				windowconsole.Command.Register("quit", () => { run = false; });
				windowconsole.QuitEvent += () => { run = false; };
				windowconsole.Launch();
				while (run)
				{
					windowconsole.Update();
				}

				windowconsole.Shutdown();
				windowconsole.Command.Unregister("quit");
			}
		}
	}
}