using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Remoting.Ghost.Native
{
    public partial class Agent 
    {
        class ConnectStage : Regulus.Utility.IStage
        {
            private System.Net.Sockets.Socket _Socket;
            private string _Ipaddress;
            private int _Port;
            IAsyncResult _AsyncResult;
            bool? _Result;
            public event Action<bool, System.Net.Sockets.Socket> ResultEvent;
            public ConnectStage( string ipaddress, int port)
            {
                _Socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
                _Socket.NoDelay = true;
                if (ipaddress == null)
                    throw new ArgumentNullException();                
                this._Ipaddress = ipaddress;
                this._Port = port;
            }

            void Utility.IStage.Enter()
            {
                _AsyncResult = _Socket.BeginConnect(_Ipaddress, _Port, _ConnectResult, null);
            }

            private void _ConnectResult(IAsyncResult ar)
            {
                bool result = false;
                try
                {
                    
                    _Socket.EndConnect(ar);
                    result = true;
                }
                catch (System.Net.Sockets.SocketException ex)
                {
                    
                }
                catch (ObjectDisposedException ode)
                {
                    
                }
                catch 
                {
                    
                }
                _Result = result;
            }

            void _InvokeResultEvent()
            {

                if (_Result.HasValue && ResultEvent != null)
                {
                    var call = ResultEvent;
                    ResultEvent = null;
                    call(_Result.Value, _Socket);                    
                }

                
            }
            void Utility.IStage.Leave()
            {

            }

            void Utility.IStage.Update()
            {
                _InvokeResultEvent();
            }
        }

    }
}
