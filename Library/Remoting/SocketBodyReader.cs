using System;
using System.Net.Sockets;
using Regulus.Network;

namespace Regulus.Remoting
{

    internal interface ISocketReader
    {
        event OnByteDataCallback DoneEvent;
        event OnErrorCallback ErrorEvent;
    }
    internal class SocketBodyReader : ISocketReader
    {
        public event OnByteDataCallback DoneEvent;

        public event OnErrorCallback ErrorEvent;

        private readonly IPeer _Peer;

        private byte[] _Buffer;

        private int _Offset;

        public SocketBodyReader(IPeer peer)
        {
            this._Peer = peer;
        }

        internal void Read(int size)
        {
            _Offset = 0;
            _Buffer = new byte[size];
            try
            {
                _Peer.Receive(_Buffer, _Offset, _Buffer.Length - _Offset,  _Readed);
            }
            catch(SystemException e)
            {
                if(ErrorEvent != null)
                {
                    ErrorEvent();
                }
            }
        }

        private void _Readed(int read_count , SocketError error)
        {
            try
            {

                var readSize = read_count;

                if (error == SocketError.Success && readSize != 0)
                {
                    _Offset += readSize;
                    NetworkMonitor.Instance.Read.Set(readSize);
                    if (_Offset == _Buffer.Length)
                    {
                        DoneEvent(_Buffer);
                    }
                    else
                    {
                        _Peer.Receive(
                            _Buffer,
                            _Offset,
                            _Buffer.Length - _Offset,                            
                            _Readed);
                    }
                }
                else
                {
                    Regulus.Utility.Log.Instance.WriteDebug(string.Format("read body error {0} size:{1}", error, readSize));
                    if (ErrorEvent != null)
                    {
                        ErrorEvent();
                    }
                    
                }
                
            }
            catch(SystemException e)
            {
                if(ErrorEvent != null)
                {
                    ErrorEvent();
                }
            }
            finally
            {
            }
        }
    }
}