using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regulus.Remoting.Soul.Native
{
    partial class TcpController : Application.IController
    {
        class StageRun : Regulus.Game.IStage
        {
            public event Action ShutdownEvent;
            private Utility.Command _Command;
            private TcpController _Controller;

            Regulus.Game.ICore _Core;
            public StageRun(Regulus.Game.ICore core,Utility.Command command, TcpController tcpcontroller)
            {
                _Controller = tcpcontroller;
                this._Command = command;
                _Core = core;
            }

            void Game.IStage.Enter()
            {
                _Core.Launch();
                _Command.Register("Shutdown", _ShutdownEvent );
            }

            private void _ShutdownEvent()
            {
                _Controller._StopListen();
                ShutdownEvent();
            }

            void Game.IStage.Leave()
            {
                _Core.Shutdown();
                _Command.Unregister("Shutdown");
            }

            void Game.IStage.Update()
            {
                _Controller._HandlePeer(_Controller._Clients, _Controller._NewPeers , _Core);
                _Core.Update();
            }
        }
    }
    
}
