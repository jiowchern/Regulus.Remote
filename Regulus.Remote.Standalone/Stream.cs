using Regulus.Network;
using Regulus.Utility;
using System.IO;
using System.Threading.Tasks;
namespace Regulus.Remote.Standalone
{


    public class Stream : Network.IStreamable
    {
        
        readonly System.Collections.Concurrent.ConcurrentQueue<byte> _Sends;
        
        readonly System.Collections.Concurrent.ConcurrentQueue<byte> _Receives;

        

        public Stream()
        {
        
            _Sends = new System.Collections.Concurrent.ConcurrentQueue<byte>();
            _Receives = new System.Collections.Concurrent.ConcurrentQueue<byte>();

        }

        public IWaitableValue<int> Push(byte[] buffer, int offset, int count)
        {
            int readCount = 0;            
            
            for (int i = offset; i < buffer.Length; i++)
            {
                int index = i - offset;
                if (index >= count)
                {                    
                    return new Regulus.Network.NoWaitValue<int>(readCount);
                }
                readCount++;
                _Receives.Enqueue(buffer[i]);
            }
            return new Regulus.Network.NoWaitValue<int>(readCount);
            
        }




        IWaitableValue<int> IStreamable.Receive(byte[] buffer, int offset, int count)
        {
            
            return _Read(buffer, offset, count);
        }

        private IWaitableValue<int> _Read(byte[] buffer, int offset, int count)
        {

            return _Dequeue(_Receives , buffer , offset , count);
        }

        private IWaitableValue<int> _Dequeue(System.Collections.Concurrent.ConcurrentQueue<byte> quque,byte[] buffer, int offset, int count)
        {
            int readCount = 0;
            byte b;
            while (quque.TryDequeue(out b))
            {
                int index = offset + readCount++;
                buffer[index] = b;

                if (readCount == count)
                    return new Regulus.Network.NoWaitValue<int>(readCount);
            }
            return new Regulus.Network.NoWaitValue<int>(readCount);
        }

        private IWaitableValue<int> _Write(byte[] buffer, int offset, int count)
        {
            return _Dequeue(_Sends , buffer , offset , count);
            


        }

        public IWaitableValue<int> Pop(byte[] buffer, int offset, int count)
        {
            return _Write(buffer, offset, count);


        }


        IWaitableValue<int> IStreamable.Send(byte[] buffer, int offset, int count)
        {
            int readCount = 0;

            for (int i = offset; i < buffer.Length; i++)
            {
                int index = i - offset;
                if (index >= count)
                {
                    
                    return new Regulus.Network.NoWaitValue<int>(readCount);
                }
                readCount++;
                _Sends.Enqueue(buffer[i]);
            }

            return new Regulus.Network.NoWaitValue<int>(readCount);

        }
    }
}
