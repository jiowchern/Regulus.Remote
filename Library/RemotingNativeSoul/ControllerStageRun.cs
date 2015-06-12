﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regulus.Remoting.Native.Soul;


namespace Regulus.Remoting.Soul.Native
{
    
    class ParallelUpdate : Regulus.Utility.Launcher<Regulus.Utility.IUpdatable>
    {
        
        public ParallelUpdate()
        {
            
        }
        public void Update()
        {
            /*var exceptions = new System.Collections.Concurrent.ConcurrentQueue<Exception>();

            Parallel.ForEach(base._GetObjectSet(), (updater) => 
            {
                try
                {
                    _Update(updater);
                }
                catch (Exception e) { exceptions.Enqueue(e); }
            });

            if (exceptions.Count > 0) 
                throw new AggregateException(exceptions);*/
            foreach(var up in base._GetObjectSet())
            {
                _Update(up);
            }
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
        Regulus.Utility.PowerRegulator _Spin;
        
        Regulus.Remoting.ICore _Core;
        Queue<ISoulBinder> _Binders;
       
        volatile bool _Run;
                
        public int FPS { get { return _Spin.FPS; } }

        public float Power { get { return _Spin.Power; } }
        
        public ThreadCoreHandler(Regulus.Remoting.ICore core )
        {
            if (core == null)
            {
                throw new ArgumentNullException();
            }

            _Core = core;

            _RequesterHandlers = new Utility.Updater();
            _Spin = new Utility.PowerRegulator();
            _Binders = new Queue<ISoulBinder>();
        }

        public void DoWork(object obj)
        {
            _Run = true;
            _Core.Launch();

            while (_Run )
            {
                if (_Binders.Count > 0)
                {
                    lock (_Binders)
                    {
                        while (_Binders.Count > 0)
                        {
                            var provider = _Binders.Dequeue();
                            _Core.AssignBinder(provider);
                        }
                    }
                }

                _Core.Update();
                _RequesterHandlers.Working();                
                _Spin.Operate(Peer.TotalResponse);
            }
            _Core.Shutdown();

            
        }

        public void Stop()
        {
            _Run = false;
        }

        internal void Push(ISoulBinder soulBinder , Regulus.Utility.IUpdatable handler)
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

        PeerSet _Peers;
        
        //ParallelUpdate _Peers;

        Regulus.Utility.PowerRegulator _Spin;
        
        int _Port;
        volatile bool _Run;
        
        public int FPS { get { return _Spin.FPS; } }

        public float Power { get { return _Spin.Power; } }

        public int PeerCount { get { return _Peers.Count; } }

        public ThreadSocketHandler(int port , ThreadCoreHandler core_handler)
        {
            _CoreHandler = core_handler;
            _Port = port;
            
            _Sockets = new Queue<System.Net.Sockets.Socket>();

            _Peers = new PeerSet();
            
            _Spin = new Utility.PowerRegulator();
            
        }
        public void DoWork(object obj)
        {            
            _Run = true;

            _Socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
            _Socket.NoDelay = true;
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

                            _Peers.Join(peer);                            

                            _CoreHandler.Push(peer.Binder , peer.Handler);                            
                        }
                    }
                }

                

                _Spin.Operate(Peer.TotalRequest);                
            }
            _Peers.Release();
            
            _Socket.Close();
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

        public void Stop()
        {
            _Run = false;
        }
    }

    class StageRun : Regulus.Utility.IStage
    {
        Regulus.Utility.Command _Command;
        Regulus.Utility.Console.IViewer _View;
        Regulus.Utility.Launcher _Launcher;

        Server _Server;
        
        public event Action ShutdownEvent;
        
        public StageRun(Regulus.Remoting.ICore core, Utility.Command command, int port, Utility.Console.IViewer viewer)
        {            
            _View = viewer;
            this._Command = command;

            _Server = new Server(core, port);
            _Launcher = new Utility.Launcher();
            _Launcher.Push(_Server);
        }

        void Utility.IStage.Enter()
        {
            _Launcher.Launch();
            _Command.Register("FPS", () =>
            {
                _View.WriteLine("PeerFPS:" + _Server.PeerFPS.ToString());
                _View.WriteLine("PeerCount:" + _Server.PeerCount.ToString());
                _View.WriteLine("CoreFPS:" + _Server.CoreFPS.ToString());


                _View.WriteLine(string.Format("PeerPower:{0:0.00%}", _Server.PeerUsage));
                _View.WriteLine(string.Format("CorePower:{0:0.00%}", _Server.CoreUsage));                                

                _View.WriteLine("\nTotalReadBytes:" + string.Format("{0:N0}", NetworkMonitor.Instance.Read.TotalBytes));
                _View.WriteLine("TotalWriteBytes:" + string.Format("{0:N0}", NetworkMonitor.Instance.Write.TotalBytes));
                _View.WriteLine("\nSecondReadBytes:" + string.Format("{0:N0}", NetworkMonitor.Instance.Read.SecondBytes));
                _View.WriteLine("SecondWriteBytes:" + string.Format("{0:N0}", NetworkMonitor.Instance.Write.SecondBytes));
                _View.WriteLine("\nRequest Queue:" + Peer.TotalRequest.ToString());
                _View.WriteLine("Response Queue:" + Peer.TotalResponse.ToString());
            });
            _Command.Register("Shutdown", _ShutdownEvent);
        }

        void Utility.IStage.Leave()
        {
            _Launcher.Shutdown();

            _Command.Unregister("Shutdown");
            _Command.Unregister("FPS");
        }

        void Utility.IStage.Update()
        {

        }
        private void _ShutdownEvent()
        {
            ShutdownEvent();
        }
    }
}
