using Regulus.Network;
using System.IO;
using System.Threading.Tasks;
namespace Regulus.Remote.Standalone
{


    public class Stream : Network.IStreamable
    {
        readonly System.Collections.Generic.Queue<byte> _Sends;
        
        readonly System.Collections.Generic.Queue<byte> _Receives;
        

        public Stream()
        {

            _Sends = new System.Collections.Generic.Queue<byte>();
            _Receives = new System.Collections.Generic.Queue<byte>();

        }

        System.Threading.Tasks.Task<int> _RunTask(int num)
        {
            var t = new Task<int>(() => {
                if (num == 0)
                    System.Threading.Thread.Sleep(1000/30);
                return num;
            } );
            t.RunSynchronously();
            return t;
        }

        public Task<int> Push(byte[] buffer, int offset, int count)
        {
            lock(_Receives)
            {
                for (int i = offset; i < buffer.Length ; i++)
                {
                    int readCount = i - offset;
                    if (readCount >= count)
                        return _RunTask(readCount);
                    _Receives.Enqueue(buffer[i]);
                }
                return _RunTask(buffer.Length - offset);
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
                        return _RunTask(readCount);
                    if (_Receives.Count == 0)
                    {
                        return _RunTask(readCount);
                    }
                    buffer[i] = _Receives.Dequeue();
                }
            }

            return _RunTask(buffer.Length - offset);
        }

        private Task<int> _Write(byte[] buffer, int offset, int count)
        {
            lock (_Sends)
            {
                for (int i = offset; i < buffer.Length; i++)
                {
                    int readCount = i - offset;
                    if (readCount >= count)
                        return _RunTask(readCount);
                    if (_Sends.Count == 0)
                    {
                        return _RunTask(readCount);
                    }
                    buffer[i] = _Sends.Dequeue();
                }
            }

            return _RunTask(buffer.Length - offset);
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
                        return _RunTask(readCount);
                    _Sends.Enqueue(buffer[i]);
                }
                
                return _RunTask(buffer.Length - offset);
            }
        }
    }
}
