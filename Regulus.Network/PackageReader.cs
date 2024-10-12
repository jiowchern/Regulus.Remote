using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;

namespace Regulus.Network
{
    public class PackageReader
    {
        private readonly IStreamable _Stream;
        private readonly Regulus.Memorys.IPool _Pool;
        private readonly Memorys.Buffer _Empty;
        
        public struct Package
        {
            public Regulus.Memorys.Buffer Buffer;
            public ArraySegment<byte> Segment;
        }

        public PackageReader(IStreamable stream, Regulus.Memorys.IPool pool)
        {
                        
            _Stream = stream;
            _Pool = pool;
            _Empty = _Pool.Alloc(0);
        }
        
        public async Task<List<Regulus.Memorys.Buffer>> Read()
        {            
            return await ReadBuffers( new Package { Buffer = _Empty, Segment = _Empty.Bytes });
        }
        public async Task<List<Regulus.Memorys.Buffer>> ReadBuffers(Package unfin)
        {
            var packages = new List<Package>();
            var headBuffer = _Pool.Alloc(8);
            _CopyBuffer(unfin.Segment, headBuffer.Bytes, unfin.Segment.Count);
            var headReadCount = await _ReadFromStream( headBuffer.Bytes.Array, offset: headBuffer.Bytes.Offset + unfin.Segment.Count, count: headBuffer.Bytes.Count - unfin.Segment.Count);
            if (headReadCount == 0)
                return new List<Regulus.Memorys.Buffer>();
            headReadCount += unfin.Segment.Count;
            var readedSeg = new ArraySegment<byte>(headBuffer.Bytes.Array, headBuffer.Bytes.Offset, headReadCount);
            int varintPackagesLen = 0;
            var varintPackages = Regulus.Serialization.Varint.FindPackages(readedSeg).ToArray();
            foreach (var pkg in varintPackages)
            {
                varintPackagesLen += pkg.Head.Count + pkg.Body.Count  ;
                packages.Add(new Package { Buffer = headBuffer , Segment = pkg.Body });
            }

            var remaining = new ArraySegment<byte>(readedSeg.Array , readedSeg.Offset + varintPackagesLen , readedSeg.Count - varintPackagesLen);
            
            var remainingHeadVarint = Regulus.Serialization.Varint.FindVarint(ref remaining);
            if (remainingHeadVarint.Count > 0)
            {
                var offset = Regulus.Serialization.Varint.BufferToNumber(remaining.Array, remaining.Offset, out int bodySize);
                var body = _Pool.Alloc(bodySize);
                _CopyBuffer(remaining, offset, body.Bytes, 0, remaining.Count - offset);
                var remainingBodySize = bodySize - (remaining.Count - offset);
                if (remainingBodySize > 0)
                {
                    var readOffset = bodySize - remainingBodySize;
                    var bodyReadCount = 0;
                    while (bodyReadCount < remainingBodySize)
                    {
                        var readed = await _ReadFromStream(body.Bytes.Array, body.Bytes.Offset + readOffset + bodyReadCount, remainingBodySize - bodyReadCount);
                        if(readed == 0)
                            return new List<Regulus.Memorys.Buffer>();
                        bodyReadCount += readed;
                    }
                }
                packages.Add(new Package { Buffer = body, Segment = new ArraySegment<byte>(body.Bytes.Array, body.Bytes.Offset, bodySize) });
                return _Convert(packages).ToList();
            }
            if(varintPackages.Length == 0 )
                throw new SystemException("head size greater than 8.");
            var buffers = _Convert(packages).ToList();
            if (remaining.Count > 0)
            {
                var nestBuffers = await ReadBuffers(new Package { Buffer = headBuffer, Segment = remaining });
                buffers.AddRange(nestBuffers);
            }
            return buffers;            
        }

        IEnumerable<Regulus.Memorys.Buffer> _Convert(IEnumerable<Package> packages)
        {
            foreach (var pkg in packages)
            {
                var buf = _Pool.Alloc(pkg.Segment.Count);
                _CopyBuffer(pkg.Segment, buf.Bytes, pkg.Segment.Count);
                yield return buf;
            }
        }
        /*
        public async Task<List<Regulus.Memorys.Buffer>> Read2()
        {           
            var buffers = new List<Regulus.Memorys.Buffer>();
            var remainingBuffer = _Pool.Alloc(0);
            _IsStop = false;
            while (!_IsStop)
            {
                var headBuffer = _Pool.Alloc(8);
                _CopyBuffer(remainingBuffer.Bytes, headBuffer.Bytes, remainingBuffer.Bytes.Count);
                var headReadCount = await _ReadFromStream(headBuffer.Bytes.Array, headBuffer.Bytes.Offset + remainingBuffer.Count, headBuffer.Bytes.Count - remainingBuffer.Count);                

                if(headReadCount == 0)
                {
                    return buffers;
                }
                //if (headReadCount == 0 && remainingBuffer.Bytes.Count == 0)
                //    return buffers;

                var remainingBufferCount = remainingBuffer.Count;
                headReadCount += remainingBuffer.Count;
                remainingBuffer = _Pool.Alloc(0);


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

            return buffers;
        }*/

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
