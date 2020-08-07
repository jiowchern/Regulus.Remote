using Regulus.Utility;
using System.Collections.Generic;
using System.Net;

namespace Regulus.Network
{
    public class Agent : IUpdatable<Timestamp>
    {

        private readonly ISocketRecevieable m_SocketRecevieable;
        private readonly ISocketSendable m_SocketSendable;


        private readonly Dictionary<EndPoint, Socket> m_Peers;
        private readonly List<Socket> m_RemovePeers;
        private readonly Updater<Timestamp> m_Updater;
        private readonly WiringOperator m_WiringOperator;

        public Agent(ISocketRecevieable SocketRecevieable, ISocketSendable SocketSendable)
        {
            m_SocketRecevieable = SocketRecevieable;
            m_SocketSendable = SocketSendable;
            m_Updater = new Updater<Timestamp>();
            m_RemovePeers = new List<Socket>();
            m_WiringOperator = new WiringOperator(m_SocketSendable, m_SocketRecevieable, false);
            m_Peers = new Dictionary<EndPoint, Socket>();
        }

        bool IUpdatable<Timestamp>.Update(Timestamp Timestamp)
        {
            Socket[] removePeers = GetRemovePeers();
            int count = removePeers.Length;
            for (int i = 0; i < count; i++)
            {
                Socket peer = removePeers[i];
                lock (m_WiringOperator)
                {
                    m_WiringOperator.Destroy(peer.EndPoint);
                }
            }

            m_Updater.Working(Timestamp);
            return true;
        }

        private Socket[] GetRemovePeers()
        {
            lock (m_RemovePeers)
            {
                Socket[] cloned = m_RemovePeers.ToArray();
                m_RemovePeers.Clear();
                return cloned;
            }
        }

        void IBootable.Launch()
        {



            m_WiringOperator.JoinStreamEvent += CreatePeer;
            m_WiringOperator.LeftStreamEvent += DestroyPeer;
            m_Updater.Add(m_WiringOperator);


        }

        void IBootable.Shutdown()
        {
            m_Updater.Shutdown();
            m_WiringOperator.JoinStreamEvent -= CreatePeer;
            m_WiringOperator.LeftStreamEvent -= DestroyPeer;
        }

        private void DestroyPeer(Line Obj)
        {
            Socket rudpSocket;
            if (m_Peers.TryGetValue(Obj.EndPoint, out rudpSocket))
            {
                rudpSocket.Break();
                m_Updater.Remove(rudpSocket);
                lock (m_Peers)
                {
                    m_Peers.Remove(Obj.EndPoint);
                }

            }

        }

        private void CreatePeer(Line Obj)
        {
        }

        public Socket Connect(EndPoint EndPoint, System.Action<bool> Result)
        {
            Socket rudpSocket = null;
            System.Action<Line> handler = Stream =>
            {
                PeerConnecter connecter = new PeerConnecter(Stream);
                connecter.TimeoutEvent += () => { Result(obj: false); };
                connecter.DoneEvent += () => { Result(obj: true); };

                Socket p = new Socket(Stream, connecter);
                p.CloseEvent += () => { Remove(p); };

                lock (m_Peers)
                {
                    m_Peers.Add(Stream.EndPoint, p);
                }

                m_Updater.Add(p);
                rudpSocket = p;
            };

            lock (m_WiringOperator)
            {
                m_WiringOperator.JoinStreamEvent += handler;
                m_WiringOperator.Create(EndPoint);
                m_WiringOperator.JoinStreamEvent -= handler;
            }

            return rudpSocket;
        }

        private void Remove(Socket rudp_socket)
        {
            lock (m_RemovePeers)
            {
                m_RemovePeers.Add(rudp_socket);
            }

        }




    }
}