using Regulus.Network;
using Regulus.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Regulus.Remote
{
    public class PackageWriter<TPackage>
    {
        private readonly ISerializer _Serializer;


        private IStreamable _Peer;

        private volatile bool _Stop;
        
        
        System.Collections.Concurrent.ConcurrentQueue<TPackage[]> _SendPkgs;

        public PackageWriter(ISerializer serializer)
        {

            _Serializer = serializer;
            _SendPkgs = new System.Collections.Concurrent.ConcurrentQueue<TPackage[]>();
        }

       

        public void Start(IStreamable peer)
        {
            _Stop = false;
            _Peer = peer;
            

        }

        public  void Update()
        {
            
            TPackage[] pkgs;
            if(_SendPkgs.TryDequeue(out pkgs))
            {
                var buffer = _CreateBuffer(pkgs);

                var resultTask = _Peer.Send(buffer, 0, buffer.Length);
                resultTask.ValueEvent +=_WriteEnd;
            }
            
        }

        private void _WriteEnd(int send_count)
        {
            NetworkMonitor.Instance.Write.Set(send_count);
        }

        public void Push(TPackage[] packages)
        {

            _SendPkgs.Enqueue(packages);
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
            _Stop = true;            
        }

    }
}
