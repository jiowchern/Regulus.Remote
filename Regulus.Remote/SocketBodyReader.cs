using Regulus.Network;
using Regulus.Utility;
using System;

namespace Regulus.Remote
{
    internal class SocketBodyReader : ISocketReader
    {
        public event OnByteDataCallback DoneEvent;

        public event OnErrorCallback ErrorEvent;

        private readonly IStreamable _Peer;

        private byte[] _Buffer;

        private int _Offset;

        volatile bool _Enable;

        public SocketBodyReader(IStreamable peer)
        {
            _Enable = true;
            this._Peer = peer;
        }

        

        internal void Read(int size)
        {            
            _Offset = 0;
            _Buffer = new byte[size];
            try
            {
                System.Threading.Tasks.Task<int> task = _Peer.Receive(_Buffer, _Offset, _Buffer.Length - _Offset);                
                task.ContinueWith(t => _Readed(t.Result));


            }
            catch (SystemException e)
            {
                if (ErrorEvent != null)
                {
                    ErrorEvent();
                }
            }
        }

        private void _Readed(int read_count)
        {
            if (!_Enable)
                return;
            int readSize = read_count;

            _Offset += readSize;
            NetworkMonitor.Instance.Read.Set(readSize);
            if (_Offset == _Buffer.Length)
            {            
                
                DoneEvent(_Buffer);
            }
            else                 
            {
                System.Threading.Tasks.Task<int> task = _Peer.Receive(
                    _Buffer,
                    _Offset,
                    _Buffer.Length - _Offset);
                task.ContinueWith(t => _Readed(t.Result));
            }
        }

        

        void IBootable.Launch()
        {
            
        }

        void IBootable.Shutdown()
        {
            DoneEvent = _Empty;
            _Enable = false;
        }

        private void _Empty(byte[] bytes)
        {
            
        }
    }
}