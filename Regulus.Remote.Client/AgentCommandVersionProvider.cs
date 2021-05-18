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
        private readonly System.Collections.Generic.List<TypeMethod> _Counts;
        public AgentCommandVersionProvider()
        {
            _Counts = new System.Collections.Generic.List<TypeMethod>();
        }

        private int _Version(Type type, MethodInfo method_info)
        {
            TypeMethod typeMethod = _Counts.FirstOrDefault((tm) => tm.Type == type && tm.Method == method_info);
            if (typeMethod == null)
            {
                typeMethod = new TypeMethod(type, method_info);
                _Counts.Add(typeMethod);
            }
            return typeMethod.Increase();
        }

        public int GetVersion(Type type, MethodInfo method_info)
        {
            return _Version(type, method_info);
        }
    }
}
