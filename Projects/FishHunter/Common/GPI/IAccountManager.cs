using Regulus.Remoting;


using VGame.Project.FishHunter.Common.Data;

namespace VGame.Project.FishHunter.Common.GPI
{
	public interface IAccountManager 
	{
		Value<ACCOUNT_REQUEST_RESULT> Create(Account account);
		Value<Account[]> QueryAllAccount();

		Value<ACCOUNT_REQUEST_RESULT> Delete(string account);

		Value<ACCOUNT_REQUEST_RESULT> Update(Account account);
	}
}
