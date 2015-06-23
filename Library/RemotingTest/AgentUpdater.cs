using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemotingTest
{
    class AgentUpdater
    {
        private Regulus.Remoting.IAgent _Agent;

        public AgentUpdater(Regulus.Remoting.IAgent agent)
        {        
            this._Agent = agent;
        }

        internal void Run()
        {
            _Agent.Launch();
            
            bool enable = true;
            _Agent.BreakEvent += () => enable = false;
            while (enable)
            {
                _Agent.Update();
            }
            _Agent.Shutdown();
        }
    }
}
