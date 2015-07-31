// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Ghost.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Ghost type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;

using Regulus.Remoting;

#endregion

namespace RemotingTest
{
	internal class Ghost : IGhost
	{
		private readonly Guid _Id;

		public Ghost()
		{
			_Id = Guid.NewGuid();
		}

		void IGhost.OnEvent(string name_event, object[] args)
		{
			throw new NotImplementedException();
		}

		Guid IGhost.GetID()
		{
			return _Id;
		}

		void IGhost.OnProperty(string name, byte[] value)
		{
			throw new NotImplementedException();
		}

		bool IGhost.IsReturnType()
		{
			throw new NotImplementedException();
		}
	}
}