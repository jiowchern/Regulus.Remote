using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Game
{
    public class ConsoleFramework<TUser> : Regulus.Game.IFramework
        where TUser : Regulus.Game.IFramework
    {

        public interface IController : Regulus.Game.IFramework
        {
            string Name { get; set; }
            event OnSpawnUser UserSpawnEvent;
            event OnUnspawnUser UserUnpawnEvent;
            void Release();

            void Initialize(Utility.Console.IViewer view, Utility.Command command);
        }

        public delegate void BuildCompiled (TUser controller);
        public class ControllerProvider
        {
            public string Command;
            public Func<IController> Spawn;
        }

        bool _Runable = false;

        Regulus.Utility.Console _Console;
        Regulus.Utility.Console.IInput _Input;
        Regulus.Utility.Console.IViewer _Viewer;
        Regulus.Game.StageMachine _StageMachine;
        FrameworkRoot _Loops;        
        ControllerProvider[] _ControllerProviders;
        public Regulus.Utility.Command Command { get { return _Console.Command; } }
        public Regulus.Utility.Console.IViewer Viewer { get { return _Viewer; } }

        public delegate void OnSpawnUser(TUser user);
        public event OnSpawnUser UserSpawnEvent;

        public delegate void OnUnspawnUser(TUser user);
        public event OnUnspawnUser UserUnspawnEvent;        

        public ConsoleFramework(Regulus.Utility.Console.IViewer viewer, Regulus.Utility.Console.IInput input, ControllerProvider[] controller_providers)
        {
            _ControllerProviders = controller_providers;
            _Viewer = viewer;
            _Input = input;
            _Loops = new FrameworkRoot();
            
        }

        class StageSelectSystem : Regulus.Game.IStage
        {
            Regulus.Utility.Console.IViewer _Viewer;
            ControllerProvider[] _SystemProviders;
            Regulus.Utility.Command _Command;
            

            public StageSelectSystem(Regulus.Utility.Console.IViewer viewer, ControllerProvider[] system_provider,Regulus.Utility.Command command)
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
                        if (SelectedEvent != null)
                        {                            
                            SelectedEvent(provider);
                        }
                    });
                }
            }
            
            void IStage.Leave()
            {
                foreach (var provider in _SystemProviders)
                {
                    _Command.Unregister(provider.Command);
                }
                
            }

            void IStage.Update()
            {
                
            }
            public event Action<ControllerProvider> SelectedEvent;
        }
        private StageMachine _CreateStage()
        {
            StageMachine stageMachine = new StageMachine();

            var sss = new StageSelectSystem(_Viewer , _ControllerProviders , Command);
            sss.SelectedEvent += _OnSelectedSystem;
            stageMachine.Push(sss);
            return stageMachine;
        }
        class StageSystemReady : Regulus.Game.IStage
        {
            private Utility.Console.IViewer _Viewer;
            private ControllerProvider _ControllerProvider;
            private Utility.Command _Command;
            Regulus.Game.FrameworkRoot _Loops;
            System.Collections.Generic.List<IController> _Controlls;
            System.Collections.Generic.List<IController> _SelectedControlls;

            public event OnSpawnUser UserSpawnEvent;
            public event OnUnspawnUser UserUnspawnEvent;
            public StageSystemReady(Utility.Console.IViewer view, ControllerProvider controller_provider, Utility.Command command)
            {
             
                this._Viewer = view;
                this._ControllerProvider = controller_provider;
                this._Command = command;
            }
            void IStage.Enter()
            {
                _SelectedControlls = new List<IController>();
                _Controlls = new List<IController>();
                _Loops = new FrameworkRoot();
                _Command.Register<string>("SpawnController", _SpawnController);
                _Command.Register<string>("UnspawnController", _UnspawnController);
                _Command.Register<string>("SelectController", _SelectController);
            }

            private void _SelectController(string name)
            {
                foreach (var controller in _SelectedControlls)
                {
                    controller.Release();                
                }
                _SelectedControlls.Clear();
                _SelectedControlls.AddRange(from controller in _Controlls where controller.Name == name select controller);
                
                foreach (var controller in _SelectedControlls)
                {
                    controller.Initialize(_Viewer, _Command);
                }
                _Viewer.WriteLine("選擇控制者[" + name + "]x" + _SelectedControlls.Count());
            }

            void IStage.Leave()
            {
                _Command.Unregister("UnsawnController");
                _Command.Unregister("SpawnController");
                _Command.Unregister("SelectController");
            }

            void IStage.Update()
            {
                _Loops.Update();   
            }
            
            private void _UnspawnController(string name)
            {
                var controllers = from controller in _Controlls where controller.Name == name select controller;
                foreach (var c in controllers)
                {                    
                    _SelectedControlls.Remove(c);
                    _Loops.RemoveFramework(c);
                    _Controlls.Remove(c);                    
                    _Viewer.WriteLine("控制者[" + name + "] 移除.");
                }

            }            

            void _SpawnController(string name)
            {
                var controller = _ControllerProvider.Spawn();
                
                controller.Name = name;
                
                _Controlls.Add(controller);
                _Loops.AddFramework(controller);
                controller.UserSpawnEvent += UserSpawnEvent;
                controller.UserUnpawnEvent += UserUnspawnEvent;
                
                _Viewer.WriteLine("控制者[" + name + "] 增加.");
            }
            
        }
        
        void _OnSelectedSystem(ConsoleFramework<TUser>.ControllerProvider controller_provider)
        {
            _Viewer.WriteLine("啟動系統");
            var ssr = new StageSystemReady(_Viewer, controller_provider , Command);
            ssr.UserSpawnEvent += UserSpawnEvent;
            ssr.UserUnspawnEvent += UserUnspawnEvent;
            _StageMachine.Push(ssr);
        }

        public void SetLogMessage(Regulus.Utility.Console.LogFilter flag)
        {
            _Console.SetLogFilter(flag);
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
            _Loops.Shutdown();
            _Loops = null;
            _Console.Command.Unregister("quit");
            _Console.Release();

            _StageMachine = null;
            _Viewer = null;
            _Input = null;
            _Console = null;
        }
    }
}
