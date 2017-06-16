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
        private readonly ArrayCache<byte> _Cache;
        private readonly List<byte[]> _Buffers;
        public interface IPackageReceviable
        { 
            void Receive(ref MessagePackage package);
            void Receive(ref AckPackage package);

            void Receive(ref PingPackage package);

            void Receive(ref ConnectPackage package);
        }
        private readonly Serializer _Serializer;

        
        public ReceiveHandler(Serializer serializer,int package_size)
        {
            _Cache = new ArrayCache<byte>(package_size);
            _Buffers = new List<byte[]>();
            _Serializer = serializer;
        }

        public void Push(byte[] buffer , int count)
        {
            var data = _Cache.Alloc();
            for (int i = 0; i < count; i++)
            {
                data[i] = buffer[i];
            }
            _Buffers.Add(data);
        }

        public void Pop(IPackageReceviable receviable)
        {
            foreach (var buffer in _Buffers)
            {
                _ParsePackage(buffer, receviable);
                _Cache.Free(buffer);
            }

            _Buffers.Clear();
        }
        

        private void _ParsePackage(byte[] buffer , IPackageReceviable receviable)
        {
            try
            {
                var package = _Serializer.BufferToObject(buffer);
                if (package is PingPackage)
                {
                    var ping = (PingPackage)package;
                    receviable.Receive(ref ping);
                    
                }
                else if (package is AckPackage)
                {
                    var ack = (AckPackage)package;
                    receviable.Receive(ref ack);
                }
                else if (package is MessagePackage)
                {
                    var data = (MessagePackage)package;
                    receviable.Receive(ref data);
                }
                else if (package is ConnectPackage)
                {
                    var connect = (ConnectPackage)package;
                    receviable.Receive(ref connect);
                }

            }
            catch (Exception e)
            {
                Regulus.Utility.Log.Instance.WriteInfo(string.Format("receive error package {0}" , buffer.ShowMembers()));                
            }
        }
    }
}