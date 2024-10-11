using Regulus.Remote;
using System.Buffers;
using System.Collections.Concurrent;
using System;
using System.Threading;
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

        private readonly ManualResetEvent _ReceivesMre;
        private readonly ManualResetEvent _SendsMre;
        private readonly System.Collections.Generic.Queue<BufferSegment> _Sends;
        private readonly System.Collections.Generic.Queue<BufferSegment> _Receives;
        private readonly ArrayPool<byte> _BufferPool;

        public Stream()
        {            
            _ReceivesMre = new System.Threading.ManualResetEvent(false);
            _SendsMre = new System.Threading.ManualResetEvent(false);
            _Sends = new System.Collections.Generic.Queue<BufferSegment>();
            _Receives = new System.Collections.Generic.Queue<BufferSegment>();
            _BufferPool = ArrayPool<byte>.Shared;
        }

        public IWaitableValue<int> Push(byte[] buffer, int offset, int count)
        {
            // 租借一个缓冲区
            byte[] pooledBuffer = _BufferPool.Rent(count);
            Buffer.BlockCopy(buffer, offset, pooledBuffer, 0, count);

            // 将缓冲区加入队列
            lock(_Receives)
            {
                _Receives.Enqueue(new BufferSegment(pooledBuffer, 0, count));
                System.Threading.SpinWait.SpinUntil(() => _ReceivesMre.Set());
            }
            
            return new NoWaitValue<int>(count);
        }

        public IWaitableValue<int> Pop(byte[] buffer, int offset, int count)
        {
            
            return Dequeue(_Sends, buffer, offset, count, _SendsMre);
        }

        IWaitableValue<int> IStreamable.Send(byte[] buffer, int offset, int count)
        {
            // 租借一个缓冲区
            byte[] pooledBuffer = _BufferPool.Rent(count);
            Buffer.BlockCopy(buffer, offset, pooledBuffer, 0, count);

            // 将缓冲区加入队列
            lock(_Sends)
            {
                _Sends.Enqueue(new BufferSegment(pooledBuffer, 0, count));
                System.Threading.SpinWait.SpinUntil(() => _SendsMre.Set());
            }
            
            
            return new NoWaitValue<int>(count);
        }

        IWaitableValue<int> IStreamable.Receive(byte[] buffer, int offset, int count)
        {
            
            return Dequeue(_Receives, buffer, offset, count, _ReceivesMre);
        }

        private IWaitableValue<int> Dequeue(System.Collections.Generic.Queue<BufferSegment> queue, byte[] buffer, int offset, int count, ManualResetEvent mre)
        {
            mre.WaitOne();
            int totalRead = 0;
            lock(queue)
            {
                while (totalRead < count && queue.Count > 0)
                {
                    var segment = queue.Peek();
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
                        queue.Dequeue();
                        _BufferPool.Return(segment.Buffer);
                    }
                }

                if (queue.Count == 0)
                {
                    System.Threading.SpinWait.SpinUntil(() => mre.Reset());
                }
            }
           

            return new NoWaitValue<int>(totalRead);
        }
    }
}