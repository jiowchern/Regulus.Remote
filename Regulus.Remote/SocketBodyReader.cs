using Regulus.Network;
using Regulus.Utility;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Regulus.Remote
{
    internal class SocketBodyReader : ISocketReader
    {
        public event OnByteDataCallback DoneEvent;

        public event OnErrorCallback ErrorEvent;

        private readonly IStreamable _Peer;

        private Regulus.Memorys.Buffer _Buffer;

        private int _Offset;
        

        public SocketBodyReader(IStreamable peer)
        {
            this._Peer = peer;
         
        }

        internal void Read(Regulus.Memorys.Buffer buffer)
        {
            _Offset = 0;
            _Buffer = buffer;
            _Check(_Offset);
        }

        private async Task<int> _Read()
        {
            
            try
            {
                var bytes = _Buffer.Bytes;
                return await _Peer.Receive(bytes.Array, bytes.Offset + _Offset, bytes.Count - _Offset);
                //return await _Peer.Receive(_Buffer, _Offset, _Buffer.Count - _Offset);

            }
            catch (SystemException e)
            {
                if (ErrorEvent != null)
                {
                    ErrorEvent();
                }

                throw e;
            }
        }



        private void _Check(int read_size)
        {
            
            _Offset += read_size;
            NetworkMonitor.Instance.Read.Set(read_size);
            if (_Offset == _Buffer.Count)
            {
                
                DoneEvent(_Buffer.Bytes.Array);                
            }
            else
            {
                _Read().ContinueWith(t => {
                    var readSize = t.Result;
                    int waitTime = readSize == 0 ? 10 : 0;   

                    System.Threading.Tasks.Task.Delay(waitTime).ContinueWith(_ => {
                        _Check(readSize);
                    });

                });
            }
        }
    }
}