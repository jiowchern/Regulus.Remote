using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TestNativeUser
{
    using Regulus.Extension;
    class RemotingController : Application.IController
    {
        IUser _User;
        Regulus.Utility.Updater _Updater;
        Regulus.Utility.Command _Command;
        string _Name;
        Regulus.Utility.Console.IViewer _View;
        string Regulus.Game.ConsoleFramework<IUser>.IController.Name
        {
            get
            {
                return _Name  ;
            }
            set
            {
                _Name = value;
            }
        }

        
        public RemotingController(Regulus.Utility.Command command , Regulus.Utility.Console.IViewer view)
        {
            _View = view;
            _User = new User();
            _Updater = new Regulus.Utility.Updater();

            _Command = command;
        }
        void Regulus.Game.ConsoleFramework<IUser>.IController.Look()
        {
            _User.ConnectProvider.Supply += ConnectProvider_Supply;
            _User.MessagerProvider.Supply += MessagerProvider_Supply;
        }

        void MessagerProvider_Supply(TestNativeGameCore.IMessager obj)
        {
            _Command.RemotingRegister<string, string>("Send", obj.Send, (result) => { _View.WriteLine(result); });                       
        }

        void ConnectProvider_Supply(TestNativeGameCore.IConnect connect)
        {
            _Command.RemotingRegister<string, int, bool>("Connect", connect.Connect, (result) => { _View.WriteLine(result.ToString()); });                       
        }

        void Regulus.Game.ConsoleFramework<IUser>.IController.NotLook()
        {
            _Command.Unregister("Connect");
            _Command.Unregister("Send");

            _User.ConnectProvider.Supply -= ConnectProvider_Supply;
            _User.MessagerProvider.Supply -= MessagerProvider_Supply;
        }

        bool Regulus.Utility.IUpdatable.Update()
        {
            _Updater.Update();
            return true;
        }

        void Regulus.Framework.ILaunched.Launch()
        {
            
            _Updater.Add(_User);
        }

        void Regulus.Framework.ILaunched.Shutdown()
        {
            
            _Updater.Remove(_User);
        }


        IUser Regulus.Game.ConsoleFramework<IUser>.IController.GetUser()
        {
            throw new NotImplementedException();
        }
    }
}

