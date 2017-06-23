using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Regulus.Extension;
using Regulus.Serialization;

namespace Regulus.Network.RUDP
{
    public class ReceiveHandler
    {
        
        private readonly List<SocketPackage> _Packages;
        

        public event Action<AckPackage, EndPoint> PopAckEvent;
        public event Action<MessagePackage, EndPoint> PopMessageEvent;
        public event Action<ConnectRequestPackage, EndPoint> PopConnectRequestEvent;
        public event Action<ConnectedAckPackage, EndPoint> PopConnectAckEvent;
        public event Action<ListenAgreePackage, EndPoint> PopListenAgreeEvent;

        private readonly Serializer _Serializer;
        public ReceiveHandler(Serializer serializer)
        {            
            _Packages = new List<SocketPackage>();            
            _Serializer = serializer;
        }

        

        void _Push(SocketPackage socket_package)
        {            
            lock (_Packages)
            {
                _Packages.Add(socket_package);
            }            
        }

        public void Pop()
        {
            lock (_Packages)
            {
                foreach (var package in _Packages)
                {
                    _ParsePackage(package.Buffer , package.EndPoint );                    
                }
                _Packages.Clear();
            }
        }

        private void _ParsePackage(byte[] buffer , EndPoint end_point)
        {
            try
            {
                var package = _Serializer.BufferToObject(buffer);
                if (package is AckPackage)
                {
                    var ack = (AckPackage)package;

                    PopAckEvent?.Invoke(ack, end_point);
                }
                else if (package is MessagePackage)
                {
                    var message = (MessagePackage)package;
                    
                    PopMessageEvent?.Invoke(message, end_point);
                }
                else if (package is ConnectRequestPackage)
                {
                    var connect = (ConnectRequestPackage)package;

                    PopConnectRequestEvent?.Invoke(connect, end_point);
                    
                }
                else if (package is ListenAgreePackage)
                {
                    

                    PopListenAgreeEvent?.Invoke((ListenAgreePackage)package, end_point);
                }
                else if (package is ConnectedAckPackage)
                {
                    var accept = (ConnectedAckPackage)package;

                    PopConnectAckEvent?.Invoke(accept, end_point);

                }



            }
            catch (Exception e)
            {
                Regulus.Utility.Log.Instance.WriteInfo(string.Format("receive error package {0}" , buffer.ShowMembers()));                
            }
        }

        public void Push(SocketPackage obj)
        {
            _Push(obj);
        }
    }


    
}