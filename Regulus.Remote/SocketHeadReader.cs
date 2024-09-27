using Regulus.Memorys;
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

        readonly Regulus.Memorys.Buffer _Buffer;
        readonly ArraySegment<byte> _Bytes;
        int _Offset;
        int _Readed;

        public SocketHeadReader(IStreamable peer , Regulus.Memorys.Buffer buffer)
        {
            
            _Peer = peer;
            _Buffer = buffer;
            _Bytes = _Buffer.Bytes;
        }

        public void Read()
        {            
            _Check(0);
        }
        private async Task<int> _Read()
        {                        
            return await _Peer.Receive(_Bytes.Array, _Bytes.Offset + _Offset, _Bytes.Count - _Offset );            
        }
        

        private bool _ReadData(int read_size)
        {
            
            if (read_size == 0)
                return false;
            _Readed += read_size;

            for (int i = 0; i < read_size; i++)
            {
                
                byte value = _Bytes.Array[_Bytes.Offset + _Offset++ ];                
                if (value < 0x80)
                {                    
                    return true;
                }
            }
           
            return false;
        }

        private void _Check(int read_size)
        {
            if (_ReadData(read_size))
            {
                DoneEvent(_Offset, _Readed);
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

        public System.Action<int,int> DoneEvent;
        
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