using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Utility
{
    public abstract class WindowConsole : Regulus.Utility.IUpdatable
    {
        Regulus.Utility.Console _Console;
        Regulus.Utility.Console.IViewer _Viewer;
        Regulus.Utility.ConsoleInput _Input;

        Regulus.Utility.Updater _Updater;

        public Regulus.Utility.Command Command
        {
            get { return _Console.Command; }
        }

        public Regulus.Utility.Console.IViewer Viewer
        {
            get { return _Viewer; }
        }

        protected WindowConsole()
        {
            _Viewer = new Regulus.Utility.ConsoleViewer();
            _Input = new ConsoleInput(_Viewer);
            _Console = new Regulus.Utility.Console(_Input, _Viewer);
            _Updater = new Updater();
        }

        protected abstract void _Launch();
        
        protected abstract void _Update();
        
        protected abstract void _Shutdown();

        void Framework.IBootable.Launch()
        {
            _HideLog();

            _Updater.Add(_Input);

            _Launch();
        }

        void Framework.IBootable.Shutdown()
        {
            _Shutdown();
            _Updater.Shutdown();
            Regulus.Utility.Log.Instance.RecordEvent -= _RecordView;
        }

        bool IUpdatable.Update()
        {
            _Update();
            _Updater.Working();
            return true;
        }

        private void _HideLog()
        {
            Regulus.Utility.Log.Instance.RecordEvent -= _RecordView;
            _Console.Command.Unregister("HideLog");
            _Console.Command.Register("ShowLog", _ShowLog);
        }

        private void _ShowLog()
        {
            Regulus.Utility.Log.Instance.RecordEvent += _RecordView;
            _Console.Command.Unregister("ShowLog");
            _Console.Command.Register("HideLog", _HideLog);
        }

        private void _RecordView(string message)
        {
            _Viewer.WriteLine(message);
        }

        
    }

    namespace WindowConsoleStand
    {
        public static class StandExtension
        {
            public static void Launch(this WindowConsole windowconsole)
            {
                Regulus.Utility.IUpdatable updater = windowconsole;
                updater.Launch();
            }

            public static void Update(this WindowConsole windowconsole)
            {
                Regulus.Utility.IUpdatable updater = windowconsole;
                updater.Update();
            }
            public static void Shutdown(this WindowConsole windowconsole)
            {
                Regulus.Utility.IUpdatable updater = windowconsole;
                updater.Shutdown();
            }
        }
    }

    namespace WindowConsoleAppliction
    {
        using WindowConsoleStand;
        public static class ApplictionExtension
        {
            public static void Run(this WindowConsole windowconsole)
            {
                bool run = true;
                windowconsole.Command.Register("quit", () => { run = false; });
                windowconsole.Launch();
                while(run)
                {
                    windowconsole.Update();
                }
                windowconsole.Shutdown();
                windowconsole.Command.Unregister("quit");
            }
        }
    }
}
