using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Regulus.Extension;
namespace Regulus.Game
{
    

    public abstract partial class ConsoleFramework<TUser> : Regulus.Utility.IUpdatable
		where TUser : Regulus.Utility.IUpdatable
    {

		public interface IController : Regulus.Utility.IUpdatable
        {
            string Name { get; set; }
            event OnSpawnUser UserSpawnEvent;
            event OnSpawnUserFail UserSpawnFailEvent;
            event OnUnspawnUser UserUnpawnEvent;

            void Look();
            void NotLook();
        }

        public delegate void BuildCompiled (TUser controller);
        public class ControllerProvider
        {
            public string Command;
            public Func<IController> Spawn;
        }

        bool _Runable = false;

        Regulus.Utility.Console _Console;
        protected Regulus.Utility.Console.IInput _Input;
        protected Regulus.Utility.Console.IViewer _Viewer;
        Regulus.Game.StageMachine _StageMachine;
		Regulus.Utility.Updater<Regulus.Utility.IUpdatable> _Loops;        
        
        public Regulus.Utility.Command Command { get { return _Console.Command; } }
        public Regulus.Utility.Console.IViewer Viewer { get { return _Viewer; } }

        public delegate void OnSpawnUser(TUser user);
        public event OnSpawnUser UserSpawnEvent;

        public delegate void OnSpawnUserFail(string message);
        public event OnSpawnUserFail UserSpawnFailEvent;

        public delegate void OnUnspawnUser(TUser user);
        public event OnUnspawnUser UserUnspawnEvent;
        protected abstract ControllerProvider[] _ControllerProvider();

        public ConsoleFramework(Regulus.Utility.Console.IViewer viewer, Regulus.Utility.Console.IInput input)
        {
            
            _Viewer = viewer;
            _Input = input;
            _Loops = new Regulus.Utility.Updater<Regulus.Utility.IUpdatable>();
			_Console = new Regulus.Utility.Console(_Input, _Viewer);
        }
        
        
        private StageMachine _CreateStage()
        {
            StageMachine stageMachine = new StageMachine();
            var sss = new StageSelectSystem(_Viewer, _ControllerProvider(), Command);
            sss.SelectSystemEvent += SelectSystemEvent;
            sss.SelectedEvent += _OnSelectedSystem;
            stageMachine.Push(sss);
            return stageMachine;
        }

        public event OnUserRequester UserRequesterEvent;

        IUserRequester _OnSelectedSystem(ConsoleFramework<TUser>.ControllerProvider controller_provider)
        {
            _Viewer.WriteLine("啟動系統");
            var ssr = new StageSystemReady(_Viewer, controller_provider , Command);
            ssr.UserSpawnEvent += UserSpawnEvent;
            ssr.UserSpawnFailEvent += UserSpawnFailEvent;
            ssr.UserRequesterEvent += UserRequesterEvent;            
            ssr.UserUnspawnEvent += UserUnspawnEvent;
            _StageMachine.Push(ssr);

            return ssr;
        }



        public void SetLogMessage(Regulus.Utility.Console.LogFilter flag)
        {
            _Console.SetLogFilter(flag);
        }
        public void Stop()
        {
            _Runable = false;

        }


		void Regulus.Framework.ILaunched.Launch()
        {
            _Runable = true;

            _StageMachine = _CreateStage();
        }

		bool Regulus.Utility.IUpdatable.Update()
        {
            _Loops.Update();
            _StageMachine.Update();
            return _Runable;
        }

		void Regulus.Framework.ILaunched.Shutdown()
        {
            _Loops.Shutdown();
            _Loops = null;
        
            _StageMachine = null;
            _Viewer = null;
            _Input = null;
            _Console = null;
        }
    }


    public abstract partial class ConsoleFramework<TUser>
    {
        public event OnSelectSystem SelectSystemEvent;
        public delegate void OnSelectSystem(ISystemSelector system_selector);

        public interface ISystemSelector
        {
            Regulus.Remoting.Value<IUserRequester> Use(string system);
        }
        

        class StageSelectSystem : Regulus.Game.IStage, ISystemSelector
        {
            class SystemSelector : ISystemSelector
            {

                WeakReference _SystemSelector;
                
                public SystemSelector(ISystemSelector system_selector)
                {
                    _SystemSelector = new WeakReference(system_selector);
                }
                Regulus.Remoting.Value<IUserRequester> ISystemSelector.Use(string system)
                {
                    if (_SystemSelector != null && _SystemSelector.IsAlive)
                    {
                        
                        var val =  (_SystemSelector.Target as ISystemSelector).Use(system);
                        _SystemSelector = null;
                        return val;
                    }
                    return null;
                }
            }
            Regulus.Utility.Console.IViewer _Viewer;
            ControllerProvider[] _SystemProviders;
            Regulus.Utility.Command _Command;

