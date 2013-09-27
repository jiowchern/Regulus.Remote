using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserConsole
{
    public class Framework
    {
        bool bRun = false;

        Regulus.Utility.Console _Console;
        Regulus.Utility.Console.IInput _Input;
        Regulus.Utility.Console.IViewer _Viewer;
        Regulus.Game.StageMachine<Framework> _StageMachine;
        Regulus.Game.FrameworkRoot _FrameworkRoot;
        Regulus.Project.Crystal.IUser _User ;

        public event Action<Regulus.Project.Crystal.IUser> UserCreatedEvent;

        public Regulus.Utility.Command Command { get { return _Console.Command; } }
        public Regulus.Utility.Console.IViewer Viewer { get { return _Viewer; } }
        public Framework()
        {            
            
        }
        public void Run()
        {
            bRun = true;
            _FrameworkRoot = new Regulus.Game.FrameworkRoot();

            _Console = new Regulus.Utility.Console();
            _Viewer = new Regulus.Utility.ConsoleViewer();
            var input = new Regulus.Utility.ConsoleInput(_Viewer);
            _Input = input;
            _Console.Initial(_Input, _Viewer);
            _Console.Command.Register("quit", Stop);


            Regulus.Game.StageMachine<Framework> stageMachine = new Regulus.Game.StageMachine<Framework>(this);
            _StageMachine = stageMachine;
            _ToFirst(stageMachine);
                        
            while (bRun)
            {
                input.Update();
                stageMachine.Update();
                _FrameworkRoot.Update();
            }

            _FrameworkRoot.Shutdown();
            _Console.Command.Unregister("quit");
            _Console.Release();

            _StageMachine = null;
            _Viewer = null;
            _Input = null;
            _Console = null;
            _FrameworkRoot = null;
            _User = null;
        }

        private void _ToFirst(Regulus.Game.StageMachine<Framework> stageMachine)
        {
            var stage = new SystemSelectStage();
            stage.RunSystemEvent += _OnLaunchSystem;
            stageMachine.Push(stage);
        }

        void _OnLaunchSystem(SystemSelectStage.System obj)
        {
            
            if (obj == SystemSelectStage.System.Standalong)
            {
                _User = Regulus.Project.Crystal.UserGenerator.BuildStandalong();
                _ToReady();
            }
            else if (obj == SystemSelectStage.System.Remoting)
            {
                _User = Regulus.Project.Crystal.UserGenerator.BuildRemoting();
                _ToReady();
            }
            
            _FrameworkRoot.AddFramework(_User);
            if (UserCreatedEvent != null)
                UserCreatedEvent(_User);

            
        }

        private void _ToReady()
        {
            var stage = new ReadyStage(Command , _Viewer,_User);
            _StageMachine.Push(stage);
        }

        

        private void _ToRemotingSystemLaunch()
        {
            var stage = new RemotingSystemStage();
            _StageMachine.Push(stage);
        }

        public void Stop()
        {
            bRun = false;
        }
    }
}
