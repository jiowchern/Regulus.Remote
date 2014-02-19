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
            bool _Result;
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
                try
                {
                    _Socket.EndConnect(ar);
                    _Result = true;
                }
                catch (System.Net.Sockets.SocketException ex)
                {
                    _Result = false;
                }
                catch
                {
                    _Result = false;
                }
            }

            void Game.IStage.Leave()
            {

            }

            void Game.IStage.Update()
            {
                if (_AsyncResult.IsCompleted)
                {
                    ResultEvent(_Result);
                }
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
