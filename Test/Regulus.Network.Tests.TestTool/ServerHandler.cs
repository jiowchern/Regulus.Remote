using System;
using Regulus.Framework;
using Regulus.Utility;
using Console = Regulus.Utility.Console;

namespace Regulus.Network.Tests.TestTool
{
    internal class ServerHandler : IUpdatable
    {
        private readonly int _Id;
        private readonly IServer _Server;
        private readonly Command _Command;
        private readonly Console.IViewer _Viewer;

        private readonly Regulus.Utility.Updater _Updater;
        private bool _Enable;

        public ServerHandler(int id,IServer server, Command command, Console.IViewer viewer)
        {
            
            _Id = id;
            _Server = server;
            _Command = command;
            _Viewer = viewer;
            _Updater = new Updater();
        }

        void IBootable.Launch()
        {
            _Enable = true;
            _Server.AcceptEvent += _AcceptPeer;
            _Command.Register<int>("Bind" + _Id, Bind);
            _Command.Register("Close" + _Id, Close );

            
        }

        void IBootable.Shutdown()
        {
            
            _Server.AcceptEvent -= _AcceptPeer;
            _Updater.Shutdown();
            _Command.Unregister("Bind" + _Id);
            _Command.Unregister("Close" + _Id);
            
        }

        private void _AcceptPeer(IPeer obj)
        {
            _Updater.Add( new PeerHandler(_Command , _Viewer , obj));
        }

        bool IUpdatable.Update()
        {
            _Updater.Working();
            return _Enable;
        }

        public void Bind(int port)
        {
            _Server.Bind(port);           
        }

        public void Close()
        {
            _Server.Close();
            _Enable = false;
        }

        
    }
}