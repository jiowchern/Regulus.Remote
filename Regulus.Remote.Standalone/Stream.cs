using Regulus.Network;
using Regulus.Utility;
using System.IO;
using System.Threading.Tasks;
namespace Regulus.Remote.Standalone
{


    public class Stream : Network.IStreamable
    {
        
        readonly System.Collections.Generic.Queue<byte> _Sends;
        
        readonly System.Collections.Generic.Queue<byte> _Receives;

        readonly System.Threading.ManualResetEvent _ReadyReceive;
        readonly System.Threading.ManualResetEvent _ReadySend;

        public Stream()
        {
            _ReadyReceive = new System.Threading.ManualResetEvent(false);
            _ReadySend = new System.Threading.ManualResetEvent(false);
            _Sends = new System.Collections.Generic.Queue<byte>();
            _Receives = new System.Collections.Generic.Queue<byte>();

        }

        System.Threading.Tasks.Task<int> _RunTask(int num)
        {
            var t = new Task<int>(() => {
                if (num == 0)
                {
                    System.Threading.Thread.Sleep(1000 / 30);
                }
        
                return num;
            } );
            t.RunSynchronously();
            return t;
        }

        public Task<int> Push(byte[] buffer, int offset, int count)
        {            

            return System.Threading.Tasks.Task<int>.Factory.StartNew(() => {
                lock (_Receives)
                {
                    for (int i = offset; i < buffer.Length; i++)
                    {
                        int readCount = i - offset;
                        if (readCount >= count)
                        {
                            _ReadyReceive.Set();
                            return readCount;
                        }
                            
                        _Receives.Enqueue(buffer[i]);
                    }
                    _ReadyReceive.Set();
                    return buffer.Length - offset;
                }
            } );
        }




        Task<int> IStreamable.Receive(byte[] buffer, int offset, int count)
        {
            
            return _Read(buffer, offset, count);
        }

        private Task<int> _Read(byte[] buffer, int offset, int count)
        {

            
            

            return System.Threading.Tasks.Task<int>.Factory.StartNew(()=> {
                _ReadyReceive.WaitOne();
                lock (_Receives)
                {
                    for (int i = offset; i < buffer.Length; i++)
                    {
                        int readCount = i - offset;
                        if (readCount >= count)
                            return readCount;
                        if (_Receives.Count == 0)
                        {
                            _ReadyReceive.Reset();
                            return readCount;
                        }
                        buffer[i] = _Receives.Dequeue();
                    }
                }

                return buffer.Length - offset;
            });
        }

        private Task<int> _Write(byte[] buffer, int offset, int count)
        {
            


            return System.Threading.Tasks.Task<int>.Factory.StartNew(()=> {
                _ReadySend.WaitOne();
                lock (_Sends)
                {
                    for (int i = offset; i < buffer.Length; i++)
                    {
                        int readCount = i - offset;
                        if (readCount >= count)
                            return readCount;
                        if (_Sends.Count == 0)
                        {
                            _ReadySend.Reset();
                            return readCount;
                        }
                        buffer[i] = _Sends.Dequeue();
                    }
                }

                return buffer.Length - offset;
            });
        }

        public Task<int> Pop(byte[] buffer, int offset, int count)
        {
            return _Write(buffer, offset, count);


        }

        Task<int> IStreamable.Send(byte[] buffer, int offset, int count)
        {
            

            return System.Threading.Tasks.Task<int>.Factory.StartNew(()=> {
                lock (_Sends)
                {
                    for (int i = offset; i < buffer.Length; i++)
                    {
                        int readCount = i - offset;
                        if (readCount >= count)
                        {
                            _ReadySend.Set();
                            return readCount;
                        }
                            
                        _Sends.Enqueue(buffer[i]);
                    }
                    _ReadySend.Set();
                    return buffer.Length - offset;
                }
            });
        }
    }
}
