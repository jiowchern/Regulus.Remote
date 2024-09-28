using System;
using System.Linq;
using System.Net.Sockets;

namespace Regulus.Network.Tcp
{
    using Regulus.Remote;

    public class SockerTransactor
    {
        public delegate IAsyncResult OnStart(byte[] buffer, int offset, int size, SocketFlags socketFlags, out SocketError errorCode, AsyncCallback callback, object state);
        static readonly SocketError[] _SafeList = new[] { SocketError.Success, SocketError.IOPending };
        OnStart _StartHandler;

        public delegate int OnEnd(IAsyncResult arg);
        OnEnd _EndHandler;

        event Action<SocketError> _SocketErrorEvent;
        public event Action<SocketError> SocketErrorEvent
        {
            add
            {
                _SocketErrorEvent += value;
            }

            remove
            {
                _SocketErrorEvent -= value;
            }
        }

        public SockerTransactor(OnStart start , OnEnd end )
        {
            _StartHandler = start;
            _EndHandler = end;
            _SocketErrorEvent += (e) => { };
        }
         
        
        public IWaitableValue<int> Transact(byte[] readed_byte, int offset, int count)
        {
            SocketError error;
            var ar = _StartHandler(readed_byte, offset, count, SocketFlags.None, out error, _EndDummy, null);

            
            if (!_SafeList.Any(s => s == error))
                _SocketErrorEvent(error);

            if (ar == null)
                return (0).ToWaitableValue();

            return System.Threading.Tasks.Task<int>.Factory.FromAsync(ar, (a)=> { return _EndHandler(a); }).ToWaitableValue();
        }
        private void _EndDummy(IAsyncResult arg)
        {

        }
    }


}