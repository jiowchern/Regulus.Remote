using System;

using System.Runtime.InteropServices;

using Regulus.Utility;
using Regulus.Utility.WindowConsoleStand;


namespace Regulus.Utility
{
    public abstract class WindowConsole : IUpdatable
	{
		public delegate void QuitCallback();

		public event QuitCallback QuitEvent;

		private readonly Console _Console;

		private readonly Console.IInput _Input;

		private readonly Updater _Updater;

        private AutoPowerRegulator _AutoPowerRegulator;


        public Command Command
		{
			get { return _Console.Command; }
		}

		public Console.IViewer Viewer { get; private set; }

		protected WindowConsole()
		{            
            _AutoPowerRegulator = new AutoPowerRegulator(new PowerRegulator());
            Viewer = new ConsoleViewer();
			var consoleInput = new ConsoleInput(Viewer);
			_Input = consoleInput;
			_Console = new Console(_Input, Viewer);
			_Updater = new Updater();


			_Updater.Add(consoleInput);
		}

		protected WindowConsole(Console.IViewer viewer , Console.IInput input)
		{
			_AutoPowerRegulator = new AutoPowerRegulator(new PowerRegulator());
			Viewer = viewer;
			_Input = input;
			_Console = new Console(_Input, Viewer);
			_Updater = new Updater();
		}

		void IBootable.Launch()
		{

            ShowLog();

			

			_Launch();
		}

        private void _Quit(object sender, EventArgs e)
        {
            QuitEvent();
        }

        void IBootable.Shutdown()
		{
			_Shutdown();
			_Updater.Shutdown();
			Singleton<Log>.Instance.RecordEvent -= _RecordView;
		}

		bool IUpdatable.Update()
		{
            _AutoPowerRegulator.Operate();
            _Update();
			_Updater.Working();
			return true;
		}

		protected abstract void _Launch();

		protected abstract void _Update();

		protected abstract void _Shutdown();

		public void HideLog()
		{
			Singleton<Log>.Instance.RecordEvent -= _RecordView;
			_Console.Command.Unregister("HideLog");
			_Console.Command.Register("ShowLog", ShowLog);
		}

		public void ShowLog()
		{
			Singleton<Log>.Instance.RecordEvent += _RecordView;
			_Console.Command.Unregister("ShowLog");
			_Console.Command.Register("HideLog", HideLog);
		}

		private void _RecordView(string message)
		{
            
            Viewer.WriteLine(message);
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
			private static void _Empty()
			{
				
			}
			public static void Run(this WindowConsole windowconsole, System.Action update_handler = null)
			{
				if (update_handler == null)
					update_handler = _Empty;

					AppDomain.CurrentDomain.UnhandledException += _Dump;
                var run = true;
                AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
                {
                    run = false;
                };
                windowconsole.Command.Register("quit", () => { run = false; });
				windowconsole.QuitEvent += () => { run = false; };
				windowconsole.Launch();
				while(run)
				{
					windowconsole.Update();
					update_handler();
				}

				windowconsole.Shutdown();
				windowconsole.Command.Unregister("quit");
			}

		    internal  static void _Dump(object sender, UnhandledExceptionEventArgs e)
		    {
		        System.IO.File.WriteAllText(string.Format("UnhandledException_{0}.log", DateTime.Now.ToString("yyyyMMdd-HHmmss")), e.ExceptionObject.ToString());
		        //Regulus.Utility.CrashDump.Write();
		        Singleton<Log>.Instance.Shutdown();
		    }
        }
	}
}



