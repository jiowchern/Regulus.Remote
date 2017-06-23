using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Regulus.Serialization;
using System.Linq;
namespace Regulus.Network.RUDP
{
    public class SendHandler
    {
        private readonly ISendable _Sendable;
        private readonly Serializer _Serializer;

        public SendHandler(ISendable sendable, Serializer serializer)
        {
            _Sendable = sendable;
            _Serializer = serializer;        
        }
        

        public void PushAck(uint package_serial_number , EndPoint end_point)
        {
            var ackPackage = new AckPackage();
            ackPackage.SerialNumber = package_serial_number;
            var buffer = _Serializer.ObjectToBuffer(ackPackage);
            var package = new SocketPackage();
            package.EndPoint = end_point;
            package.Buffer = buffer;

            _Sendable.Transport(package);
                           
        }

        public void PushMessage(ref MessagePackage message , EndPoint end_point)
        {
            var buffer = _Serializer.ObjectToBuffer(message);
            var package = new SocketPackage();
            package.EndPoint = end_point;
            package.Buffer = buffer;
            _Sendable.Transport(package);
        }

        public void PushListenAgree(EndPoint end_point)
        {
            
            var buffer = _Serializer.ObjectToBuffer(new ListenAgreePackage());

            var package = new SocketPackage();
            package.EndPoint = end_point;
            package.Buffer = buffer;
            _Sendable.Transport(package);
        }

        public void PushConnectRequest(EndPoint end_point)
        {
            var buffer = _Serializer.ObjectToBuffer(new ConnectRequestPackage());

            var package = new SocketPackage();
            package.EndPoint = end_point;
            package.Buffer = buffer;
            _Sendable.Transport(package);
        }

        public void PushConnectedAck(EndPoint end_point)
        {
            var buffer = _Serializer.ObjectToBuffer(new ConnectedAckPackage());

            var package = new SocketPackage();
            package.EndPoint = end_point;
            package.Buffer = buffer;
            _Sendable.Transport(package);
        }
    }
}