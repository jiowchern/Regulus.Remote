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
                _Command.Register(ac.Name, (args) => _PrintReturn(invoker.Invoke(args)), method.ReturnParameter.ParameterType, method.GetParameters().Select((p) => p.ParameterType).ToArray());
            }

        }

        private void _PrintReturn(object val)
        {
            if (val == null)
                return;
            Type type = val.GetType();

            if (type.GetGenericTypeDefinition() != typeof(Regulus.Remote.Value<>))
            {
                return;
            }
            IValue value = val as IValue;

            value.QueryValue(_PrintValue);

        }

        private void _PrintValue(object obj)
        {
            Regulus.Utility.Log.Instance.WriteInfo($"return :{obj}");
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
