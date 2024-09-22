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
            
            _Check(0);
        }
        private async Task<int> _Read()
        {            
            //_Peer.Receive(_ReadedByte, 0, 1).ValueEvent += _ReadDone;            
            return await _Peer.Receive(_ReadedByte, 0, 1);
            
        }
        

        private bool _ReadData(int read_size)
        {
            if (read_size == 0)
                return false;            
            byte value = _ReadedByte[0];
            _Buffer.Add(value);

            if (value < 0x80)
            {
                return true;
            }
            return false;
        }

        private void _Check(int read_size)
        {
            if (_ReadData(read_size))
            {
                
                _DoneEvent(_Buffer.ToArray());
            }
            else
            {
                int delayTime = read_size == 0 ? 10 : 0;    
                System.Threading.Tasks.Task.Delay(delayTime).ContinueWith((_) =>
                {
                    _Read().ContinueWith((task) =>
                    {
                        _Check(task.Result);
                    });
                });                
            }
        }

        private OnByteDataCallback _DoneEvent;
        event OnByteDataCallback ISocketReader.DoneEvent
        {
            add { _DoneEvent += value; }
            remove { _DoneEvent -= value; }
        }
#pragma warning disable CS0067
        private event OnErrorCallback _ErrorEvent;
#pragma warning restore CS0067
        event OnErrorCallback ISocketReader.ErrorEvent
        {
            add { _ErrorEvent += value; }
            remove { _ErrorEvent -= value; }
        }
    }
}