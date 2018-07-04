using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using Regulus.Network;

namespace Regulus.Remoting
{
    internal class SocketHeadReader : ISocketReader
    {
        private readonly IPeer _Peer;

        private readonly System.Collections.Generic.List<byte> _Buffer;

        private readonly byte[] _ReadedByte;
        public SocketHeadReader(IPeer peer)
        {
            _ReadedByte = new byte[1];
            _Peer = peer;
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
                _Peer.Receive(_ReadedByte, 0, 1 , _Readed);
            }
            catch (SystemException e)
            {
                if (_ErrorEvent != null)
                {
                    _ErrorEvent();
                }
            }
            
        }

        private void _Readed(int read_size )
        {

            if (read_size != 0)
            {
                if (_ReadData(read_size))
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
                Regulus.Utility.Log.Instance.WriteDebug(string.Format("read head error size:{0}", read_size));
                if (_ErrorEvent != null)
                    _ErrorEvent();
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