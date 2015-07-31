// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestQueue.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the IRequestQueue type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;

#endregion

namespace Regulus.Remoting
{
	public interface IRequestQueue
	{
		event Action BreakEvent;

		event Action<Guid, string, Guid, byte[][]> InvokeMethodEvent;

		void Update();
	}
}