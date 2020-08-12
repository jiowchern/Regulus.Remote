using Regulus.Network;
using Regulus.Utility;
using System;
using System.Collections.Generic;

namespace Regulus.Remote
{
    internal class SocketHeadReader : ISocketReader
    {
        private readonly IStreamable _Peer;

        private readonly System.Collections.Generic.List<byte> _Buffer;

        private readonly byte[] _ReadedByte;
        bool _Enable;
        public SocketHeadReader(IStreamable peer)
        {
            _Enable = true;
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

            System.Threading.Tasks.Task<int> task = _Peer.Receive(_ReadedByte, 0, 1);
            task.ContinueWith(t => _Readed(t.Result));
        }

        private void _Readed(int read_size)
        {

            if (_ReadData(read_size))
            {
                if (_DoneEvent != null)
                    _DoneEvent(_Buffer.ToArray());
            }
            else if (_Enable)
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
            _Enable = false;
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