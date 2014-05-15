using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Remoting.Ghost.Native
{
    public partial class Agent 
    {
        class ConnectStage : Regulus.Game.IStage
        {
            private System.Net.Sockets.Socket _Socket;
            private string _Ipaddress;
            private int _Port;
            IAsyncResult _AsyncResult;
            bool? _Result;
            public event Action<bool> ResultEvent;
            public ConnectStage(System.Net.Sockets.Socket socket, string ipaddress, int port)
            {
                // TODO: Complete member initialization
                this._Socket = socket;
                this._Ipaddress = ipaddress;
                this._Port = port;
            }

            void Game.IStage.Enter()
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

            bool _InvokeResultEvent()
            {

                if (_Result.HasValue && ResultEvent != null)
                {
                    ResultEvent(_Result.Value);
                    ResultEvent = null;
                }

                return false;
            }
            void Game.IStage.Leave()
            {
                if (_Result.HasValue == false)
                {
                    ResultEvent(false);
                }
                
            }

            void Game.IStage.Update()
            {
                _InvokeResultEvent();
            }
        }

        class IdleStage : Regulus.Game.IStage
        {

            void Game.IStage.Enter()
            {

            }

            void Game.IStage.Leave()
            {

            }

            void Game.IStage.Update()
            {

            }
        }


    }
}
