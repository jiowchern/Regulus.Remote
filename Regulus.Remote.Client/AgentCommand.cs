using System.Linq;
using System.Reflection;

namespace Regulus.Remote.Client
{
    internal class AgentCommand
    {


        internal readonly string Name;
        internal readonly object Target;

        public AgentCommand(AgentCommandVersionProvider provider, System.Type type, MethodStringInvoker invoker)
        {
            Target = invoker.Target;
            Name = $"{type.Name}-{ provider.GetVersion(type, invoker.Method)   }.{invoker.Method.Name} [{_BuildParams(invoker.Method)}]";
        }

        private string _BuildParams(MethodInfo method)
        {
            return string.Join(",", method.GetParameters().Select(p => p.Name));
        }


    }
}
