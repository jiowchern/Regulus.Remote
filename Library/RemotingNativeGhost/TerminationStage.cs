using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Remoting.Ghost.Native
{
    public partial class Agent
    {
        class TerminationStage : Regulus.Utility.IStage
        {
            private Remoting.Ghost.Native.Agent agent;

            public TerminationStage(Remoting.Ghost.Native.Agent agent)
            {
                // TODO: Complete member initialization
                this.agent = agent;
            }

            void Utility.IStage.Enter()
            {                
            }

            void Utility.IStage.Leave()
            {
                
            }

            void Utility.IStage.Update()
            {
                
            }
        }
    }
    
}
