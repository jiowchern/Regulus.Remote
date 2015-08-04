// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GhostIghost.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the IGhost type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;

#endregion

namespace Regulus.Remoting
{
	public interface IGhost
	{
		void OnEvent(string name_event, object[] args);

		Guid GetID();

		void OnProperty(string name, byte[] value);

		bool IsReturnType();
	}
}