using Regulus.Network;
using Regulus.Serialization;
using Regulus.Utility;
using System;

namespace Regulus.Remote
{
    public delegate void OnErrorCallback();

    public delegate void OnByteDataCallback(byte[] bytes);
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

        public PackageReader(IInternalSerializable serializer)
        {
            
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
            var readHead = new SocketHeadReader(_Peer);
            _Reader = readHead;
            ISocketReader reader = readHead;
            reader.DoneEvent += _ReadBody;
            reader.ErrorEvent += ErrorEvent;            
            readHead.Read();
            

        }

        private void _ReadBody(byte[] bytes)
        {
            ulong len;
            Regulus.Serialization.Varint.BufferToNumber(bytes, 0, out len);
            int bodySize = (int)len;

            var bodyReader = new SocketBodyReader(_Peer);
            _Reader = bodyReader;
            ISocketReader reader = bodyReader;
            reader.DoneEvent += _Package;
            reader.ErrorEvent += ErrorEvent;
            
            bodyReader.Read(bodySize);
        }

        private void _Package(byte[] bytes)
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
