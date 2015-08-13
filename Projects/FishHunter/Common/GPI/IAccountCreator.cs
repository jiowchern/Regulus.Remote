using Regulus.Remoting;


using VGame.Project.FishHunter.Common.Data;

namespace VGame.Project.FishHunter.Common.GPI
{
	public interface IAccountCreator
	{
		Value<ACCOUNT_REQUEST_RESULT> Create(Account account);
	}
}
