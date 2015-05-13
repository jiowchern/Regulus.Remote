using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter
{
    public class DummyFrature : IAccountFinder , IFishStageQueryer , IStorage
    {
        List<Data.Account> _Accounts;
        public DummyFrature()
        {
            _Accounts = new List<Data.Account>();
            _Accounts.Add(new Data.Account { Id = Guid.NewGuid(), Password = "pw", Name = "name", Competnce = Data.Account.COMPETENCE.ALL });
            _Accounts.Add(new Data.Account { Id = Guid.NewGuid(), Password = "vgame", Name = "vgameadmini", Competnce = Data.Account.COMPETENCE.ALL });
        }

        Regulus.Remoting.Value<Data.Account> IAccountFinder.FindAccountByName(string id)
        {
            return _Accounts.Find(a => a.Name == id);            
        }


        Regulus.Remoting.Value<Data.Account> IAccountFinder.FindAccountById(Guid accountId)
        {
            return _Accounts.Find(a => a.Id == accountId);                        
        }

        Regulus.Remoting.Value<IFishStage> IFishStageQueryer.Query(long player_id, byte fish_stage)
        {
            return new VGame.Project.FishHunter.Formula.FishStage(player_id, fish_stage);
        }

        Regulus.Remoting.Value<Data.Account[]> IAccountManager.QueryAllAccount()
        {

            return _Accounts.ToArray();
        }

        Regulus.Remoting.Value<ACCOUNT_REQUEST_RESULT> IAccountManager.Create(Data.Account account)
        {
            _Accounts.Add(account);
            return ACCOUNT_REQUEST_RESULT.OK;
        }

        Regulus.Remoting.Value<ACCOUNT_REQUEST_RESULT> IAccountManager.Delete(string account)
        {
            _Accounts.RemoveAll(a => a.Name == account);
            return ACCOUNT_REQUEST_RESULT.OK;
        }

        Regulus.Remoting.Value<ACCOUNT_REQUEST_RESULT> IAccountManager.Update(Data.Account account)
        {
            if(_Accounts.RemoveAll(a => a.Id == account.Id) > 0)
            {
                _Accounts.Add(account);
                return ACCOUNT_REQUEST_RESULT.OK;
            }
            return ACCOUNT_REQUEST_RESULT.NOTFOUND;
        }
    }
}
