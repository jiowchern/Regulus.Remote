using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Regulus.Network
{
    public class PackageReader
    {
        private readonly IStreamable _Stream;
        private readonly Regulus.Memorys.IPool _Pool;

        public struct StreamSegment
        {
            public ArraySegment<byte>[] Packages;
            public ArraySegment<byte> Unpacket;
        }

        public PackageReader(IStreamable stream, Regulus.Memorys.IPool pool)
        {
            _Stream = stream;
            _Pool = pool;
        }

        public async Task<List<Regulus.Memorys.Buffer>> Read()
        {
            var buffers = new List<Regulus.Memorys.Buffer>();
            var remainingBuffer = _Pool.Alloc(0);

            while (true)
            {
                var headBuffer = _Pool.Alloc(8);
                _CopyBuffer(remainingBuffer.Bytes, headBuffer.Bytes, remainingBuffer.Bytes.Count);
                var headReadCount = await _ReadFromStream(headBuffer.Bytes.Array, headBuffer.Bytes.Offset + remainingBuffer.Count, headBuffer.Bytes.Count - remainingBuffer.Count);
                headReadCount += remainingBuffer.Bytes.Count;
                remainingBuffer = _Pool.Alloc(0);

                if (headReadCount == 0)
                    return buffers;

                var headReadedBuffer = new ArraySegment<byte>(headBuffer.Bytes.Array, headBuffer.Bytes.Offset, headReadCount);
                var headVarint = Regulus.Serialization.Varint.FindVarint(ref headReadedBuffer);

                if (headVarint.Count == 0)
                    throw new SystemException("head size greater than 8.");

                var bodyVarintSize = Regulus.Serialization.Varint.BufferToNumber(headBuffer.Bytes.Array, headBuffer.Bytes.Offset, out int bodySize);
                var remaining = headReadCount - bodyVarintSize;
                var needReadSize = bodySize - remaining;

                if (needReadSize <= 0)
                {
                    var bodyBuffer = _Pool.Alloc(bodySize);
                    _CopyBuffer(headBuffer.Bytes, bodyVarintSize, bodyBuffer.Bytes, 0, bodySize);

                    if (bodySize > 0)
                        buffers.Add(bodyBuffer);

                    if (needReadSize == 0)
                        return buffers;

                    remainingBuffer = _Pool.Alloc(-needReadSize);
                    _CopyBuffer(headBuffer.Bytes, bodyVarintSize + bodySize, remainingBuffer.Bytes, 0, -needReadSize);
                    continue;
                }
                else
                {
                    var bodyBuffer = _Pool.Alloc(bodySize);
                    _CopyBuffer(headBuffer.Bytes, headReadCount - remaining, bodyBuffer.Bytes, 0, remaining);

                    var bodyReadCount = 0;
                    while (bodyReadCount < needReadSize)
                    {
                        bodyReadCount += await _ReadFromStream(bodyBuffer.Bytes.Array, bodyBuffer.Bytes.Offset + remaining + bodyReadCount, needReadSize - bodyReadCount);
                    }

                    if (bodySize > 0)
                        buffers.Add(bodyBuffer);

                    return buffers;
                }
            }
        }

        private void _CopyBuffer(ArraySegment<byte> source, ArraySegment<byte> destination, int count)
        {
            Array.Copy(source.Array, source.Offset, destination.Array, destination.Offset, count);
        }

        private void _CopyBuffer(ArraySegment<byte> source, int sourceOffset, ArraySegment<byte> destination, int destOffset, int count)
        {
            Array.Copy(source.Array, source.Offset + sourceOffset, destination.Array, destination.Offset + destOffset, count);
        }

        private async Task<int> _ReadFromStream(byte[] buffer, int offset, int count)
        {
            return await _Stream.Receive(buffer, offset, count);
        }
    }
}
