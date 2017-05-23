using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;

namespace Regulus.Remoting
{
    internal class SocketHeadReader : ISocketReader
    {
        private readonly Socket _Socket;

        private readonly System.Collections.Generic.List<byte> _Buffer;

        private readonly byte[] _ReadedByte;
        public SocketHeadReader(Socket socket)
        {
            _ReadedByte = new byte[1];
            _Socket = socket;
            _Buffer = new List<byte>();
            
        }

        public void Read()
        {
            _Read();
        }
        private void _Read()
        {

            try
            {
                _Socket.BeginReceive(_ReadedByte, 0, 1, SocketFlags.None, _Readed, null);
            }
            catch (SystemException e)
            {
                if (_ErrorEvent != null)
                {
                    _ErrorEvent();
                }
            }
            
        }

        private void _Readed(IAsyncResult ar)
        {

            try
        
            {
                SocketError error;
                var readSize = _Socket.EndReceive(ar , out error);                
                NetworkMonitor.Instance.Read.Set(readSize);


                if (error == SocketError.Success && readSize != 0)
                {
                    if (_ReadData(readSize))
                    {
                        if (_DoneEvent != null)
                            _DoneEvent(_Buffer.ToArray());
                    }
                    else
                    {
                        _Read();
                    }
                }
                else
                {
                    Regulus.Utility.Log.Instance.WriteDebug(string.Format("read head error {0} size:{1}", error, readSize));
                    if (_ErrorEvent != null)
                        _ErrorEvent();
                }
                

            }
            catch (SystemException e)
            {
                if (_ErrorEvent != null)
                {
                    _ErrorEvent();
                }
            }
            finally
            {
            }
        }

        private bool _ReadData(int readSize)
        {
            
            if (readSize != 0)
            {
                var value = _ReadedByte[0];
                _Buffer.Add(value);

                if (value < 0x80)
                {                    
                    return true;
                }
            }
            return false;
        }

        private OnByteDataCallback _DoneEvent;
        event OnByteDataCallback ISocketReader.DoneEvent
        {
            add { _DoneEvent += value; }
            remove { _DoneEvent -= value; }
        }

        private event OnErrorCallback _ErrorEvent;
        event OnErrorCallback ISocketReader.ErrorEvent
        {
            add { _ErrorEvent += value; }
            remove { _ErrorEvent -= value; }
        }
    }
}