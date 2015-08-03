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

using VGame.Project.FishHunter.Common.Datas;

#endregion

namespace VGame.Project.FishHunter.Common.GPIs
{
	public interface IStorageCompetences
	{
		Value<Account.COMPETENCE[]> Query();

		Value<Guid> QueryForId();
	}
}