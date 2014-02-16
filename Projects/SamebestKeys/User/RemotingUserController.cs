using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regulus.Projects.SamebestKeys;

namespace Regulus.Project.SamebestKeys.Remoting
{
    class UserController : Regulus.Project.SamebestKeys.Console.IController
    {
        Regulus.Utility.Updater<Regulus.Utility.IUpdatable> _Updater;
        private Utility.Console.IViewer _Viewer;
        private Utility.Command _Command;
        Regulus.Project.SamebestKeys.IUser _User;

        UserCommand _UserCommand;
        public UserController(Utility.Console.IViewer view, Utility.Command command)
        {            
            this._Viewer = view;
            this._Command = command;
            _UserCommand = new UserCommand(view, command);
            _User = new Regulus.Projects.SamebestKeys.Remoting.RemotingUser();
            _Updater = new Utility.Updater<Utility.IUpdatable>();
        }


        string _Name;
        string Game.ConsoleFramework<Project.SamebestKeys.IUser>.IController.Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }

        event Game.ConsoleFramework<Project.SamebestKeys.IUser>.OnSpawnUser _UserSpawnEvent;
        event Game.ConsoleFramework<Project.SamebestKeys.IUser>.OnSpawnUser Game.ConsoleFramework<Project.SamebestKeys.IUser>.IController.UserSpawnEvent
        {
            add { _UserSpawnEvent += value; }
            remove { _UserSpawnEvent -= value; }
        }

        
        event Game.ConsoleFramework<Project.SamebestKeys.IUser>.OnSpawnUserFail Game.ConsoleFramework<Project.SamebestKeys.IUser>.IController.UserSpawnFailEvent
        {
            add {  }
            remove {  }
        }

        event Game.ConsoleFramework<Project.SamebestKeys.IUser>.OnUnspawnUser _UserUnpawnEvent;
        event Game.ConsoleFramework<Project.SamebestKeys.IUser>.OnUnspawnUser Game.ConsoleFramework<Project.SamebestKeys.IUser>.IController.UserUnpawnEvent
        {
            add { _UserUnpawnEvent += value; }
            remove { _UserUnpawnEvent -= value; }
        }
        
        void Game.ConsoleFramework<Project.SamebestKeys.IUser>.IController.Look()
        {
            _UserCommand.Register(_User);
        }

        void Game.ConsoleFramework<Project.SamebestKeys.IUser>.IController.NotLook()
        {
            _UserCommand.Unregister(_User);
        }

        bool Utility.IUpdatable.Update()
        {
            _Updater.Update();
            return true;
        }

        void Framework.ILaunched.Launch()
        {
            if (_UserSpawnEvent != null)
                _UserSpawnEvent(_User);

            _Updater.Add(_User);
        }

        void Framework.ILaunched.Shutdown()
        {
            if (_UserUnpawnEvent != null)
                _UserUnpawnEvent(_User);

            _Updater.Shutdown();
        }
    }
}
