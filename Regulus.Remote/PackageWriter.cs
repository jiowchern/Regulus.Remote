using Regulus.Network;
using Regulus.Serialization;

using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace Regulus.Remote
{
    public class PackageWriter<TPackage>
    {
        private readonly IInternalSerializable _Serializer;


        private IStreamable _Peer;

        public System.Action ErrorEvent;

        

        public PackageWriter(IInternalSerializable serializer)
        {
        
            _Serializer = serializer;
        
        }
        public void Start(IStreamable peer)
        {            
            _Peer = peer;
        }

        

        private void _WriteEnd(int send_count)
        {
            NetworkMonitor.Instance.Write.Set(send_count);
        }

        public void Push(TPackage package)
        {
            var buffer = _CreateBuffer(ref package);
            if (buffer.Length == 0)
                return;

            _Write(buffer).ContinueWith(t => { }); 
            
        }

        public async void Push(System.Collections.Generic.IEnumerable<TPackage> packages)
        {

            var buffer = _CreateBuffer(packages.ToArray());
            if (buffer.Length == 0)
                return;
            
            await _Write(buffer);            
        }

        private async System.Threading.Tasks.Task _Write(byte[] buffer)
        {
            
            
            
            IWaitableValue<int> wv = null;
            try
            {
                wv = _Peer.Send(buffer, 0, buffer.Length);
            }
            catch (System.Exception ex)
            {
                Regulus.Utility.Log.Instance.WriteInfo(ex.ToString());
                ErrorEvent();
            }

            
            var sendCount = await wv;
            _WriteEnd(sendCount);            
        }

        

        private byte[] _CreateBuffer(ref TPackage package)
        {
            byte[] buffers = _Serializer.Serialize(package);
            using (MemoryStream stream = new MemoryStream())
            {
                int len = buffers.Length;
                int lenCount = Regulus.Serialization.Varint.GetByteCount(len);
                byte[] lenBuffer = new byte[lenCount];
                Regulus.Serialization.Varint.NumberToBuffer(lenBuffer, 0, len);
                stream.Write(lenBuffer, 0, lenBuffer.Length);
                stream.Write(buffers, 0, buffers.Length);
                return stream.ToArray();
            }
        }
        private byte[] _CreateBuffer(TPackage[] packages)
        {
            IEnumerable<byte[]> buffers = from p in packages select _Serializer.Serialize(p);

            // Regulus.Utility.Log.Instance.WriteDebug(string.Format("Serialize to Buffer size {0}", buffers.Sum( b => b.Length )));
            using (MemoryStream stream = new MemoryStream())
            {
                foreach (byte[] buffer in buffers)
                {
                    int len = buffer.Length;
                    int lenCount = Regulus.Serialization.Varint.GetByteCount(len);
                    byte[] lenBuffer = new byte[lenCount];
                    Regulus.Serialization.Varint.NumberToBuffer(lenBuffer, 0, len);
                    stream.Write(lenBuffer, 0, lenBuffer.Length);
                    stream.Write(buffer, 0, buffer.Length);
                }

                return stream.ToArray();
            }
        }

        public void Stop()
        {
                 
        }

    }
}
