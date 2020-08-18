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

        
        public event OnErrorCallback ErrorEvent;

        private byte[] _Buffer;

        private IStreamable _Peer;

        private volatile bool _Stop;

        
        readonly System.Threading.Tasks.Task _Task;
        System.Collections.Concurrent.ConcurrentQueue<TPackage[]> _SendPkgs;

        public PackageWriter(ISerializer serializer)
        {

            _Serializer = serializer;
        
            _Task = new System.Threading.Tasks.Task(_Run,System.Threading.Tasks.TaskCreationOptions.LongRunning);
            _SendPkgs = new System.Collections.Concurrent.ConcurrentQueue<TPackage[]>();


        }

        private void _Run()
        {
            var apr = new Regulus.Utility.AutoPowerRegulator(new Utility.PowerRegulator());
            while(!_Stop)
            {
                apr.Operate();
                TPackage[] pkgs;
                while (_SendPkgs.TryDequeue(out pkgs))
                {
                    var buffer = _CreateBuffer(pkgs);

                    var resultTask = _Peer.Send(buffer, 0, buffer.Length);
                    
                    var sendSize = resultTask.GetAwaiter().GetResult();
                    NetworkMonitor.Instance.Write.Set(sendSize);
                }
                
               
            }
        }

        public void Start(IStreamable peer)
        {
            _Stop = false;
            _Peer = peer;

            _Task.Start();
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
            _Task.Wait();
        }

    }
}
