using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Remoting.Native
{
    public delegate void OnErrorCallback();
    public delegate void OnByteDataCallback(byte[] bytes);
    public delegate void OnPackageCallback(Package package);
    public class PackageReader
    {
        public event OnErrorCallback ErrorEvent;
        public event OnPackageCallback DoneEvent;
        const int _HeadSize = 4;
        System.Net.Sockets.Socket _Socket;
        private byte[] _Buffer;
        private int _Offset;

        SocketReader _Reader;
        enum Status
        {
            Begin, End
        }

        volatile bool _Stop;        

        public void Start(System.Net.Sockets.Socket socket)
        {
            _Stop = false;
            _Socket = socket;            
            _ReadHead();    
        }

        private void _ReadHead()
        {            
            _Reader = new SocketReader(_Socket);
            _Reader.DoneEvent += _ReadBody;
            _Reader.ErrorEvent += ErrorEvent;
            _Reader.Read(_HeadSize);            
        }

        private void _ReadBody(byte[] bytes)
        {
            var bodySize = System.BitConverter.ToInt32(bytes, 0);
            _Reader = new SocketReader(_Socket);
            _Reader.DoneEvent += _Package;
            _Reader.ErrorEvent += ErrorEvent;
            _Reader.Read(bodySize);            
        }

        private void _Package(byte[] bytes)
        {
            DoneEvent(Regulus.Serializer.TypeHelper.Deserialize<Package>(bytes));

            if(_Stop == false)
                _ReadHead();
        }

        public void Stop()
        {
            _Stop = true;
            _Socket = null; 
        }
    }

    class SocketReader
    {
        private System.Net.Sockets.Socket _Socket;
        private byte[] _Buffer;
        private int _Offset;
        public event OnErrorCallback ErrorEvent;
        public event OnByteDataCallback DoneEvent;
        public SocketReader(System.Net.Sockets.Socket _Socket)
        {            
            this._Socket = _Socket;
        }


        internal void Read(int size)
        {
            _Offset = 0;
            _Buffer = new byte[size];
            try
            {
                _Socket.BeginReceive(_Buffer, _Offset, _Buffer.Length - _Offset, 0, _Readed, null);
            }
            catch
            {
                if (ErrorEvent != null)
                    ErrorEvent();
            }
        }

        private void _Readed(IAsyncResult ar)
        {
            try
            {
                var readSize = _Socket.EndReceive(ar);
                _Offset += readSize;

                if (readSize == 0)
                    ErrorEvent();
                else if (_Offset == _Buffer.Length)
                    DoneEvent(_Buffer);
                else
                    _Socket.BeginReceive(_Buffer, _Offset, _Buffer.Length - _Offset, 0, _Readed, null);
            }
            catch
            {
                if (ErrorEvent != null)
                    ErrorEvent();
            }
            

        }
    }
}
