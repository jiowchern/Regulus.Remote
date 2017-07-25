using Regulus.Network.RUDP;
using Regulus.Utility;

namespace Regulus.Network.Tests.AgentApp
{
    internal class AgentConsole : Regulus.Utility.WindowConsole
    {
        readonly Regulus.Utility.StageMachine _Machine;
        private readonly Regulus.Utility.Updater<Timestamp> _Updater;
        private Regulus.Network.ITime _Time;
        private Agent _Agent;

        public AgentConsole()
        {
            _Time = new Time();
            _Agent = Regulus.Network.RUDP.Agent.CreateStandard(_Time.OneSeconds);
            _Updater = new Updater<Timestamp>();
            _Machine = new StageMachine();
            
        }
        protected override void _Launch()
        {
            

            _Updater.Add(_Agent);
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
            _Time.Sample();
            _Updater.Working(new Timestamp(_Time.Now , _Time.Delta));
            _Machine.Update();
        }

        protected override void _Shutdown()
        {
            _Updater.Shutdown();
            _Machine.Termination();
        }
    }
}
