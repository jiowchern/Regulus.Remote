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
                Regulus.Utility.Log.Instance.WriteInfo(string.Format("connect stage enter."));                    
                Regulus.Utility.Log.Instance.WriteInfo(string.Format("Agent connect start .")); 
                
                try
                {
                    //_Socket.SetSocketOption(System.Net.Sockets.SocketOptionLevel.Socket, System.Net.Sockets.SocketOptionName.ReuseAddress, true);
                    //_Socket.Bind(new System.Net.IPEndPoint(System.Net.IPAddress.Any, 42255));
                    _AsyncResult = _Socket.BeginConnect(_Ipaddress, _Port, _ConnectResult , null);
                }                
                catch (System.Exception e)
                {
                    Regulus.Utility.Log.Instance.WriteInfo(string.Format("begin connect fail {0}.", e.ToString()));
                    ResultEvent(false , null);
                }
                finally
                {
                    Regulus.Utility.Log.Instance.WriteInfo(string.Format("agent connect started .")); 
                }
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
                    Regulus.Utility.Log.Instance.WriteInfo(ex.ToString());
                }
                catch (ObjectDisposedException ode)
                {
                    Regulus.Utility.Log.Instance.WriteInfo(ode.ToString());
                }
                finally
                {
                    _Result = result;
                    Regulus.Utility.Log.Instance.WriteInfo(string.Format("connect result {0}.", _Result));                    
                }
                
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
                if (_Result.HasValue == false && ResultEvent != null)
                {
                    
                    var call = ResultEvent;
                    ResultEvent = null;
                    call(false, null);                    
                }

                if(_Result.HasValue && _Result.Value == false)
                    _Socket.Close();                
                Regulus.Utility.Log.Instance.WriteInfo(string.Format("Agent connect leave."));                    
            }

            void Utility.IStage.Update()
            {
                _InvokeResultEvent();
            }
        }

    }
}
