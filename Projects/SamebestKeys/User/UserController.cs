using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regulus.Projects.SamebestKeys;

namespace Regulus.Project.SamebestKeys
{
    class UserController : Regulus.Project.SamebestKeys.Console.IController
    {
        Regulus.Utility.Updater _Updater;
        private Utility.Console.IViewer _Viewer;
        private Utility.Command _Command;
        Regulus.Project.SamebestKeys.IUser _User;

        UserCommand _UserCommand;
        public UserController(Utility.Console.IViewer view, Utility.Command command , Regulus.Project.SamebestKeys.IUser user)
        {            
            this._Viewer = view;
            this._Command = command;
            _UserCommand = new UserCommand(view, command);
            _User = user;
            _Updater = new Utility.Updater();
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
            

            _Updater.Add(_User);
        }

        void Framework.ILaunched.Shutdown()
        {            

            _Updater.Shutdown();
        }


        IUser Game.ConsoleFramework<IUser>.IController.GetUser()
        {
            return _User;
        }
    }
}
