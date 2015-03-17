using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regulus.Remoting.Soul.Native
{
    
    class ParallelUpdate : Regulus.Utility.Launcher<Regulus.Utility.IUpdatable>
    {
        
        public ParallelUpdate()
        {
            
        }
        public void Update()
        {
            Parallel.ForEach(base.Update(), _Update );            
            /*foreach(var up in base.Update())
            {
                _Update(up);
            }*/
        }

        private void _Update(Regulus.Utility.IUpdatable updater)
        {
            bool result = false;
            
            result = updater.Update();

            if (result == false)
            {
                Remove(updater);
            }            
        }
    }

    class ThreadCoreHandler
    {
        Regulus.Utility.Updater _RequesterHandlers;
        volatile bool _Run;
        Regulus.Utility.ICore _Core;
        Queue<ISoulBinder> _Binders;
        Regulus.Utility.FPSCounter _FPS;
        
        public int FPS { get { return _FPS.Value; } }
        public ThreadCoreHandler(Regulus.Utility.ICore core )
        {
            if (core == null)
                throw new ArgumentNullException();


            _Core = core;
            
            _Binders = new Queue<ISoulBinder>();
            _FPS = new Utility.FPSCounter();
            _RequesterHandlers = new Utility.Updater();
            
        }

        public void DoWork(object obj)
        {
            var sw = new System.Threading.SpinWait();
            _Run = true;
            _Core.Launch();
            while (_Run)
            {
                if (_Binders.Count > 0)
                {
                    lock (_Binders)
                    {
                        while (_Binders.Count > 0)
                        {
                            var provider = _Binders.Dequeue();
                            _Core.ObtainController(provider);
                        }
                    }
                    
                }
                _RequesterHandlers.Update();
                _Core.Update();
                _FPS.Update();
                if (Peer.IsIdle )
                    sw.SpinOnce();
                
            }
            _Core.Shutdown();
        }

        public void Stop()
        {
            _Run = false;
        }

        internal void Push(ISoulBinder soulBinder , CoreThreadRequestHandler handler)
        {
            _RequesterHandlers.Add(handler);
            lock (_Binders)
            {
                _Binders.Enqueue(soulBinder);
            }

        }
    }


    class ThreadSocketHandler
    {
        System.Net.Sockets.Socket _Socket;
        System.Collections.Generic.Queue<System.Net.Sockets.Socket> _Sockets;
        ThreadCoreHandler _CoreHandler;
        int _Port;
        volatile bool _Run;
        ParallelUpdate _Peers;
        Regulus.Utility.FPSCounter _FPS;
        
        public int FPS { get { return _FPS.Value; } }
        public int PeerCount { get { return _Peers.Count; } }
        public ThreadSocketHandler(int port , ThreadCoreHandler core_handler)
        {
            _CoreHandler = core_handler;
            _Port = port;
            
            _Sockets = new Queue<System.Net.Sockets.Socket>();
            _Socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
            _Socket.NoDelay = true;

            _Peers = new ParallelUpdate();
            _FPS = new Utility.FPSCounter();
            
        }
        public void DoWork(object obj)
        {
            var sw = new System.Threading.SpinWait();
            _Run = true;

            _Socket.Bind(new System.Net.IPEndPoint(System.Net.IPAddress.Any, _Port));
            
            _Socket.Listen(5);
            _Socket.BeginAccept(_Accept, null);

            while (_Run)
            {
                if (_Sockets.Count > 0)
                {
                    lock (_Sockets)
                    {
                        while (_Sockets.Count > 0)
                        {
                            var socket = _Sockets.Dequeue();
                            var peer = new Peer(socket);
                            _Peers.Add(peer);
                            _CoreHandler.Push(peer.Binder , peer.Handler);                            
                        }
                    }
                }           
     
                
                _Peers.Update();
                _FPS.Update();
                if (Peer.IsIdle)
                    sw.SpinOnce();
            }

            
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
    class StageRun : Regulus.Utility.IStage
    {
        public event Action ShutdownEvent;

        private Utility.Command _Command;
        Utility.Console.IViewer _View;

        ThreadSocketHandler _ThreadSocketHandler;
        //System.Threading.Thread _ThreadSocket;

        ThreadCoreHandler _ThreadCoreHandler;
        //System.Threading.Thread _ThreadCore;
        
        public StageRun(Regulus.Utility.ICore core, Utility.Command command, int port, Utility.Console.IViewer viewer)
        {            
            _View = viewer;
            this._Command = command;

            _ThreadCoreHandler = new ThreadCoreHandler(core);
            
            /*_ThreadCore = new System.Threading.Thread(_ThreadCoreHandler.DoWork);
            _ThreadCore.Priority = System.Threading.ThreadPriority.Normal;
            _ThreadCore.IsBackground = true;*/

            _ThreadSocketHandler = new ThreadSocketHandler(port, _ThreadCoreHandler);
            
            /*_ThreadSocket = new System.Threading.Thread(_ThreadSocketHandler.DoWork);
            _ThreadSocket.Priority = System.Threading.ThreadPriority.Normal;
            _ThreadSocket.IsBackground = true;*/
        }

        void Utility.IStage.Enter()
        {

            _Command.Register("FPS", () =>
            {
                _View.WriteLine("PeerFPS:" + _ThreadSocketHandler.FPS.ToString());
                _View.WriteLine("PeerCount:" + _ThreadSocketHandler.PeerCount.ToString());
                _View.WriteLine("CoreFPS:" + _ThreadCoreHandler.FPS.ToString());
                _View.WriteLine("Read:" + NetworkStreamReadStage.TotalBytesPerSecond.ToString());
                _View.WriteLine("Write:" + NetworkStreamWriteStage.TotalBytesPerSecond.ToString());
                _View.WriteLine("Requests:" + Peer.TotalRequest.ToString());
                _View.WriteLine("Responses:" + Peer.TotalResponse.ToString());

                
            });
            _Command.Register("Shutdown", _ShutdownEvent);

            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(_ThreadCoreHandler.DoWork));
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(_ThreadSocketHandler.DoWork));
            //_ThreadCore.Start();
            //_ThreadSocket.Start();
        }



        private void _ShutdownEvent()
        {
            ShutdownEvent();
        }

        void Utility.IStage.Leave()
        {
            _ThreadCoreHandler.Stop();
            //_ThreadCore.Abort();
            //_ThreadCore.Join();

            _ThreadSocketHandler.Stop();
            //_ThreadSocket.Abort();
            //_ThreadSocket.Join();

            _Command.Unregister("Shutdown");
            _Command.Unregister("FPS");

        }

        void Utility.IStage.Update()
        {

        }



    }
    
}
