using Regulus.Network;
using Regulus.Utility;
using System;
using System.Globalization;

namespace Regulus.Remote.Soul.Console
{
    public class Application : WindowConsole
    {
        private readonly  StatusMachine _Machine;


        public Application(IEntry core, IProtocol protocol, int port, IListenable server)
        {
            
            _Machine = new StatusMachine();

            _ToRun(core , protocol , port , server);
        }

        protected override void _Launch()
        {
            
            
        }

        protected override void _Update()
        {
            _Machine.Update();
        }

        protected override void _Shutdown()
        {
            _Machine.Termination();
        }

        

        private void _ToRun(IEntry core, IProtocol protocol, int port, IListenable server)
        {
            var stage = new StageRun(core, protocol, Command, port, Viewer, server);
            stage.ShutdownEvent += _ToEnd;
            _Machine.Push(stage);
        }

        private void _ToEnd()
        {
            Command.Run("quit" , new string[0]);
        }
    }
}
