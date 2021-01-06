using Regulus.Network;
using System.IO;
using System.Threading.Tasks;
namespace Regulus.Remote.Standalone
{


    public class Stream : Network.IStreamable
    {
        readonly new System.Collections.Generic.Queue<byte> _Sends;
        
        readonly System.Collections.Generic.Queue<byte> _Receives;
        

        public Stream()
        {

            _Sends = new System.Collections.Generic.Queue<byte>();
            _Receives = new System.Collections.Generic.Queue<byte>();

        }

        public Task<int> Push(byte[] buffer, int offset, int count)
        {
            lock(_Receives)
            {
                for (int i = offset; i < buffer.Length ; i++)
                {
                    int readCount = i - offset;
                    if (readCount >= count)
                        return System.Threading.Tasks.Task<int>.Factory.StartNew(() => readCount);
                    _Receives.Enqueue(buffer[i]);
                }
                return System.Threading.Tasks.Task<int>.Factory.StartNew(() => buffer.Length - offset);
            }
            
            
        }




        Task<int> IStreamable.Receive(byte[] buffer, int offset, int count)
        {
            
            return _Read(buffer, offset, count);
        }

        private Task<int> _Read(byte[] buffer, int offset, int count)
        {
            lock (_Receives)
            {
                for (int i = offset; i < buffer.Length; i++)
                {
                    int readCount = i - offset;
                    if(readCount >= count)
                        return System.Threading.Tasks.Task<int>.Factory.StartNew(() => readCount);
                    if (_Receives.Count == 0)
                    {
                        return System.Threading.Tasks.Task<int>.Factory.StartNew(() => readCount);
                    }
                    buffer[i] = _Receives.Dequeue();
                }
            }

            return System.Threading.Tasks.Task<int>.Factory.StartNew(() => buffer.Length - offset);
        }

        private Task<int> _Write(byte[] buffer, int offset, int count)
        {
            lock (_Sends)
            {
                for (int i = offset; i < buffer.Length; i++)
                {
                    int readCount = i - offset;
                    if (readCount >= count)
                        return System.Threading.Tasks.Task<int>.Factory.StartNew(() => readCount);
                    if (_Sends.Count == 0)
                    {
                        return System.Threading.Tasks.Task<int>.Factory.StartNew(() => readCount);
                    }
                    buffer[i] = _Sends.Dequeue();
                }
            }

            return System.Threading.Tasks.Task<int>.Factory.StartNew(() => buffer.Length - offset);
        }

        public Task<int> Pop(byte[] buffer, int offset, int count)
        {
            return _Write(buffer, offset, count);


        }

        Task<int> IStreamable.Send(byte[] buffer, int offset, int count)
        {
            lock (_Sends)
            {
                for (int i = offset; i < buffer.Length; i++)
                {
                    int readCount = i - offset;
                    if (readCount >= count)
                        return System.Threading.Tasks.Task<int>.Factory.StartNew(() => readCount);
                    _Sends.Enqueue(buffer[i]);
                }
                return System.Threading.Tasks.Task<int>.Factory.StartNew(() => buffer.Length - offset);
            }
        }
    }
}
