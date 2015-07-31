// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GhostRequest.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the IGhostRequest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System.Collections.Generic;

#endregion

namespace Regulus.Remoting
{
	public interface IGhostRequest
	{
		void Request(byte code, Dictionary<byte, byte[]> args);
	}
}