            public event OnSelectSystem SelectSystemEvent;
            public StageSelectSystem(Regulus.Utility.Console.IViewer viewer, ControllerProvider[] system_provider, Regulus.Utility.Command command)
            {
                SelectSystemEvent = (system_selector) => { };
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

                SelectSystemEvent(new SystemSelector(this));
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
            public event Func<ControllerProvider, IUserRequester> SelectedEvent;

            Regulus.Remoting.Value<IUserRequester> ISystemSelector.Use(string system)
            {
                var p = (from provider in _SystemProviders where provider.Command == system select provider).FirstOrDefault();
                if (p != null)
                {
                    return new Regulus.Remoting.Value<IUserRequester>(SelectedEvent(p));
                }
                else
                {
                    _Viewer.WriteLine("錯誤的系統名稱.");
                }
                return null;
            }
        }
    }

    public abstract partial class ConsoleFramework<TUser>
    {
        public delegate void OnUserRequester(IUserRequester user_requester);
        public interface IUserRequester
        {
            Regulus.Remoting.Value<TUser> Spawn(string name ,  bool look);
            void Unspawn(string name);
        }

        public class UserRequester : IUserRequester
        {
            WeakReference _UserRequester;
            public UserRequester(IUserRequester user_requester)
            {
                _UserRequester = new WeakReference(user_requester);
            }
            Regulus.Remoting.Value<TUser> IUserRequester.Spawn(string name, bool look)
            {
                if (_UserRequester.IsAlive)
                {
                    return (_UserRequester.Target as IUserRequester).Spawn(name, look);                    
                }
                return null;
            }

            void IUserRequester.Unspawn(string name)
            {
                if (_UserRequester.IsAlive)
                {
                    (_UserRequester.Target as IUserRequester).Unspawn(name);
                }
            }
        }

        class StageSystemReady : Regulus.Game.IStage, IUserRequester
        {
            private Utility.Console.IViewer _Viewer;
            private ControllerProvider _ControllerProvider;
            private Utility.Command _Command;
			Regulus.Utility.Updater<Regulus.Utility.IUpdatable> _Loops;
            System.Collections.Generic.List<IController> _Controlls;
            System.Collections.Generic.List<IController> _SelectedControlls;

            public event OnSpawnUser UserSpawnEvent;
            public event OnSpawnUserFail UserSpawnFailEvent;
            public event OnUnspawnUser UserUnspawnEvent;
            public event OnUserRequester UserRequesterEvent;
            public StageSystemReady(Utility.Console.IViewer view, ControllerProvider controller_provider, Utility.Command command)
            {
                UserRequesterEvent = (user_requester) => { };
                this._Viewer = view;
                this._ControllerProvider = controller_provider;
                this._Command = command;

                _SelectedControlls = new List<IController>();
                _Controlls = new List<IController>();
                _Loops = new Regulus.Utility.Updater<Regulus.Utility.IUpdatable>();
            }
            void IStage.Enter()
            {


                _Command.RemotingRegister<string, TUser>("SpawnController", _SpawnController, (user) => { });                
                _Command.Register<string>("SelectController", _SelectController);
                _Command.Register<string>("UnspawnController", _UnspawnController);                
                
                UserRequesterEvent(new UserRequester(this));
            }

            void IStage.Leave()
            {
                _Command.Unregister("SelectController");
                _Command.Unregister("UnsawnController");
                _Command.Unregister("SpawnController");                
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
					_Loops.Remove(c);                    
                    _Controlls.Remove(c);
                    _Viewer.WriteLine("控制者[" + name + "] 移除.");
                }

            }

            Regulus.Remoting.Value<TUser> _SpawnController(string name)
            {
                var value = new Regulus.Remoting.Value<TUser>();
                var controller = _ControllerProvider.Spawn();

                controller.Name = name;

                _Controlls.Add(controller);
				_Loops.Add(controller);                
                controller.UserSpawnEvent += (user)=>
                {
                    if (UserSpawnEvent != null)
                        UserSpawnEvent(user);
                    value.SetValue(user);
                };
                controller.UserSpawnFailEvent += UserSpawnFailEvent;
                controller.UserUnpawnEvent += UserUnspawnEvent;

                _Viewer.WriteLine("控制者[" + name + "] 增加.");
                return value;
            }

            private void _SelectController(string name)
            {
                foreach (var controller in _SelectedControlls)
                {
                    controller.NotLook();
                }
                _SelectedControlls.Clear();
                _SelectedControlls.AddRange(from controller in _Controlls where controller.Name == name select controller);

                foreach (var controller in _SelectedControlls)
                {
                    controller.Look();
                }
                _Viewer.WriteLine("選擇控制者[" + name + "]x" + _SelectedControlls.Count());
            }

            Regulus.Remoting.Value<TUser> IUserRequester.Spawn(string name, bool look)
            {
                var val = _SpawnController(name);
                if (look)
                    _SelectController(name);
                return val;
            }

            void IUserRequester.Unspawn(string name)
            {
                _UnspawnController(name);
            }
        }
    }
}
