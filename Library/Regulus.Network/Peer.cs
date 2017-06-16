using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Regulus.Framework;
using Regulus.Serialization;
using Regulus.Utility;

namespace Regulus.Network.RUDP
{
    

    public class Peer : IUpdatable, ReceiveHandler.IPackageReceviable
    {
	    private readonly Socket _Socket;
	    private EndPoint _EndPoint;

	    public const int PackageSize = 548;
	    
	    private readonly ReceiveHandler _ReceiveHandler;
	    private readonly Serializer _Serializer;
	    
	    private readonly SendHandler _SendHandler;
	    private readonly byte[] _ReceiveBuffer;
        private readonly List<byte[]> _SendBuffers;

        


	    public Peer(Socket socket , EndPoint end_point)
        {
            _Socket = socket;
            _EndPoint = end_point;
            _ReceiveBuffer = new byte[PackageSize];

            _Serializer = Peer.CreateSerializer();
            _ReceiveHandler = new ReceiveHandler(_Serializer , PackageSize);	        
            _SendHandler = new SendHandler(PackageSize , _Serializer);            
        }

	   

	   



	    public bool Activity { get; }
		

		public event Action<byte[]> ReceiveEvent;

		public void Send(byte[] buffer)
		{
		    _SendHandler.PushData(buffer);
		}

	    public static Serializer CreateSerializer()
	    {
	        var builder = new DescriberBuilder(typeof(byte), typeof(byte[]),
	            typeof(MessagePackage), typeof(AckPackage), typeof(PingPackage), typeof(ConnectPackage));
	        var lastId = builder.Describers.Length;
            return new Regulus.Serialization.Serializer(builder.Describers.Union(new ITypeDescriber[] { new BlittableDescriber(++lastId, typeof(uint)) ,new BlittableDescriber(++lastId, typeof(int)) }).ToArray()  );
	    }

	    void IBootable.Launch()
	    {
	        _BeginReceive();
	    }

	    private void _BeginReceive()
	    {
	        _Socket.BeginReceiveFrom(_ReceiveBuffer, 0, PackageSize, SocketFlags.None, ref _EndPoint, _EndReceive, null);
	    }

	    private void _EndReceive(IAsyncResult ar)
        {
            
            var readCount = _Socket.EndReceiveFrom(ar, ref _EndPoint );
            if (readCount > 0)
            {
                _ReceiveHandler.Push(_ReceiveBuffer , readCount);

                _BeginReceive();
            }
            else
            {
                //todo : 接受到0長度封包可能是對方已中斷 .. 需要處理我方斷線
            }
        }

        void IBootable.Shutdown()
	    {
	        
	    }

	    bool IUpdatable.Update()
	    {
	        _ReceiveHandler.Pop(this);
	        _SendBuffers.Clear();
            _SendHandler.PopPackages(_SendBuffers);
	        if (_SendBuffers.Count > 0)
	        {
	           // _BeginSend(_SendBuffers);

	        }
            return true;
	    }

        

        void ReceiveHandler.IPackageReceviable.Receive(ref MessagePackage package)
        {
            _SendAck(package.Ack);
            ReceiveEvent(package.Data);
        }

        private void _SendAck(uint package_ack)
        {
            _SendHandler.PushAck(package_ack);
        }

        void ReceiveHandler.IPackageReceviable.Receive(ref AckPackage package)
        {
            throw new NotImplementedException();
        }

        void ReceiveHandler.IPackageReceviable.Receive(ref PingPackage package)
        {
            throw new NotImplementedException();
        }

        void ReceiveHandler.IPackageReceviable.Receive(ref ConnectPackage package)
        {
            
        }
    }
}