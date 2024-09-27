

using Regulus.Memorys;
using System;
using System.Threading.Tasks;
namespace Regulus.Network
{
    public class PackageSender
    {
        private readonly IStreamable _Stream;
        readonly IPool _Pool;
        public PackageSender(IStreamable stream , Regulus.Memorys.IPool pool)
        {
            _Stream = stream;
            _Pool = pool;
        }
        public async Task Send(Regulus.Memorys.Buffer buffer)
        {
            
            var packageVarintCount = Regulus.Serialization.Varint.GetByteCount(buffer.Bytes.Count);
            var sendBuffer = _Pool.Alloc(packageVarintCount + buffer.Bytes.Count);
            var offset = Regulus.Serialization.Varint.NumberToBuffer(sendBuffer.Bytes.Array, sendBuffer.Bytes.Offset, buffer.Bytes.Count);
                        
            for (int i = 0; i < buffer.Bytes.Count; i++)
            {
                sendBuffer.Bytes.Array[sendBuffer.Bytes.Offset + offset + i] = buffer.Bytes.Array[buffer.Bytes.Offset + i];
            }
            offset += buffer.Bytes.Count;
            var sendCount = 0;
            sendCount += await _Stream.Send(sendBuffer.Bytes.Array, sendBuffer.Bytes.Offset, offset);
            while (sendCount < offset)
            {
                sendCount += await _Stream.Send(sendBuffer.Bytes.Array, sendBuffer.Bytes.Offset + sendCount, offset - sendCount);
            }
        }
    }
}