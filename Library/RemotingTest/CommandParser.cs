// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandParser.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the CommandParser type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Framework;
using Regulus.Remoting;

#endregion

namespace RemotingTest
{
	internal class CommandParser : ICommandParsable<IUser>
	{
		void ICommandParsable<IUser>.Setup(IGPIBinderFactory build)
		{
		}

		void ICommandParsable<IUser>.Clear()
		{
		}
	}
}