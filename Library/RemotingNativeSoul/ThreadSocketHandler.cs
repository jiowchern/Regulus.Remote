using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;


using Regulus.Utility;

namespace Regulus.Remoting.Soul.Native
{
    internal class ThreadSocketHandler
    {
        private readonly ThreadCoreHandler _CoreHandler;

        private readonly PeerSet _Peers;

        private readonly int _Port;

        private readonly Queue<Socket> _Sockets;

        // ParallelUpdate _Peers;
        private readonly PowerRegulator _Spin;

        private readonly AutoPowerRegulator _AutoPowerRegulator;

        private volatile bool _Run;

        private Socket _Socket;

        public int FPS
        {
            get { return _Spin.FPS; }
        }

        public float Power
        {
            get { return _Spin.Power; }
        }

        public int PeerCount
        {
            get { return _Peers.Count; }
        }

        public ThreadSocketHandler(int port, ThreadCoreHandler core_handler)
        {
            _CoreHandler = core_handler;
            _Port = port;

            _Sockets = new Queue<Socket>();

            _Peers = new PeerSet();

            _Spin = new PowerRegulator();
            _AutoPowerRegulator = new AutoPowerRegulator(_Spin);
        }

        public void DoWork(object obj)
        {
            Singleton<Log>.Instance.WriteInfo("server socket launch");
            var are = (AutoResetEvent)obj;
            _Run = true;

            _Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _Socket.NoDelay = true;

            // _Socket.SetSocketOption(System.Net.Sockets.SocketOptionLevel.Socket, System.Net.Sockets.SocketOptionName.ReuseAddress, true);
            _Socket.Bind(new IPEndPoint(IPAddress.Any, _Port));
            _Socket.Listen(5);
            _Socket.BeginAccept(_Accept, null);

            while(_Run)
            {
                if(_Sockets.Count > 0)
                {
                    lock(_Sockets)
                    {
                        while(_Sockets.Count > 0)
                        {
                            var socket = _Sockets.Dequeue();

                            Singleton<Log>.Instance.WriteInfo(
                                string.Format("socket accept Remot {0} Local {1} .", socket.RemoteEndPoint, socket.LocalEndPoint));
                            var peer = new Peer(socket);

                            _Peers.Join(peer);

                            _CoreHandler.Push(peer.Binder, peer.Handler);
                        }
                    }
                }

                _AutoPowerRegulator.Operate();
            }

            _Peers.Release();

            if(_Socket.Connected)
            {
                _Socket.Shutdown(SocketShutdown.Both);
            }

            _Socket.Close();

            are.Set();
            Singleton<Log>.Instance.WriteInfo("server socket shutdown");
        }

        private void _Accept(IAsyncResult ar)
        {
            try
            {
                var socket = _Socket.EndAccept(ar);
                lock(_Sockets)
                {
                    _Sockets.Enqueue(socket);
                }

                _Socket.BeginAccept(_Accept, null);
            }

                // System.ArgumentNullException:
                // asyncResult 為 null。
                // System.ArgumentException:
                // asyncResult 不是透過呼叫 System.Net.Sockets.Socket.BeginAccept(System.AsyncCallback,System.Object)
                // 所建立。
                // System.Net.Sockets.SocketException:
                // 嘗試存取通訊端時發生錯誤。如需詳細資訊，請參閱備註章節。
                // System.ObjectDisposedException:
                // System.Net.Sockets.Socket 已經關閉。
                // System.InvalidOperationException:
                // 先前已呼叫 System.Net.Sockets.Socket.EndAccept(System.IAsyncResult) 方法。
            catch(SocketException se)
            {
                Singleton<Log>.Instance.WriteInfo(se.ToString());
            }
            catch(ObjectDisposedException ode)
            {
                Singleton<Log>.Instance.WriteInfo(ode.ToString());
            }
            catch(InvalidOperationException ioe)
            {
                Singleton<Log>.Instance.WriteInfo(ioe.ToString());
            }
            catch(Exception e)
            {
                Singleton<Log>.Instance.WriteInfo(e.ToString());
            }
        }

        public void Stop()
        {
            _Run = false;
        }
    }
}