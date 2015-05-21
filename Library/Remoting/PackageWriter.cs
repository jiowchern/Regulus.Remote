using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Remoting.Native
{
    public class PackageWriter
    {
        public delegate Package[] CheckSourceCallback();
        public event CheckSourceCallback CheckSourceEvent;
        const int _HeadSize = 4;
        System.Net.Sockets.Socket _Socket;
        
        private byte[] _Buffer;
        private IAsyncResult _AsyncResult;

        public event OnErrorCallback ErrorEvent;
        public void Start(System.Net.Sockets.Socket socket)
        {
            _Socket = socket;
            

            _Write();
        }

        private void _Write()
        {

            _Buffer = _CreateBuffer(CheckSourceEvent());
            
            try
            {
                _AsyncResult = _Socket.BeginSend(_Buffer, 0, _Buffer.Length, 0, _WriteCompletion, null);
            }
            catch
            {
                ErrorEvent();
            }
        }

        private void _WriteCompletion(IAsyncResult ar)
        {
            try
            {
                _Socket.EndSend(ar);
                if (_Buffer.Length == 0)
                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1.0 / 20.0));


                _Write();
            }
            catch
            {
                ErrorEvent();
            }
            
        }

        byte[] _CreateBuffer(Package[] packages)
        {

            var buffers = from p in packages select Regulus.Serializer.TypeHelper.Serializer<Package>(p);

            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                foreach (var buffer in buffers)
                {
                    stream.Write(System.BitConverter.GetBytes((int)buffer.Length), 0, _HeadSize);
                    stream.Write(buffer, 0, buffer.Length);
                }
                return stream.ToArray();
            }
        }


        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
