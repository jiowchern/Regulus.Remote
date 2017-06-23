using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using Regulus.Framework;
using Regulus.Serialization;
using Regulus.Utility;

namespace Regulus.Network.RUDP
{
	public class Host : IUpdatable<Timestamp>
	{
	    private readonly IRecevieable _Recevieable;
	    public event System.Action<IPeer> AcceptEvent;

	    private readonly Regulus.Utility.Updater<Timestamp> _Updater;		

	    private readonly ReceiveHandler _ReceiveHandler;
	    private readonly SendHandler _SendHandler;
        private readonly List<Transmitter> _Transmitters;
	    private readonly List<Listener> _Listeners;

	    public static Host CreateStandard(int port)
	    {
	        var endPoint = new System.Net.IPEndPoint(System.Net.IPAddress.Any, port);
	        var socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Dgram, System.Net.Sockets.ProtocolType.Udp);
	        socket.Bind(endPoint);
            return new Host(new SocketRecevier(socket , Transmitter.PackageSize ),new SocketSender(socket) );
	    }

        public Host(IRecevieable recevieable , ISendable sendable)
		{            
		    _Listeners = new List<Listener>();
            _Transmitters = new List<Transmitter>();

		    _Updater = new Updater<Timestamp>();
		    var serializer = Transmitter.CreateSerializer();

		    _Recevieable = recevieable;
            _ReceiveHandler = new ReceiveHandler(serializer);

            _SendHandler = new SendHandler(sendable,  serializer);
        }

	    void IBootable.Launch()
	    {
	        _ReceiveHandler.PopConnectAckEvent += _Receive;

            _ReceiveHandler.PopConnectRequestEvent += _Receive;
            _ReceiveHandler.PopAckEvent += _Receive;
	        _ReceiveHandler.PopMessageEvent += _Receive;
            _Recevieable.ReceivedEvent += _ReceiveHandler.Push;
	    }

	    void IBootable.Shutdown()
	    {
	        
            _Recevieable.ReceivedEvent -= _ReceiveHandler.Push;

	        _ReceiveHandler.PopConnectAckEvent -= _Receive;
            _ReceiveHandler.PopConnectRequestEvent -= _Receive;
            _ReceiveHandler.PopAckEvent -= _Receive;
	        _ReceiveHandler.PopMessageEvent -= _Receive;
        }

	    bool IUpdatable<Timestamp>.Update(Timestamp time)
	    {
	        _ReceiveHandler.Pop();
            _Updater.Working(time);
	        return true;
	    }


	    

	    private Transmitter _FindTransmitter(EndPoint end_point)
	    {
	        return _Transmitters.FirstOrDefault(p => p.EndPoint == end_point);
	        
	    }

	    void _Receive(MessagePackage package, EndPoint end_point)
	    {
	        Transmitter transmitter = _FindTransmitter(end_point);
	        if (transmitter != null)
	        {
	            transmitter.Receive(ref package);
	        }
	    }

        void _Receive(AckPackage package, EndPoint end_point)
	    {
	        Transmitter transmitter = _FindTransmitter(end_point);
	        if (transmitter != null)
	        {
	            transmitter.Receive(ref package);
	        }
        }

	    void _Receive(ConnectedAckPackage syn, EndPoint end_point)
	    {
	        var listener = _FindListener(end_point);
            if(listener != null)
	            listener.SetConnectAck(syn);

        }

	    private Listener _FindListener(EndPoint end_point)
	    {
	        return _Listeners.FirstOrDefault(l => l.EndPoint == end_point);
	        
	    }

	    void _Receive(ConnectRequestPackage request, EndPoint end_point)
	    {
            if (_HaveEndPoint(end_point) == false)
	        {
	            var listener = new Listener(end_point , _SendHandler, Config.HostListenTimeout );	            
	            listener.SuccessEvent += ()=> { _CreateTransmitter(listener);};
	            listener.TimeoutEvent += ()=> { _ReleaseListener(listener); };
                _Listeners.Add(listener);
	            _Updater.Add(listener);
            }

        }

	    private void _CreateTransmitter(Listener listener)
	    {
	        var transmitter = new Transmitter(listener.EndPoint , _SendHandler , new BufferDispenser(Transmitter.PackageSize));
	        transmitter.TimeoutEvent += () => { _ReleaseTransmitter(transmitter); };
	        _Listeners.Remove(listener);
	        _Updater.Remove(listener);

            _Transmitters.Add(transmitter);
	        _Updater.Add(transmitter);

	        AcceptEvent(transmitter);
	    }

	    private void _ReleaseTransmitter(Transmitter transmitter)
	    {
	        _Transmitters.Remove(transmitter);
            _Updater.Remove(transmitter);
	    }

	    private void _ReleaseListener(Listener listener)
	    {
	        _Listeners.Remove(listener);

	        _Updater.Remove(listener);
            
	    }

	    private bool _HaveEndPoint(EndPoint end_point)
	    {
	        return _Transmitters.Any(peer => peer.EndPoint == end_point) ||
	               _Listeners.Any(listener => listener.EndPoint == end_point);
	    }
	}
}
