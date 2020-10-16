using Regulus.Utility;
using System;
using System.Linq;
using System.Reflection;

namespace Regulus.Remote.Client
{
    public class AgentCommandRegister
    {
        readonly System.Collections.Generic.List<AgentCommand> _Invokers;

        private readonly Command _Command;
        private readonly TypeConverterSet _TypeConverterSet;
        private readonly AgentCommandVersionProvider _VersionProvider;

        public AgentCommandRegister(Command command, TypeConverterSet set)
        {
            _VersionProvider = new AgentCommandVersionProvider();
            _Invokers = new System.Collections.Generic.List<AgentCommand>();
            this._Command = command;
            this._TypeConverterSet = set;
        }


        public void Regist(Type type, object instance)
        {

            foreach (MethodInfo method in type.GetMethods())
            {

                if (method.IsSpecialName)
                    continue;
                Regulus.Utility.Log.Instance.WriteInfo($"method name = {method.Name} ");

                MethodStringInvoker invoker = new MethodStringInvoker(instance, method, _TypeConverterSet);
                AgentCommand ac = new AgentCommand(_VersionProvider, type, invoker);
                _Invokers.Add(ac);
                _Command.Register(ac.Name, (args) => invoker.Invoke(args), method.ReturnParameter.ParameterType, method.GetParameters().Select((p) => p.ParameterType).ToArray());
            }

        }

        

        public void Unregist(object instance)
        {
            AgentCommand[] removes = _Invokers.Where(i => i.Target == instance).ToArray();
            foreach (AgentCommand invoker in removes)
            {
                _Command.Unregister(invoker.Name);
                _Invokers.Remove(invoker);
            }
        }
    }
}
