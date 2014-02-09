using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestNativeUser
{
    using Regulus.Extension;
    class RemotingController : Application.IController
    {
        IUser _User;
        Regulus.Utility.Updater<Regulus.Utility.IUpdatable> _Updater;
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

        event Regulus.Game.ConsoleFramework<IUser>.OnSpawnUser _UserSpawnEvent;
        event Regulus.Game.ConsoleFramework<IUser>.OnSpawnUser Regulus.Game.ConsoleFramework<IUser>.IController.UserSpawnEvent
        {
            add { _UserSpawnEvent += value; }
            remove { _UserSpawnEvent -= value; }
        }

        event Regulus.Game.ConsoleFramework<IUser>.OnSpawnUserFail _UserSpawnFailEvent;
        event Regulus.Game.ConsoleFramework<IUser>.OnSpawnUserFail Regulus.Game.ConsoleFramework<IUser>.IController.UserSpawnFailEvent
        {
            add { _UserSpawnFailEvent+= value; }
            remove { _UserSpawnFailEvent-= value; }
        }

        event Regulus.Game.ConsoleFramework<IUser>.OnUnspawnUser _UserUnpawnEvent;
        event Regulus.Game.ConsoleFramework<IUser>.OnUnspawnUser Regulus.Game.ConsoleFramework<IUser>.IController.UserUnpawnEvent
        {
            add { _UserUnpawnEvent += value; }
            remove { _UserUnpawnEvent -= value; }
        }
        public RemotingController(Regulus.Utility.Command command , Regulus.Utility.Console.IViewer view)
        {
            _View = view;
            _User = new User();
            _Updater = new Regulus.Utility.Updater<Regulus.Utility.IUpdatable>();

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
            if (_UserSpawnEvent != null)
                _UserSpawnEvent(_User);
            _Updater.Add(_User);
        }

        void Regulus.Framework.ILaunched.Shutdown()
        {
            _UserUnpawnEvent(_User);
            _Updater.Remove(_User);
        }
    }
}

