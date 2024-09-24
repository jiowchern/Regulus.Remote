using Regulus.Memorys;
using Regulus.Network;
using Regulus.Serialization;

using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace Regulus.Remote
{
    public class PackageWriter<TPackage>
    {
        private readonly IPool _Pool;
        private readonly IInternalSerializable _Serializer;


        private IStreamable _Peer;

        public System.Action ErrorEvent;

        

        public PackageWriter(IInternalSerializable serializer,Regulus.Memorys.IPool pool)
        {
            _Pool = pool;
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
            var buffer = _Serializer.Serialize(package);
            
            var bytes = buffer.Bytes;
            int len = bytes.Count;
            int lenCount = Regulus.Serialization.Varint.GetByteCount(len);
            var lenBuffer = _Pool.Alloc(lenCount);
            var lenBufferBytes = lenBuffer.Bytes;
            Regulus.Serialization.Varint.NumberToBuffer(lenBufferBytes.Array, lenBufferBytes.Offset, len);

            using (var stream = _Pool.Alloc(lenCount + bytes.Count))
            {
                var totalBytes = stream.Bytes;

                //lenBufferBytes.Array.CopyTo(totalBytes.Array, totalBytes.Offset);                
                //bytes.Array.CopyTo(totalBytes.Array, totalBytes.Offset + lenBufferBytes.Count);
                for (int i = 0; i < lenBufferBytes.Count; i++)
                {
                    totalBytes.Array[totalBytes.Offset + i] = lenBufferBytes.Array[lenBufferBytes.Offset + i];
                }
                for (int i = 0; i < bytes.Count; i++)
                {
                    totalBytes.Array[totalBytes.Offset + lenBufferBytes.Count + i] = bytes.Array[bytes.Offset + i];
                }
                buffer.Dispose();
                lenBuffer.Dispose();
                var buf = stream.ToArray();
                stream.Dispose();
                return buf;
            }

        }
        private byte[] _CreateBuffer(TPackage[] packages)
        {
            IEnumerable<Regulus.Memorys.Buffer> buffers = from p in packages select _Serializer.Serialize(p);

            // Regulus.Utility.Log.Instance.WriteDebug(string.Format("Serialize to Buffer size {0}", buffers.Sum( b => b.Length )));
            using (MemoryStream stream = new MemoryStream())
            {
                foreach (var buffer in buffers)
                {
                    var bytes = buffer.Bytes;
                    int len = bytes.Count;
                    int lenCount = Regulus.Serialization.Varint.GetByteCount(len);
                    var lenBuffer = _Pool.Alloc(lenCount);
                    var lenBytes = lenBuffer.Bytes;
                    Regulus.Serialization.Varint.NumberToBuffer(lenBytes.Array, lenBytes.Offset, len);
                    stream.Write(lenBytes.Array, lenBytes.Offset, lenBytes.Count );
                    stream.Write(bytes.Array , bytes.Offset , bytes.Count);
                    lenBuffer.Dispose();
                    buffer.Dispose();
                }

                return stream.ToArray();
            }
        }

        public void Stop()
        {
                 
        }

    }
}
