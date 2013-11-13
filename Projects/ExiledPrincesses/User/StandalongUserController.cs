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

        public UserController()
        {
            _User = new Standalone.User();
        }
        void Regulus.Game.ConsoleFramework<IUser>.IController.Initialize(Utility.Console.IViewer view, Utility.Command command)
        {
            _View = view;
            _Command = command;
            _UserCommand = new UserCommand(_User, view, command);
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

        void Regulus.Game.ConsoleFramework<IUser>.IController.Release()
        {
            _UserCommand.Release();
        }

        

        void Regulus.Game.IFramework.Launch()
        {
            
            _User.Launch();
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
    }
}
