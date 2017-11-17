using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using Regulus.Network.Package;

namespace Regulus.Network
{
    internal class SocketSender 
    {
        private readonly System.Net.Sockets.Socket m_Socket;
        

        
        private readonly Queue<SocketMessage> _Messages;
        private volatile bool _Enable;
        private ManualResetEvent _Mre;
        

        public SocketSender(System.Net.Sockets.Socket Socket)
        {
            
            _Messages = new Queue<SocketMessage>();
            m_Socket = Socket;
            _Enable = true;

            System.Threading.ThreadPool.QueueUserWorkItem(_Run);
            
            _Mre = new ManualResetEvent(false);
        }

        private void _Run(object state)
        {
            ManualResetEvent mre = new ManualResetEvent(true);
            
            while (_Enable)
            {
                _Mre.WaitOne();
            
                SocketMessage message;
                lock (_Messages)
                {
                    if (_Messages.Count > 0)
                    {
                        message = _Messages.Dequeue();
                    }
                    else
                    {
                        _Mre.Reset();
                        continue;
                    }
                    
                }

                var size = message.GetPackageSize();
                
                mre.Reset();
                m_Socket.BeginSendTo(message.Package, 0, size , SocketFlags.None, message.RemoteEndPoint, _Done , mre);
                mre.WaitOne();
                /*var 
                  size = message.GetPackageSize();
                var count = 0;
                while (count < size)
                {
                    count += m_Socket.SendTo(message.Package, count, size - count, SocketFlags.None, message.RemoteEndPoint);
                }*/

            }
            
        }

        private void _Done(IAsyncResult ar)
        {
            var mre = (ManualResetEvent) ar.AsyncState;
            mre.Set();
        }


        public void Transport(SocketMessage Message)
        {
            lock (_Messages)
            {
                _Messages.Enqueue(Message);
                _Mre.Set();
            }
            
        }


        public void Stop()
        {
            _Enable = false;
        }
    }
}