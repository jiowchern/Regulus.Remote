// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICommand.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the ICommand type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;

#endregion

namespace Regulus.Utility
{
	public interface ICommand
	{
		void Register(string command, Action executer);

		void Register<T1, T2, T3, T4, TR>(string command, Func<T1, T2, T3, T4, TR> executer, Action<TR> value);

		void Register<T1, T2, T3, T4>(string command, Action<T1, T2, T3, T4> executer);

		void Register<T1, T2, T3, TR>(string command, Func<T1, T2, T3, TR> executer, Action<TR> value);

		void Register<T1, T2, T3>(string command, Action<T1, T2, T3> executer);

		void Register<T1, T2, TR>(string command, Func<T1, T2, TR> executer, Action<TR> value);

		void Register<T1, T2>(string command, Action<T1, T2> executer);

		void Register<T1, TR>(string command, Func<T1, TR> executer, Action<TR> value);

		void Register<T1>(string command, Action<T1> executer);

		void Register<TR>(string command, Func<TR> executer, Action<TR> value);

		void Unregister(string command);
	}
}