using System;
using System.Net.Sockets;

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

        private readonly Socket _Socket;

        private byte[] _Buffer;

        private int _Offset;

        public SocketBodyReader(Socket socket)
        {
            this._Socket = socket;
        }

        internal void Read(int size)
        {
            _Offset = 0;
            _Buffer = new byte[size];
            try
            {
                _Socket.BeginReceive(_Buffer, _Offset, _Buffer.Length - _Offset, 0, _Readed, null);
            }
            catch(SystemException e)
            {
                if(ErrorEvent != null)
                {
                    ErrorEvent();
                }
            }
        }

        private void _Readed(IAsyncResult ar)
        {
            try
            {
                SocketError error;
                var readSize = _Socket.EndReceive(ar , out error );

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
                        _Socket.BeginReceive(
                            _Buffer,
                            _Offset,
                            _Buffer.Length - _Offset,
                            SocketFlags.None,
                            _Readed,
                            null);
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