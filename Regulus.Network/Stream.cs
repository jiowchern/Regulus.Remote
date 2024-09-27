using Regulus.Remote;
using System.Buffers;
using System.Collections.Concurrent;
using System;
namespace Regulus.Network
{
    public class Stream : Network.IStreamable
    {
        private class BufferSegment
        {
            public byte[] Buffer { get; }
            public int Offset { get; private set; }
            public int Length { get; private set; }

            public BufferSegment(byte[] buffer, int offset, int length)
            {
                Buffer = buffer;
                Offset = offset;
                Length = length;
            }

            public void Advance(int count)
            {
                Offset += count;
                Length -= count;
            }
        }

        private readonly ConcurrentQueue<BufferSegment> _sends;
        private readonly ConcurrentQueue<BufferSegment> _receives;
        private readonly ArrayPool<byte> _bufferPool;

        public Stream()
        {
            _sends = new ConcurrentQueue<BufferSegment>();
            _receives = new ConcurrentQueue<BufferSegment>();
            _bufferPool = ArrayPool<byte>.Shared;
        }

        public IWaitableValue<int> Push(byte[] buffer, int offset, int count)
        {
            // 租借一个缓冲区
            byte[] pooledBuffer = _bufferPool.Rent(count);
            Buffer.BlockCopy(buffer, offset, pooledBuffer, 0, count);

            // 将缓冲区加入队列
            _receives.Enqueue(new BufferSegment(pooledBuffer, 0, count));

            return new NoWaitValue<int>(count);
        }

        public IWaitableValue<int> Pop(byte[] buffer, int offset, int count)
        {
            return Dequeue(_sends, buffer, offset, count);
        }

        IWaitableValue<int> IStreamable.Send(byte[] buffer, int offset, int count)
        {
            // 租借一个缓冲区
            byte[] pooledBuffer = _bufferPool.Rent(count);
            Buffer.BlockCopy(buffer, offset, pooledBuffer, 0, count);

            // 将缓冲区加入队列
            _sends.Enqueue(new BufferSegment(pooledBuffer, 0, count));

            return new NoWaitValue<int>(count);
        }

        IWaitableValue<int> IStreamable.Receive(byte[] buffer, int offset, int count)
        {
            return Dequeue(_receives, buffer, offset, count);
        }

        private IWaitableValue<int> Dequeue(ConcurrentQueue<BufferSegment> queue, byte[] buffer, int offset, int count)
        {
            int totalRead = 0;

            while (totalRead < count && queue.TryPeek(out BufferSegment segment))
            {
                int bytesToCopy = Math.Min(segment.Length, count - totalRead);

                // 使用 Span 进行高效的内存复制
                var sourceSpan = new ReadOnlySpan<byte>(segment.Buffer, segment.Offset, bytesToCopy);
                var destinationSpan = new Span<byte>(buffer, offset + totalRead, bytesToCopy);
                sourceSpan.CopyTo(destinationSpan);

                totalRead += bytesToCopy;
                segment.Advance(bytesToCopy);

                if (segment.Length == 0)
                {
                    // 从队列中移除已消费的缓冲区并归还到池中
                    queue.TryDequeue(out _);
                    _bufferPool.Return(segment.Buffer);
                }
            }

            return new NoWaitValue<int>(totalRead);
        }
    }
}