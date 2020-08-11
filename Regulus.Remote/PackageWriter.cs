using Regulus.Network;
using Regulus.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Regulus.Remote
{
    public class PackageWriter<TPackage>
    {
        private readonly ISerializer _Serializer;


        public event OnErrorCallback ErrorEvent;

        private byte[] _Buffer;

        private IStreamable _Peer;

        private volatile bool _Stop;




        public PackageWriter(ISerializer serializer)
        {

            _Serializer = serializer;


        }


        public void Start(IStreamable peer)
        {
            _Stop = false;
            _Peer = peer;

        }

        public void Push(TPackage[] packages)
        {

            _Write(packages);


        }
        private void _Write(TPackage[] packages)
        {
            try
            {

                _Buffer = _CreateBuffer(packages);


                System.Threading.Tasks.Task<int> task = _Peer.Send(_Buffer, 0, _Buffer.Length);
                task.ContinueWith(t => _WriteCompletion(t.Result));




            }
            catch (SystemException e)
            {
                
                if (ErrorEvent != null)
                {
                    ErrorEvent();
                }
            }
        }

        private void _WriteCompletion(int send_count)
        {
            if (_Stop == false)
            {
                int sendSize = send_count;
                NetworkMonitor.Instance.Write.Set(sendSize);
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
            _Stop = true;



        }

    }
}
