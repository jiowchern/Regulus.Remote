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

        public async System.Threading.Tasks.Task Push(TPackage package)
        {
            var buffer = _CreateBuffer(ref package);
            var sendCount = await _Write(buffer);
            _WriteEnd(sendCount);
        }



        
        private async System.Threading.Tasks.Task<int> _Write(Regulus.Memorys.Buffer buffer)
        {
            
            try
            {
                var offset = 0;
                
                var bytes = buffer.Bytes;
                while (offset < buffer.Count)
                {

                    offset += await _Peer.Send(bytes.Array, bytes.Offset + offset, bytes.Count - offset);

                }
                
                
                
                return offset;
            }
            catch (System.Exception ex)
            {
                Regulus.Utility.Log.Instance.WriteInfo($"send error {ex.ToString()}.");
                throw ex;
            }

            
            
            
        }

        

        private Regulus.Memorys.Buffer _CreateBuffer(ref TPackage package)
        {
            var buffer = _Serializer.Serialize(package);
            
            var bytes = buffer.Bytes;
            int len = bytes.Count;
            int lenCount = Regulus.Serialization.Varint.GetByteCount(len);
            var lenBuffer = _Pool.Alloc(lenCount);
            var lenBufferBytes = lenBuffer.Bytes;
            Regulus.Serialization.Varint.NumberToBuffer(lenBufferBytes.Array, lenBufferBytes.Offset, len);

            var stream = _Pool.Alloc(lenCount + bytes.Count);
            
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
                
            
            return stream;
            

        }
      

        public void Stop()
        {
                 
        }

    }
}
