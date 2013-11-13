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

        

        void Regulus.Game.ConsoleFramework<IUser>.IController.Release()
        {
            _Command.Unregister("connect");
            _UserCommand.Release();
        }

        void _Connect(string addr)
        {
            _User = new User();
            _User.ConnectSuccessEvent += _OnConnectSuccess;
            _User.ConnectFailEvent += _OnConnectFail;
            try
            {
                _User.Connect(addr);
            }
            catch
            {
                _User.ConnectSuccessEvent -= _OnConnectSuccess;
                _User.ConnectFailEvent -= _OnConnectFail;
            }
            
            
        }

        void _OnConnectFail(string obj)
        {
            _View.WriteLine("連線失敗: " + obj);            
        }

        void _OnConnectSuccess()
        {
            _UserSpawnEvent(_User);
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
            _UserUnpawnEvent(_User);
            (_User as Regulus.Game.IFramework).Shutdown();
        }

        event Regulus.Game.ConsoleFramework<IUser>.OnSpawnUser _UserSpawnEvent;
        event Regulus.Game.ConsoleFramework<IUser>.OnSpawnUser Regulus.Game.ConsoleFramework<IUser>.IController.UserSpawnEvent
        {
            add { _UserSpawnEvent += value;  }
            remove { _UserSpawnEvent -= value ; }
        }
        event Regulus.Game.ConsoleFramework<IUser>.OnUnspawnUser _UserUnpawnEvent;
        event Regulus.Game.ConsoleFramework<IUser>.OnUnspawnUser Regulus.Game.ConsoleFramework<IUser>.IController.UserUnpawnEvent
        {
            add { _UserUnpawnEvent += value; }
            remove { _UserUnpawnEvent -= value; }
        }
    }
}
