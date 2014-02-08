using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Remoting.Soul.Native
{
    partial class TcpController : Application.IController
    {
        class StageRun : Regulus.Game.IStage
        {
            public event Action ShutdownEvent;
            
            private Utility.Command _Command;
            Utility.Console.IViewer _View;

            System.Net.Sockets.Socket _Socket;
            Regulus.Game.ICore _Core;
            Regulus.Utility.Updater _Peers;
            int _Port;
            System.Collections.Generic.Queue<System.Net.Sockets.Socket> _Sockets;

            public StageRun(Regulus.Game.ICore core,Utility.Command command,int port , Utility.Console.IViewer viewer)
            {
                _View = viewer;
                this._Command = command;
                _Core = core;
                _Port = port;
                _Socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
                _Sockets = new Queue<System.Net.Sockets.Socket>();
                _Peers = new Utility.Updater();
            }

            void Game.IStage.Enter()
            {

                _Socket.Bind(new System.Net.IPEndPoint(System.Net.IPAddress.Any,_Port) );
                _Socket.Listen((int)System.Net.Sockets.SocketOptionName.MaxConnections);
                _Socket.BeginAccept(_Accept, null);
                _Core.Launch();
                _Command.Register("Shutdown", _ShutdownEvent );
                _Command.Register("ConnectCount", () => { _View.WriteLine("Connect Count:" + _Peers.Objects.Length.ToString()); });
            }

            private void _Accept(IAsyncResult ar)
            {
                var socket = _Socket.EndAccept(ar);
                lock (_Sockets)
                {
                    _Sockets.Enqueue(socket);
                }                
                _Socket.BeginAccept(_Accept, null);
            }

            private void _ShutdownEvent()
            {                
                ShutdownEvent();
            }

            void Game.IStage.Leave()
            {
                _Core.Shutdown();
                _Command.Unregister("Shutdown");
                _Command.Unregister("ConnectCount");
            }

            void Game.IStage.Update()
            {
                if (_Sockets.Count > 0)
                {
                    lock (_Sockets)
                    {
                        var socket = _Sockets.Dequeue();
                        var peer = new Peer(socket);
                        _Peers.Add(peer);
                        _Core.ObtainController(peer.Binder);
                    }
                }
                _Peers.Update();
                _Core.Update();                
            }


            
        }
    }
    
}
