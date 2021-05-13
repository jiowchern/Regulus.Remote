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

        System.Action _Update;
        public SocketBodyReader(IStreamable peer)
        {
            this._Peer = peer;
            _Update = () => { };
        }



        internal void Read(int size)
        {
            _Update = () => { };
            _Offset = 0;
            _Buffer = new byte[size];
            try
            {
                var task = _Peer.Receive(_Buffer, _Offset, _Buffer.Length - _Offset);
                task.ValueEvent += _Readed;
                
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
            int readSize = read_count;

            _Offset += readSize;
            NetworkMonitor.Instance.Read.Set(readSize);
            _Update = _Check;
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
            if (_Offset == _Buffer.Length)
            {
                DoneEvent(_Buffer);
            }
            else
            {
                var task = _Peer.Receive(
                    _Buffer,
                    _Offset,
                    _Buffer.Length - _Offset);
                task.ValueEvent += _Readed;
            }
        }
    }
}