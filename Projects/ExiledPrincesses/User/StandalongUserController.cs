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
        

        void Regulus.Framework.ILaunched.Launch()
        {
            
            _User.Launch();
            
        }

		void Regulus.Framework.ILaunched.Shutdown()
        {
            
            
            _User.Shutdown();

        }

        bool Regulus.Utility.IUpdatable.Update()
        {
            if (_UserCommand != null)
                _UserCommand.Update();
            return _User.Update();
        }


        

        void Regulus.Game.ConsoleFramework<IUser>.IController.Look()
        {
            _UserCommand = new UserCommand(_User, _View, _Command);            
        }

        void Regulus.Game.ConsoleFramework<IUser>.IController.NotLook()
        {
            _UserCommand.Release();
            _UserCommand = null;
        }


        


        IUser Regulus.Game.ConsoleFramework<IUser>.IController.GetUser()
        {
            throw new NotImplementedException();
        }
    }
}
