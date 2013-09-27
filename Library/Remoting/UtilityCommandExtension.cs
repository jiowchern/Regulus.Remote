using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Extension
{
    public static class UtilityCommandExtension
    {
        public static void RemotingRegister<TR>(this Regulus.Utility.Command commander, string command, Func<Regulus.Remoting.Value<TR>> executer, Action<TR> value)
        {        
            commander.Register<Regulus.Remoting.Value<TR>>(command, executer , Regulus.Remoting.Helper.UnBox<TR>(value));
        }

        public static void RemotingRegister<T1, TR>(this Regulus.Utility.Command commander, string command, Func<T1, Regulus.Remoting.Value<TR>> executer, Action<TR> value)
        {
            commander.Register<T1,Regulus.Remoting.Value<TR>>(command, executer, Regulus.Remoting.Helper.UnBox<TR>(value));
        }
        public static void RemotingRegister<T1, T2, TR>(this Regulus.Utility.Command commander, string command, Func<T1, T2, Regulus.Remoting.Value<TR>> executer, Action<TR> value)
        {
            commander.Register<T1,T2, Regulus.Remoting.Value<TR>>(command, executer, Regulus.Remoting.Helper.UnBox<TR>(value));
        }

        public static void RemotingRegister<T1, T2, T3, TR>(this Regulus.Utility.Command commander, string command, Func<T1, T2, T3, Regulus.Remoting.Value<TR>> executer, Action<TR> value)
        {
            commander.Register<T1, T2, T3, Regulus.Remoting.Value<TR>>(command, executer, Regulus.Remoting.Helper.UnBox<TR>(value));
        }

        public static void RemotingRegister<T1, T2, T3, T4, TR>(this Regulus.Utility.Command commander, string command, Func<T1, T2, T3, T4, Regulus.Remoting.Value<TR>> executer, Action<TR> value)
        {
            commander.Register<T1, T2, T3, T4, Regulus.Remoting.Value<TR>>(command, executer, Regulus.Remoting.Helper.UnBox<TR>(value));
        }
        

    }
}
