using Regulus.Network;
using Regulus.Serialization;
using Regulus.Utility;

namespace Regulus.Remote
{
    public delegate void OnErrorCallback();

    public delegate void OnByteDataCallback(byte[] bytes);



    public class PackageReader<TPackage>
    {
        private readonly ISerializer _Serializer;

        public delegate void OnRequestPackageCallback(TPackage package);
        public event OnRequestPackageCallback DoneEvent
        {
            add { _DoneEvent += value; }
            remove { _DoneEvent -= value; }
        }

        public event OnErrorCallback ErrorEvent;


        private OnRequestPackageCallback _DoneEvent;

        private ISocketReader _Reader;

        private IStreamable _Peer;

        private volatile bool _Stop;

        public PackageReader(ISerializer serializer)
        {
            _Serializer = serializer;
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
            SocketHeadReader readHead = new SocketHeadReader(_Peer);
            _Reader = readHead;
            _Reader.DoneEvent += _ReadBody;
            _Reader.ErrorEvent += ErrorEvent;
            readHead.Read();
        }

        private void _ReadBody(byte[] bytes)
        {
            ulong len;
            Regulus.Serialization.Varint.BufferToNumber(bytes, 0, out len);
            int bodySize = (int)len;

            SocketBodyReader reader = new SocketBodyReader(_Peer);
            _Reader = reader;
            _Reader.DoneEvent += _Package;
            _Reader.ErrorEvent += ErrorEvent;
            reader.Read(bodySize);
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
