using System.Collections.Generic;
using System.Linq;
using Regulus.Framework;
using Regulus.Utility;

namespace Regulus.Network
{
	public class Host : IUpdatable<Timestamp>
	{
	    private readonly WiringOperator m_WiringOperator;
        public event System.Action<Socket> AcceptEvent;

	    private readonly Dictionary<Line , Socket> m_Peers;
	    private readonly List<Socket> m_RemovePeers;
        private readonly Updater<Timestamp> m_Updater;
	    
	    
        public int PeerCount => m_Peers.Count;

	    public Host(ISocketRecevieable SocketRecevieable , ISocketSendable SocketSendable)
        {
            m_RemovePeers = new List<Socket>();
            m_Updater = new Updater<Timestamp>();
            m_Peers = new Dictionary<Line, Socket>();
            m_WiringOperator = new WiringOperator(SocketSendable, SocketRecevieable ,true);            
        }

	    


	    bool IUpdatable<Timestamp>.Update(Timestamp Arg)
	    {
	        m_Updater.Working(Arg);

	        var count = m_RemovePeers.Count;
	        for (var index = 0; index < count; index++)
	            m_WiringOperator.Destroy(m_RemovePeers[index].EndPoint);
	        m_RemovePeers.Clear();
            return true;
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

	    private void DestroyPeer(Line Line)
	    {
	        

               var peer = m_Peers.FirstOrDefault(P => P.Key == Line);
	        

            if (peer.Key != null)
	        {
	            peer.Value.Break();
                m_Peers.Remove(peer.Key);
	            m_Updater.Remove(peer.Value);
            }
        }

	    

        private void CreatePeer(Line Line)
        {
            var listener = new PeerListener(Line);

            var peer = new Socket(Line, listener);
            peer.CloseEvent += () => { Remove(peer); };
            listener.DoneEvent += () =>
            {
                if (AcceptEvent != null)
                    AcceptEvent(peer);
            };
            

            m_Peers.Add(Line,peer);            
            m_Updater.Add(peer);


        }

	    private void Remove(Socket rudp_socket)
	    {
	        m_RemovePeers.Add(rudp_socket);
        }
	}
}
