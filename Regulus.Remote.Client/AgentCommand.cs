using System;
using System.Linq;
using System.Reflection;

namespace Regulus.Remote.Client
{

    public class AgentCommandVersionProvider
    {
        class TypeMethod
        {
            public readonly Type Type;
            public readonly MethodInfo Method;
            int _Count;

            public TypeMethod(Type type, MethodInfo method_info)
            {
                Type = type;
                Method = method_info;
            }
            public int Increase()
            {
                return _Count++;
            }
        }
        private readonly System.Collections.Generic.List<TypeMethod> _Counts ;
        public AgentCommandVersionProvider()
        {
            _Counts = new System.Collections.Generic.List<TypeMethod>();
        }

        private int _Version(Type type, MethodInfo method_info)
        {
            var typeMethod = _Counts.FirstOrDefault((tm) => tm.Type == type && tm.Method == method_info);
            if(typeMethod == null)
            {
                typeMethod = new TypeMethod(type, method_info);
                _Counts.Add(typeMethod);
            }
            return typeMethod.Increase();
        }

        public int GetVersion(Type type,MethodInfo method_info)
        {
            return _Version(type, method_info);
        }
    }
    internal class AgentCommand
    {

        
        internal readonly string Name;
        internal readonly object Target;

        public AgentCommand(AgentCommandVersionProvider provider,System.Type type , MethodStringInvoker invoker)
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
