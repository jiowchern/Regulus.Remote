using VGame.Project.FishHunter.Common.Data;

namespace VGame.Project.FishHunter.Common.GPI
{
    public interface IAccountCreator
    {
        Regulus.Remoting.Value<ACCOUNT_REQUEST_RESULT> Create(Account account);
    }
}
