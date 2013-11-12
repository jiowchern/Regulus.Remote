using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Remoting
{
    class UserController :  Application.IController
    {
        string _Name;
        Remoting.User _User;
        Utility.Command _Command;
        Utility.Console.IViewer _View;
        private UserCommand _UserCommand;
        public UserController()
        {
            _User = new User();
        }
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

        IUser Regulus.Game.ConsoleFramework<IUser>.IController.User
        {
            get { return _User; }
        }

        void Regulus.Game.ConsoleFramework<IUser>.IController.Release()
        {
            _Command.Unregister("connect");
            _UserCommand.Release();
        }

        void _Connect(string addr)
        {
            _User.ConnectSuccessEvent += _OnConnectSuccess;
            _User.ConnectFailEvent += _OnConnectFail;
            _User.Connect(addr);
            
        }

        void _OnConnectFail(string obj)
        {
            _View.WriteLine("連線失敗: " + obj);
        }

        void _OnConnectSuccess()
        {
            _View.WriteLine("連線成功");
            _Command.Unregister("connect");
            _UserCommand = new UserCommand(_User, _View, _Command);
        }
        void Regulus.Game.ConsoleFramework<IUser>.IController.Initialize(Utility.Console.IViewer view, Utility.Command command)
        {
            command.Register<string>("connect", _Connect);

            _Command = command;
            _View = view;
        }
        

        bool Regulus.Game.IFramework.Update()
        {
            (_User as Regulus.Game.IFramework).Update();
            return true;
        }

        void Regulus.Game.IFramework.Launch()
        {
            (_User as Regulus.Game.IFramework).Launch();
        }

        void Regulus.Game.IFramework.Shutdown()
        {
            (_User as Regulus.Game.IFramework).Shutdown();
        }
    }
}
