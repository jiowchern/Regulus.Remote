using Regulus.Network;
using Regulus.Utility;

namespace Regulus.Remote.Soul.Console
{
    public class Application : WindowConsole
    {
        private StatusMachine _Machine;

        private readonly string[] _Args;

        public Application(string[] args)
        {
            _Args = args;
        }

        protected override void _Launch()
        {
            _Machine = new StatusMachine();
            _ToStart();
        }

        protected override void _Update()
        {
            _Machine.Update();
        }

        protected override void _Shutdown()
        {
            _Machine.Termination();
        }

        private void _ToStart()
        {
            var stage = new StageStart(Command, Viewer, _Args);
            stage.DoneEvent += _ToRun;
            _Machine.Push(stage);
        }

        private void _ToRun(IEntry core, IProtocol protocol, int port, IListenable server)
        {
            var stage = new StageRun(core, protocol, Command, port, Viewer, server);
            stage.ShutdownEvent += _ToStart;
            _Machine.Push(stage);
        }
    }
}
