using System;
using System.Linq;
using System.Reflection;

namespace Regulus.Remote.Client
{
    public class MethodStringInvoker
    {
        public readonly object Target;
        public readonly MethodInfo Method;

        public MethodStringInvoker(object instance, MethodInfo method)
        {
            this.Target = instance;
            this.Method = method;
        }

        public object Invoke(params string [] in_args)
        {
            
            var argInfos = Method.GetParameters();

            if (in_args.Length != argInfos.Length)
                throw new Exception($"Method parameter is {argInfos.Length}, input parameter is {in_args.Length}");

            var argInstances = new System.Collections.Generic.List<object>();
            for (var i = 0; i < argInfos.Length; ++i)
            {
                var argInfo  = argInfos[i];
                var inArg = in_args[i];
                object val;
                if (!Regulus.Utility.Command.Conversion(inArg, out val, argInfo.ParameterType))
                    throw new Exception($"Type mismatch , arg is {inArg}");

                argInstances.Add(val);
            }
            return Method.Invoke(Target, argInstances.ToArray());
        }
    }
}