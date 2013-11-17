using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Standalong
{
    class UserController : Application.IController
    {

        Utility.Console.IViewer _View;
        Utility.Command _Command;
        Regulus.Project.ExiledPrincesses.Standalone.User _User;
        UserCommand _UserCommand;

        public UserController(Utility.Console.IViewer view, Utility.Command command)
        {
            _User = new Standalone.User();
            _View = view;
            _Command = command;
            
        }
        

        string _Name;
        string Regulus.Game.ConsoleFramework<IUser>.IController.Name
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
        

        void Regulus.Game.IFramework.Launch()
        {
            
            _User.Launch();
            if (_UserSpawnEvent != null)
                _UserSpawnEvent(_User);
        }

        void Regulus.Game.IFramework.Shutdown()
        {
            
            _UserUnpawnEvent(_User);            
            _User.Shutdown();

        }

        bool Regulus.Game.IFramework.Update()
        {
            return _User.Update();
        }


        event Regulus.Game.ConsoleFramework<IUser>.OnSpawnUser _UserSpawnEvent;
        event Regulus.Game.ConsoleFramework<IUser>.OnSpawnUser Regulus.Game.ConsoleFramework<IUser>.IController.UserSpawnEvent
        {
            add { _UserSpawnEvent += value; }
            remove { _UserSpawnEvent -= value; }
        }
        event Regulus.Game.ConsoleFramework<IUser>.OnUnspawnUser _UserUnpawnEvent;
        event Regulus.Game.ConsoleFramework<IUser>.OnUnspawnUser Regulus.Game.ConsoleFramework<IUser>.IController.UserUnpawnEvent
        {
            add { _UserUnpawnEvent += value; }
            remove { _UserUnpawnEvent -= value; }
        }


        void Regulus.Game.ConsoleFramework<IUser>.IController.Look()
        {
            _UserCommand = new UserCommand(_User, _View, _Command);            
        }

        void Regulus.Game.ConsoleFramework<IUser>.IController.NotLook()
        {
            _UserCommand.Release();
        }


        event Regulus.Game.ConsoleFramework<IUser>.OnSpawnUserFail Regulus.Game.ConsoleFramework<IUser>.IController.UserSpawnFailEvent
        {
            add {  }
            remove {  }
        }
    }
}
