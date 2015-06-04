using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regulus.Extension;
namespace VGame.Project.FishHunter
{
    public class DummyFrature : IAccountFinder , IFishStageQueryer , IStorage
    {
        List<Data.Account> _Accounts;
        List<Data.Record> _Records;
        public DummyFrature()
        {
            _Records = new List<Data.Record>();
            _Accounts = new List<Data.Account>();
            _Accounts.Add(new Data.Account { Id = Guid.NewGuid(), Password = "pw", Name = "name", Competnces = Data.Account.AllCompetnce() });
            _Accounts.Add(new Data.Account { Id = Guid.NewGuid(), Password = "vgame", Name = "vgameadmini", Competnces = Data.Account.AllCompetnce() });
            _Accounts.Add(new Data.Account { Id = Guid.NewGuid(), Password = "user", Name = "user1", Competnces = Data.Account.AllCompetnce() });
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

        Regulus.Remoting.Value<Data.Record> IRecordQueriers.Load(Guid id)
        {
            var account = _Accounts.Find(a => a.Id == id);
            if(account.IsPlayer())
            {
                var record = _Records.Find(r => r.Owner == account.Id);
                if(record == null)
                {
                    record = new Data.Record() { Money = 1000, Owner = id };
                }
                return record;
            }
            return null;
        }

        void IRecordQueriers.Save(Data.Record record)
        {
            var account = _Accounts.Find(a => a.Id == record.Owner);
            if(account.IsPlayer())
            {
                var old = _Records.Find(r => r.Owner == account.Id);
                _Records.Remove(old);
                _Records.Add(record);

            }            
        }

        Regulus.Remoting.Value<TradeNotes> ITradeAccount.Find(Guid id)
        {
            throw new NotImplementedException();
        }

        Regulus.Remoting.Value<TradeNotes> ITradeAccount.Load(Guid id)
        {
            throw new NotImplementedException();
        }

        Regulus.Remoting.Value<Data.TradeData> ITradeAccount.Saving(Data.TradeData data)
        {
            throw new NotImplementedException();
        }
    }
}
