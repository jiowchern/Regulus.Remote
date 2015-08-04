// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAccountStatus.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the IAccountStatus type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;

#endregion

namespace VGame.Project.FishHunter.Common.GPI
{
	public interface IAccountStatus
	{
		event Action KickEvent;
	}
}