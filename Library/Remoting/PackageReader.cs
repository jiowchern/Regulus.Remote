using System;
using System.Net.Sockets;

using Regulus.Serialization;
using Regulus.Utility;

namespace Regulus.Remoting
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

		private Socket _Socket;

		private volatile bool _Stop;

	    public PackageReader(ISerializer serializer)
	    {
	        _Serializer = serializer;
	    }

	    public void Start(Socket socket)
		{
			Singleton<Log>.Instance.WriteInfo("pakcage read start.");
			_Stop = false;
			_Socket = socket;
			_ReadHead();
		}

		private void _ReadHead()
		{
		    var readHead = new SocketHeadReader(_Socket);
            _Reader = readHead;
			_Reader.DoneEvent += _ReadBody;
			_Reader.ErrorEvent += ErrorEvent;
            readHead.Read();
		}

		private void _ReadBody(byte[] bytes)
		{
		    ulong len;
		    Regulus.Serialization.Varint.BufferToNumber(bytes, 0, out len);
            var bodySize = (int)len;

            var reader = new SocketBodyReader(_Socket);
            _Reader = reader;
			_Reader.DoneEvent += _Package;
			_Reader.ErrorEvent += ErrorEvent;
            reader.Read(bodySize);
		}

		private void _Package(byte[] bytes)
		{
            
            var pkg = (TPackage)_Serializer.Deserialize(bytes);

			_DoneEvent.Invoke(pkg);

			if(_Stop == false)
			{
				_ReadHead();
			}
		}

		public void Stop()
		{
			_Stop = true;
			_Socket = null;

			Singleton<Log>.Instance.WriteInfo("pakcage read stop.");
		}
	}
}
