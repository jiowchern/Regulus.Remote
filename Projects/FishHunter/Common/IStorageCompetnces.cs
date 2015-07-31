// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStorageCompetnces.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the IStorageCompetences type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;

using Regulus.Remoting;

#endregion

namespace VGame.Project.FishHunter.Common
{
	public interface IStorageCompetences
	{
		Value<Account.COMPETENCE[]> Query();

		Value<Guid> QueryForId();
	}
}