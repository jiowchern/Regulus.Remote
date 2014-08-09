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
        bool _Look;
        bool _Linked;
        public UserController(Utility.Console.IViewer view, Utility.Command command)
        {            
            _User = new User();
            _Command = command;
            _View = view;
            _Look = false;
            _Linked = false;
        }
        string Regulus.Game.Framework<IUser>.IController.Name
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

        void _Connect(string addr)
        {
            _Linked = false;
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
            _View.WriteLine("連線成功");            
            
            _Linked = true;
            if (_Look)
                _UserCommand = new UserCommand(_User, _View, _Command);            
        }
        
        

        bool Regulus.Utility.IUpdatable.Update()
        {
            if (_Look && _UserCommand != null)
                _UserCommand.Update();
			(_User as Regulus.Utility.IUpdatable).Update();
            return true;
        }

		void Regulus.Framework.ILaunched.Launch()
        {
			(_User as Regulus.Framework.ILaunched).Launch();            
            
        }

		void Regulus.Framework.ILaunched.Shutdown()
        {
            
			(_User as Regulus.Framework.ILaunched).Shutdown();
                        
        }
        


        void Regulus.Game.Framework<IUser>.IController.Look()
        {
            _Command.Register<string>("Connect", _Connect);
            if (_UserCommand != null)
                _UserCommand.Release();
            _Look = true;
            if (_Linked)
                _UserCommand = new UserCommand(_User, _View, _Command);
        }

        void Regulus.Game.Framework<IUser>.IController.NotLook()
        {
            _Command.Unregister("Connect");
            if (_UserCommand != null)
                _UserCommand.Release();

            _Look = false;
        }





        IUser Regulus.Game.Framework<IUser>.IController.GetUser()
        {
            throw new NotImplementedException();
        }
    }
}
