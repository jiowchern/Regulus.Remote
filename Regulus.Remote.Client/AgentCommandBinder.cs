using Regulus.Utility;
using System;
using System.Reflection;

namespace Regulus.Remote.Client
{
    public class AgentCommandBinder : IDisposable
    {
        
        private readonly Command _Command;

        public AgentCommandBinder(Command command, AgentEventRectifier rectifier)
        {
            rectifier.SupplyEvent += _Bind;
            this._Command = command;
        }

        private void _Bind(Type type, object instance)
        {

            

        }

        

        void IDisposable.Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
