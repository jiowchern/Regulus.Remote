// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResponseQueue.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the IResponseQueue type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System.Collections.Generic;

#endregion

namespace Regulus.Remoting
{
	public interface IResponseQueue
	{
		void Push(byte cmd, Dictionary<byte, byte[]> args);
	}
}