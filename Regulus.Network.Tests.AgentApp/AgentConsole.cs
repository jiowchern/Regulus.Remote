using Regulus.Network.RUDP;
using Regulus.Utility;

namespace Regulus.Network.Tests.AgentApp
{
    internal class AgentConsole : Regulus.Utility.WindowConsole
    {
        readonly Regulus.Utility.StageMachine _Machine;
        private readonly Regulus.Utility.Updater<Timestamp> _Updater;
        
        private long _Ticks;
        private Agent _Agent;

        public AgentConsole()
        {
            _Agent = Regulus.Network.RUDP.Agent.CreateStandard();
            _Updater = new Updater<Timestamp>();
            _Machine = new StageMachine();
        }
        protected override void _Launch()
        {
            _Ticks = System.DateTime.Now.Ticks;

            _Updater.Add(_Agent);
            _ToInitial();
        }

        private void _ToInitial()
        {            
            var stage = new InitialStage(_Agent , Command);
            stage.CreatedEvent += _ToConnecting;
            _Machine.Push(stage);            
        }

        private void _ToConnecting(IPeer peer)
        {
            var stage = new ConnectStage(peer, Command);
            stage.SuccessEvent += ()=> { _ToTransmission(peer); };
            _Machine.Push(stage);            
        }

        private void _ToTransmission(IPeer peer)
        {
            var stage = new TransmissionStage(peer, Command,Viewer);
            stage.DisconnectEvent += _ToInitial;
            _Machine.Push(stage);
            
        }

        protected override void _Update()
        {
            var nowTicks = System.DateTime.Now.Ticks;
            var delta = nowTicks - _Ticks;
            _Ticks = nowTicks;

            var time = new Timestamp(nowTicks , delta);            
            _Updater.Working(time);
            _Machine.Update();
        }

        protected override void _Shutdown()
        {
            _Updater.Shutdown();
            _Machine.Termination();
        }
    }
}
