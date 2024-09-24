using Regulus.Memorys;
using Regulus.Network;
using Regulus.Serialization;
using Regulus.Utility;
using System;
using System.Linq;

namespace Regulus.Remote
{
    public delegate void OnErrorCallback();

    
    public class PackageReader<TPackage>
    {
        private readonly IInternalSerializable _Serializer;

        public delegate void OnRequestPackageCallback(TPackage package);
        public event OnRequestPackageCallback DoneEvent
        {
            add { _DoneEvent += value; }
            remove { _DoneEvent -= value; }
        }

        public event OnErrorCallback ErrorEvent;


        private OnRequestPackageCallback _DoneEvent;


        ISocketReader _Reader;

        private IStreamable _Peer;

        private volatile bool _Stop;
        readonly Regulus.Memorys.IPool _Pool;
        public PackageReader(IInternalSerializable serializer , Regulus.Memorys.IPool pool)
        {
            _Pool = pool;

            _Serializer = serializer;
            _DoneEvent += _Empty;
            ErrorEvent += _Empty;
            
        }
        private void _Empty(TPackage  arg)
        {

        }
        private void _Empty()
        {
            
        }

        public void Start(IStreamable peer)
        {
            Singleton<Log>.Instance.WriteInfo("pakcage read start.");
            _Stop = false;
            _Peer = peer;
            _ReadHead();
        }

        private void _ReadHead()
        {
            var buffer = _Pool.Alloc(8);
            var readHead = new SocketHeadReader(_Peer, buffer);
            _Reader = readHead;
            ISocketReader reader = readHead;
            readHead.DoneEvent += (readed_count , readed_size) => { _ReadBody(buffer, readed_count, readed_size); };
            reader.ErrorEvent += ErrorEvent;            
            readHead.Read();
            

        }

        private void _ReadBody(Regulus.Memorys.Buffer head_buffer,int readed_count,int readed_size)
        {
            var headBytes = head_buffer.Bytes;
            int realBodySize = 0;
            Regulus.Serialization.Varint.BufferToNumber(headBytes.Array, headBytes.Offset, out realBodySize);
            var prevBodyReaded = readed_size - readed_count;

            var bodyReader = new SocketBodyReader(_Peer);
            _Reader = bodyReader;
            ISocketReader reader = bodyReader;
            bodyReader.DoneEvent += (body_buffer) => {

                var needReadFromHeadSize = prevBodyReaded;
                var newBodyBuffer = _Pool.Alloc(realBodySize);
                var newBodyBytes = newBodyBuffer.Bytes;
                for (int i = 0; i < needReadFromHeadSize; i++)
                {
                    newBodyBytes.Array[newBodyBytes.Offset + i] = headBytes.Array[headBytes.Offset + readed_count + i];
                }
                var bodyBytes = body_buffer.Bytes;
                for (int i = 0; i < bodyBytes.Count; i++)
                {
                    newBodyBytes.Array[newBodyBytes.Offset + prevBodyReaded + i] = bodyBytes.Array[bodyBytes.Offset + i];                
                }                
                _Package(newBodyBuffer);
                
            };
            reader.ErrorEvent += ErrorEvent;
            
            var bodySize = realBodySize - prevBodyReaded;
            var bodyBuffer = _Pool.Alloc(bodySize);
            bodyReader.Read(bodyBuffer);
        }

        private void _Package(Regulus.Memorys.Buffer bytes)
        {

            TPackage pkg = (TPackage)_Serializer.Deserialize(bytes);
            
            if (pkg == null)
            {
                ErrorEvent();
                return;
            }            
            _DoneEvent.Invoke(pkg);

            if (_Stop == false)
            {
                _ReadHead();
            }
        }
       
        public void Stop()
        {            
            _Stop = true;
            Singleton<Log>.Instance.WriteInfo("pakcage read stop.");
        }
    }
}
