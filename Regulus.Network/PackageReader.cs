using System;
using System.Linq;
using System.Threading.Tasks;
namespace Regulus.Network
{
    public class PackageReader
    {        
        private readonly IStreamable _Stream;
        private Regulus.Memorys.IPool _Pool;
        public struct StreamSegment
        {
            public ArraySegment<byte>[] Packages;
            public ArraySegment<byte> Unpacket;            
        }
        public PackageReader(IStreamable stream, Regulus.Memorys.IPool pool)
        {            
            this._Stream = stream;
            _Pool = pool;
        }

        public async Task<System.Collections.Generic.List<Regulus.Memorys.Buffer>> Read()
        {
            var buffers = new System.Collections.Generic.List<Regulus.Memorys.Buffer>();
            
            var remainingBuffer = _Pool.Alloc(0);
            while (true)
            {
                var headBuffer = _Pool.Alloc(8);
                

                // 把 remainingBuffer 的內容移到 headBuffer
                for(int i = 0; i < remainingBuffer.Bytes.Count; ++i)
                {
                    headBuffer.Bytes.Array[headBuffer.Bytes.Offset + i] = remainingBuffer.Bytes.Array[remainingBuffer.Bytes.Offset + i];
                }
                var headReadCount = await _Stream.Receive(headBuffer.Bytes.Array, headBuffer.Bytes.Offset + remainingBuffer.Count, headBuffer.Bytes.Count - remainingBuffer.Count);
                headReadCount += remainingBuffer.Bytes.Count;
                remainingBuffer = _Pool.Alloc(0);
                if (headReadCount == 0)
                {
                    return buffers;
                }
                
                var headReadedBuffer = new ArraySegment<byte>(headBuffer.Bytes.Array, headBuffer.Bytes.Offset, headReadCount);                
                var headVarint = Regulus.Serialization.Varint.FindVarint(ref headReadedBuffer);
                if (headVarint.Count == 0)
                {                    
                    throw new SystemException("head size greater than 8.");
                }
                
                
                
                var bodyVarintSize = Regulus.Serialization.Varint.BufferToNumber(headBuffer.Bytes.Array, headBuffer.Bytes.Offset, out int bodySize);
                var remaining = headReadCount - bodyVarintSize; // 多讀出來的數量
                var needReadSize = bodySize - remaining;
                if(needReadSize <= 0)
                {
                    // 不需要再讀取已經可以組出body
                    var bodyBuffer = _Pool.Alloc(bodySize);
                    
                    for(int i = 0; i < remaining + needReadSize; ++i)
                    {
                        bodyBuffer.Bytes.Array[bodyBuffer.Bytes.Offset + i] = headBuffer.Bytes.Array[headBuffer.Bytes.Offset + bodyVarintSize  + i];
                    }
                    if(bodySize>0)
                        buffers.Add(bodyBuffer);
                    if(needReadSize == 0)
                    {
                        return buffers;
                    }

                    // 將剩下的存到remainingBuffer
                    remainingBuffer = _Pool.Alloc(-needReadSize);
                    for (int i = 0; i < -needReadSize; ++i)
                    {
                        remainingBuffer.Bytes.Array[remainingBuffer.Bytes.Offset + i] = headBuffer.Bytes.Array[headBuffer.Bytes.Offset + bodyVarintSize + bodySize + i];
                    }
                    continue;
                }
                else
                {
                    // 需要再讀取
                    var bodyBuffer = _Pool.Alloc(bodySize);
                    
                    var bodyReadCount = 0;

                    // 讀取剩下的body
                    while(bodyReadCount < needReadSize)
                    {
                        bodyReadCount += await _Stream.Receive(bodyBuffer.Bytes.Array, bodyBuffer.Bytes.Offset + remaining + bodyReadCount, bodyBuffer.Bytes.Count - remaining - bodyReadCount);                        
                    }
                    
                    // 將head多讀取出來的部分放到body
                    for(int i = 0; i < remaining; ++i)
                    {
                        var read = headBuffer.Bytes.Array[headBuffer.Bytes.Offset + headBuffer.Bytes.Count - remaining + i];
                        bodyBuffer.Bytes.Array[bodyBuffer.Bytes.Offset + i] = read;
                    }
                    if (bodySize > 0)
                        buffers.Add(bodyBuffer);
                   
                    return buffers;
                }

            }

        }
    }
}