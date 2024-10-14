using Regulus.Memorys;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;


namespace Regulus.Network
{
    public class PackageSender : IDisposable
    {        
        private readonly IStreamable _Stream;
        private readonly IPool _Pool;
        
        
        private Task<int> _Sending;

        public PackageSender(IStreamable stream, Regulus.Memorys.IPool pool)
        {
            _Sending = Task.FromResult(0);

            _Stream = stream;
            _Pool = pool;
        }

        public void Push(Regulus.Memorys.Buffer buffer)
        {
            if (buffer.Count == 0)
                return;

            var packageVarintCount = Regulus.Serialization.Varint.GetByteCount(buffer.Bytes.Count);
            var sendBuffer = _Pool.Alloc(packageVarintCount + buffer.Bytes.Count);
            var offset = Regulus.Serialization.Varint.NumberToBuffer(sendBuffer.Bytes.Array, sendBuffer.Bytes.Offset, buffer.Bytes.Count);

            for (int i = 0; i < buffer.Bytes.Count; i++)
            {
                sendBuffer.Bytes.Array[sendBuffer.Bytes.Offset + offset + i] = buffer.Bytes.Array[buffer.Bytes.Offset + i];
            }
            offset += buffer.Bytes.Count;

            _Push(sendBuffer);
        }

        void IDisposable.Dispose()
        {            
        }

        

        private void _Push(Memorys.Buffer buffer)
        {
            if(_Sending.IsCompleted || _Sending.IsFaulted || _Sending.IsCanceled )
            {
                _Sending = _SendBufferAsync(buffer);
            }
            else
            {
                _Sending = _Sending.ContinueWith(t=> _SendBufferAsync(buffer)).Unwrap();
            }
        }

        private async Task<int> _SendBufferAsync(Regulus.Memorys.Buffer buffer)
        {
            var sendCount = 0;
            do
            {
                var count = await _Stream.Send(buffer.Bytes.Array, buffer.Bytes.Offset + sendCount, buffer.Count - sendCount);
                if (count == 0)
                    return 0;
                sendCount += count;

            } while (sendCount < buffer.Count);
            return sendCount;
        }
        


    }
}
