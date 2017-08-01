using System.Net;
using Microsoft.SqlServer.Server;
using Regulus.Framework;
using Regulus.Network.Rudp;
using Regulus.Utility;

namespace Regulus.Network.Tests.TestTool
{
    internal class ClientHandler : IUpdatable
    {
        private readonly int _Id;
        
        private readonly IPeerClient _Client;
        private readonly Command _Command;
        private readonly Console.IViewer _Viewer;

        private readonly Regulus.Utility.Updater _Updater;

        public ClientHandler(int id, IPeerClient client, Command command, Console.IViewer viewer)
        {
            _Id = id;
            _Client = client;
            _Command = command;
            _Viewer = viewer;
            _Updater = new Updater();
        }

        void IBootable.Launch()
        {
            _Client.Launch();

            _Command.Register<string,int>("Connect" + _Id , _Connect);
        }

        void IBootable.Shutdown()
        {
            _Updater.Shutdown();
            _Command.Unregister("Connect" + _Id);
            _Client.Shutdown();
        }

        bool IUpdatable.Update()
        {
            _Updater.Working();
            return true;
        }

        void _Connect(string ip,int port)
        {

            var socket = _Client.Spawn();
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(ip),port );
            socket.Connect(endpoint , (result) =>
            {
                if (result)
                {
                    _Updater.Add(new PeerHandler(_Command , _Viewer , socket));
                }                
            });
        }
    }
}