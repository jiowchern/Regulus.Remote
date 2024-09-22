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

        private byte[] _Buffer;

        private int _Offset;

        public static readonly byte[] Empty = new byte[0];

        public SocketBodyReader(IStreamable peer)
        {
            this._Peer = peer;
         
        }



        internal void Read(int size)
        {
        
            
            
            _Offset = 0;
            _Buffer = new byte[size];



            _Check(_Offset);
            
            
        }

        private async Task<int> _Read()
        {
            
            try
            {
                return await _Peer.Receive(_Buffer, _Offset, _Buffer.Length - _Offset);

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
            if (_Offset == _Buffer.Length)
            {
                
                DoneEvent(_Buffer);                
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