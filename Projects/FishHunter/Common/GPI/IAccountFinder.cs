using System;


using Regulus.Remoting;


using VGame.Project.FishHunter.Common.Data;

namespace VGame.Project.FishHunter.Common.GPI
{
	public interface IAccountFinder
	{
		Value<Account> FindAccountByName(string id);

		Value<Account> FindAccountById(Guid accountId);
	}
}
