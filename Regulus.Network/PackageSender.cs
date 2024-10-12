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
        
        
        private Task _Sending;

        public PackageSender(IStreamable stream, Regulus.Memorys.IPool pool)
        {
            _Sending = Task.CompletedTask;
                    
            _Stream = stream;
            _Pool = pool;

            //_Consumer.Add(this);
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
            //_Consumer.Remove(this);
        }

        

        private void _Push(Memorys.Buffer buffer)
        {
            _Sending = _Sending.ContinueWith(async t => {
                await _SendBufferAsync(buffer);
            });
        
        }
        private async Task _SendBufferAsync(Regulus.Memorys.Buffer buffer)
        {
            var sendCount = 0;
            do
            {
                var count = await _Stream.Send(buffer.Bytes.Array, buffer.Bytes.Offset + sendCount, buffer.Count - sendCount);
                if (count == 0)
                    return;
                sendCount += count;

            } while (sendCount < buffer.Count);
        }
        


    }
}
