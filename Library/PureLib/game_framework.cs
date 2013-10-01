using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Game
{
    public class ConsoleFramework<TSystem> : Regulus.Game.IFramework
        where TSystem : Regulus.Game.IFramework
    { 

        public delegate void BuildCompiled (TSystem system);
        public class SystemProvider
        {
            public string Command;
            public Action<BuildCompiled , Regulus.Game.StageMachine> Build;
        }

        bool _Runable = false;

        Regulus.Utility.Console _Console;
        Regulus.Utility.Console.IInput _Input;
        Regulus.Utility.Console.IViewer _Viewer;
        Regulus.Game.StageMachine _StageMachine;
        FrameworkRoot _Loops;        
        SystemProvider[] _SystemProviders;
        public Regulus.Utility.Command Command { get { return _Console.Command; } }
        public Regulus.Utility.Console.IViewer Viewer { get { return _Viewer; } }

        public ConsoleFramework(Regulus.Utility.Console.IViewer viewer, Regulus.Utility.Console.IInput input, SystemProvider[] system_providers, Regulus.Game.IFramework[] frameworks)
        {
            _SystemProviders = system_providers;
            _Viewer = viewer;
            _Input = input;
            _Loops = new FrameworkRoot();
            foreach (var framework in frameworks)
            {
                _Loops.AddFramework(framework);
            }
        }

        class StageSelectSystem : Regulus.Game.IStage
        {
            Regulus.Utility.Console.IViewer _Viewer;
            SystemProvider[] _SystemProviders;
            Regulus.Utility.Command _Command;
            Regulus.Game.StageMachine _StageMachine;

            public StageSelectSystem(Regulus.Utility.Console.IViewer viewer, SystemProvider[] system_provider,Regulus.Utility.Command command)
            {
                _Viewer = viewer;
                _SystemProviders = system_provider;
                _Command = command;
            }
            void IStage.Enter()
            {

                _Viewer.WriteLine("選擇系統");
                foreach (var provider in _SystemProviders)
                {
                    _Viewer.WriteLine(provider.Command);
                    _Command.Register(provider.Command, () => 
                    {
                        _StageMachine = new StageMachine();
                        provider.Build(_BuildCompiled , _StageMachine ); 
                    });
                }
            }
            void _BuildCompiled(TSystem system)
            {
                if (SelectedEvent != null)
                    SelectedEvent(system);
            }
            void IStage.Leave()
            {
                foreach (var provider in _SystemProviders)
                {
                    _Command.Unregister(provider.Command);
                }
                _StageMachine.Termination();
            }

            void IStage.Update()
            {
                if (_StageMachine != null)
                    _StageMachine.Update();
            }
            public event Action<TSystem> SelectedEvent;
        }
        private StageMachine _CreateStage()
        {
            StageMachine stageMachine = new StageMachine();

            var sss = new StageSelectSystem(_Viewer , _SystemProviders , Command);
            sss.SelectedEvent += _OnSelectedSystem;
            stageMachine.Push(sss);
            return stageMachine;
        }
        public delegate void OnSystemCreated(TSystem system);
        public event OnSystemCreated SystemCreatedEvent;
        void _OnSelectedSystem(TSystem system)
        {
            _Loops.AddFramework(system);
            SystemCreatedEvent(system);
            _StageMachine.Push(null);
        }

        public void Stop()
        {
            _Runable = false;
        }

        void IFramework.Launch()
        {
            _Runable = true;
            _Console = new Regulus.Utility.Console();
            _Console.Initial(_Input, _Viewer);
            _Console.Command.Register("quit", Stop);

            _StageMachine = _CreateStage();
        }

        bool IFramework.Update()
        {
            _Loops.Update();
            _StageMachine.Update();
            return _Runable;
        }

        void IFramework.Shutdown()
        {
            _Console.Command.Unregister("quit");
            _Console.Release();

            _StageMachine = null;
            _Viewer = null;
            _Input = null;
            _Console = null;
        }
    }
}
