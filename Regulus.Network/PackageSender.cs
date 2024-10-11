

using Regulus.Memorys;
using System.Collections.Concurrent;
using System.Threading.Tasks;
namespace Regulus.Network
{
    public class PackageSender
    {

        readonly static AsyncConsumer<Task> _Consumer = new AsyncConsumer<Task>();
        private readonly IStreamable _Stream;
        readonly IPool _Pool;
        public PackageSender(IStreamable stream , Regulus.Memorys.IPool pool)
        {
            
            
            _Stream = stream;
            _Pool = pool;
        }

        

        private async Task _SendBufferAsync(Buffer buffer)
        {
            var sendCount = 0;
            sendCount += await _Stream.Send(buffer.Bytes.Array, buffer.Bytes.Offset, buffer.Count);
            while (sendCount < buffer.Count)
            {                
                sendCount += await _Stream.Send(buffer.Bytes.Array, buffer.Bytes.Offset + sendCount, buffer.Count - sendCount);
            }
        }

        public void Send(Regulus.Memorys.Buffer buffer)
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

        private void _Push(Memorys.Buffer buffer)
        {
            _Consumer.Enqueue(_SendBufferAsync(buffer));
        }
    }
}