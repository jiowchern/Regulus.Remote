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
        int _ReadSize;
        System.Action _Update;
        
        public SocketHeadReader(IStreamable peer)
        {
        
            _ReadedByte = new byte[1];
            _Peer = peer;
            _Buffer = new List<byte>();
            _Update = () => { };
        }

        public void Read()
        {
            _Read();
        }
        private void _Read()
        {
            _ReadSize = 0;
            _Update = () => { };
            _Peer.Receive(_ReadedByte, 0, 1).ValueEvent += _ReadDone;            
        }

        private void _ReadDone(int read_size)
        {
            _ReadSize += read_size;
            _Update = _Check;
        }

        private bool _ReadData()
        {
            if (_ReadSize == 0)
                return false;
            
            byte value = _ReadedByte[0];
            _Buffer.Add(value);

            if (value < 0x80)
            {
                return true;
            }
            return false;
        }

        

        void IStatus.Enter()
        {
            
        }

        void IStatus.Leave()
        {
            
        }

        void IStatus.Update()
        {
            _Update();
            
        }

        private void _Check()
        {
            if (_ReadData())
            {
                _DoneEvent(_Buffer.ToArray());
            }
            else
            {
                _Read();
            }
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