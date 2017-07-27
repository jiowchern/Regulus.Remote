using Regulus.Network.Rudp;
using Regulus.Network.RUDP;
using Regulus.Utility;

namespace Regulus.Network.Tests.AgentApp
{
    internal class AgentConsole : Regulus.Utility.WindowConsole
    {
        readonly Regulus.Utility.StageMachine _Machine;

        
        private ISocketClient _Agent;

        public AgentConsole()
        {
            
            _Agent = new RudpClient();

            _Machine = new StageMachine();
            
        }
        protected override void _Launch()
        {

            _Agent.Launch();


            _ToInitial();
        }

        private void _ToInitial()
        {            
            var stage = new InitialStage(_Agent , Command,this.Viewer);
            stage.CreatedEvent += _ToConnecting;
            _Machine.Push(stage);            
        }

        private void _ToConnecting(ISocket peer)
        {
            var stage = new ConnectStage(peer, Command);
            stage.SuccessEvent += ()=> { _ToTransmission(peer); };
            _Machine.Push(stage);            
        }

        private void _ToTransmission(ISocket peer)
        {
            var stage = new TransmissionStage(peer, Command,Viewer);
            stage.DisconnectEvent += _ToInitial;
            _Machine.Push(stage);
            
        }

        protected override void _Update()
        {
        
        
            _Machine.Update();
        }

        protected override void _Shutdown()
        {
            _Agent.Shutdown();
            _Machine.Termination();
        }
    }
}
