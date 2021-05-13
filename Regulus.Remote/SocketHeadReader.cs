using Regulus.Network;
using Regulus.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Regulus.Remote
{
    internal class SocketHeadReader : ISocketReader
    {
        private readonly IStreamable _Peer;

        private readonly System.Collections.Generic.List<byte> _Buffer;

        private readonly byte[] _ReadedByte;
        
        public SocketHeadReader(IStreamable peer)
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

            var task = _Peer.Receive(_ReadedByte, 0, 1);
            task.ValueEvent += _Readed;            
        }

        

        private void _Readed(int read_size)
        {
        
            if (_ReadData(read_size))
            {
                _DoneEvent(_Buffer.ToArray());
            }
            else 
            {
                _Read();
            }
        }

        private bool _ReadData(int readSize)
        {

            if (readSize != 0)
            {
                byte value = _ReadedByte[0];
                _Buffer.Add(value);

                if (value < 0x80)
                {
                    return true;
                }
            }
            return false;
        }

        void IBootable.Launch()
        {
            
        }

        void IBootable.Shutdown()
        {        
        
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