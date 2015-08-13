using System;


using Regulus.Remoting;


using VGame.Project.FishHunter.Common.Data;

namespace VGame.Project.FishHunter.Common.GPI
{
	public interface IStorageCompetences
	{
		Value<Account.COMPETENCE[]> Query();

		Value<Guid> QueryForId();
	}
}
