using System;


using Regulus.Remote;

namespace Regulus.Utility
{
	public static class UtilityCommandExtension
	{
		public static void RemotingRegister<TR>(
			this ICommand commander, 
			string command, 
			Func<Value<TR>> executer, 
			Action<TR> value)
		{
			commander.Register(command, executer, Helper.UnBox(value));
		}

		public static void RemotingRegister<T1, TR>(
			this ICommand commander, 
			string command, 
			Func<T1, Value<TR>> executer, 
			Action<TR> value)
		{
			commander.Register(command, executer, Helper.UnBox(value));
		}

		public static void RemotingRegister<T1, T2, TR>(
			this ICommand commander, 
			string command, 
			Func<T1, T2, Value<TR>> executer, 
			Action<TR> value)
		{
			commander.Register(command, executer, Helper.UnBox(value));
		}

		public static void RemotingRegister<T1, T2, T3, TR>(
			this ICommand commander, 
			string command, 
			Func<T1, T2, T3, Value<TR>> executer, 
			Action<TR> value)
		{
			commander.Register(command, executer, Helper.UnBox(value));
		}

		public static void RemotingRegister<T1, T2, T3, T4, TR>(
			this ICommand commander, 
			string command, 
			Func<T1, T2, T3, T4, Value<TR>> executer, 
			Action<TR> value)
		{
			commander.Register(command, executer, Helper.UnBox(value));
		}
	}
}
