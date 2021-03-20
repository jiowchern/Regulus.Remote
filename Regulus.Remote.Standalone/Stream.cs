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
            _ReadyReceive = new System.Threading.ManualResetEvent(true);
            _ReadySend = new System.Threading.ManualResetEvent(true);
            _Sends = new System.Collections.Generic.Queue<byte>();
            _Receives = new System.Collections.Generic.Queue<byte>();

        }

        public Task<int> Push(byte[] buffer, int offset, int count)
        {            

            return System.Threading.Tasks.Task<int>.Factory.StartNew(() => {
                lock (_Receives)
                {
                    int readCount = 0;
                    for (int i = offset; i < buffer.Length; i++)
                    {
                        int index = i - offset;
                        if (index >= count)
                        {
                            if (readCount > 0)
                                _ReadyReceive.Set();
                            return readCount;
                        }
                        readCount++;
                        _Receives.Enqueue(buffer[i]);
                    }
                    if (readCount > 0)
                        _ReadyReceive.Set();
                    return readCount;
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
                    int readCount = 0;
                    for (int i = offset; i < buffer.Length; i++)
                    {
                        int index = i - offset;
                        if (index >= count)
                            return readCount;
                        if (_Receives.Count == 0)
                        {
                            _ReadyReceive.Reset();
                            return readCount;
                        }
                        readCount++;
                        buffer[i] = _Receives.Dequeue();
                    }
                    return readCount;
                }

                
            });
        }

        private Task<int> _Write(byte[] buffer, int offset, int count)
        {
            


            return System.Threading.Tasks.Task<int>.Factory.StartNew(()=> {
                _ReadySend.WaitOne();
                lock (_Sends)
                {
                    int readCount = 0;
                    for (int i = offset; i < buffer.Length; i++)
                    {
                        int index = i - offset;
                        if (index >= count)
                            return readCount;
                        if (_Sends.Count == 0)
                        {
                            _ReadySend.Reset();
                            return readCount;
                        }
                        readCount++;
                        buffer[i] = _Sends.Dequeue();
                    }
                    return readCount;
                }

                
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
                    int readCount = 0;
                    
                    for (int i = offset; i < buffer.Length; i++)
                    {
                        int index = i - offset;
                        if (index >= count)
                        {
                            if(readCount > 0)
                                _ReadySend.Set();
                            return readCount;
                        }
                        readCount++;
                        _Sends.Enqueue(buffer[i]);
                    }
                    if (readCount > 0)
                        _ReadySend.Set();
                    return readCount;
                }
            });
        }
    }
}
