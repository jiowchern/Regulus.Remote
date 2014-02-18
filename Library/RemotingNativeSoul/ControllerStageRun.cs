using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Remoting.Soul.Native
{

    class ThreadSocketHandler
    {
        System.Net.Sockets.Socket _Socket;
        System.Collections.Generic.Queue<System.Net.Sockets.Socket> _Sockets;
        Regulus.Game.ICore _Core;
        int _Port;
        volatile bool _Run;
        Regulus.Utility.Updater _Peers;
        Regulus.Utility.FPSCounter _FPS;


        public float CoreTimeCounter { get; private set; }
        public float PeerTimeCounter { get; private set; }
        public int FPS { get { return _FPS.Value; } }
        public int PeerCount { get { return _Peers.Count; } }
        public ThreadSocketHandler(int port , Regulus.Game.ICore core)
        {
            _Port = port;
            _Core = core;
            _Sockets = new Queue<System.Net.Sockets.Socket>();
            _Socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
            _Peers = new Utility.Updater();
            _FPS = new Utility.FPSCounter();
            
        }
        public void DoWork()
        {
            _Run = true;

            Regulus.Utility.TimeCounter coreTimeCounter = new Utility.TimeCounter();
            Regulus.Utility.TimeCounter peerTimeCounter = new Utility.TimeCounter();

            _Socket.Bind(new System.Net.IPEndPoint(System.Net.IPAddress.Any, _Port));
            //_Socket.Listen((int)System.Net.Sockets.SocketOptionName.MaxConnections);
            _Socket.Listen(5);
            _Socket.BeginAccept(_Accept, null);
            _Core.Launch();
            

            while (_Run)
            {
                while (_Sockets.Count > 0)
                {
                    lock (_Sockets)
                    {
                        var socket = _Sockets.Dequeue();
                        var peer = new Peer(socket);
                        _Peers.Add(peer);
                        _Core.ObtainController(peer.Binder);
                    }
                }
                peerTimeCounter.Reset();
                _Peers.Update();
                PeerTimeCounter = peerTimeCounter.Second;

                coreTimeCounter.Reset();
                _Core.Update();
                CoreTimeCounter = coreTimeCounter.Second;

                _FPS.Update();
                System.Threading.Thread.Sleep(0);
            }

            _Core.Shutdown();
        }

        public void Stop()
        {
            _Run = false;
        }
        private void _Accept(IAsyncResult ar)
        {
            try
            {
                var socket = _Socket.EndAccept(ar);
                lock (_Sockets)
                {
                    _Sockets.Enqueue(socket);
                }
                _Socket.BeginAccept(_Accept, null);
            }
            catch
            {

            }
        }

    }
    partial class TcpController : Application.IController
    {
        class StageRun : Regulus.Game.IStage
        {
            public event Action ShutdownEvent;
            
            private Utility.Command _Command;
            Utility.Console.IViewer _View;

            ThreadSocketHandler _ThreadSocketHandler;
            System.Threading.Thread _ThreadSocket;

            public StageRun(Regulus.Game.ICore core,Utility.Command command,int port , Utility.Console.IViewer viewer)
            {
                _View = viewer;
                this._Command = command;
                _ThreadSocketHandler = new ThreadSocketHandler(port, core);
                _ThreadSocket = new System.Threading.Thread(_ThreadSocketHandler.DoWork);
            }

            void Game.IStage.Enter()
            {

                _Command.Register("FPS", () => 
                { 
                    _View.WriteLine("FPS:" + _ThreadSocketHandler.FPS.ToString());
                    _View.WriteLine("Core:" + _ThreadSocketHandler.CoreTimeCounter.ToString());
                    _View.WriteLine("Peer:" + _ThreadSocketHandler.PeerTimeCounter.ToString()); 
                }  );
                _Command.Register("Shutdown", _ShutdownEvent );
                _Command.Register("PeerCount", 
                    () => 
                    {
                        _View.WriteLine("Connect Count:" + _ThreadSocketHandler.PeerCount.ToString() ); 
                    });
                
                _ThreadSocket.Start();
            }

            

            private void _ShutdownEvent()
            {                
                ShutdownEvent();
            }

            void Game.IStage.Leave()
            {
                _ThreadSocketHandler.Stop();
                _ThreadSocket.Abort();
                _ThreadSocket.Join();
                _Command.Unregister("Shutdown");
                _Command.Unregister("FPS");
                _Command.Unregister("PeerCount");
            }

            void Game.IStage.Update()
            {
                
                
            }


            
        }
    }
    
}
