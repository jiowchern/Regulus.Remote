using System;
using System.Linq;
using System.Reflection;

namespace Regulus.Remote.Client
{

    public class AgentCommandVersionProvider
    {
        private readonly System.Collections.Generic.Dictionary<System.Type, int> _Counts ;
        public AgentCommandVersionProvider()
        {
            _Counts = new System.Collections.Generic.Dictionary<Type, int>();
        }

        private int _Version(Type type)
        {
            if (!_Counts.ContainsKey(type))
                _Counts.Add(type, 0);
            return _Counts[type]++;
        }

        public int GetVersion(Type type)
        {
            return _Version(type);
        }
    }
    internal class AgentCommand
    {

        
        internal readonly string Name;
        internal readonly object Target;

        public AgentCommand(AgentCommandVersionProvider provider,System.Type type , MethodStringInvoker invoker)
        {
            Target = invoker.Target;
            Name = $"{type.Name}-{ provider.GetVersion(type)   }.{invoker.Method.Name} [{_BuildParams(invoker.Method)}]";
        }

        private string _BuildParams(MethodInfo method)
        {
            return string.Join(",", method.GetParameters().Select(p => p.Name));
        }

        
    }
}