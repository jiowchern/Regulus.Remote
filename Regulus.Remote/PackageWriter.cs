using Regulus.Network;
using Regulus.Serialization;

using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace Regulus.Remote
{
    public class PackageWriter<TPackage>
    {
        private readonly ISerializable _Serializer;


        private IStreamable _Peer;

        public System.Action ErrorEvent;

        public PackageWriter(ISerializable serializer)
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

        public async void Push(System.Collections.Generic.IEnumerable<TPackage>   packages)
        {
            
            var buffer = _CreateBuffer(packages.ToArray());
            if (buffer.Length == 0)
                return;

            IWaitableValue<int> wv = null;
            try
            {
                wv = _Peer.Send(buffer, 0, buffer.Length);
            }
            catch (System.Exception ex)
            {
                Regulus.Utility.Log.Instance.WriteInfo(ex.ToString()) ;
                ErrorEvent();
            }

            if(wv == null)
            {
                return;
            }
            var sendCount = await wv;
            _WriteEnd(sendCount);

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
