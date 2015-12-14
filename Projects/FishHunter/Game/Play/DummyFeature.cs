using System;
using System.Collections.Generic;


using Regulus.Remoting;


using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Common.GPI;
using VGame.Project.FishHunter.Formula;
using VGame.Project.FishHunter.Formula.ZsFormula;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;
using VGame.Project.FishHunter.Stage;

using Account = VGame.Project.FishHunter.Common.Data.Account;

namespace VGame.Project.FishHunter.Play
{
	public class DummyFrature : IFishStageQueryer, IStorage
	{
		private readonly List<Account> _Accounts;

		private readonly List<GamePlayerRecord> _Records;

		public DummyFrature()
		{
			_Records = new List<GamePlayerRecord>();

			_Accounts = new List<Account>
			{
				new Account
				{
					Guid = Guid.NewGuid(),
					Name = "Guest",
					Password = "vgame",
					Competnces = Account.AllCompetnce()
				},

				new Account
				{
					Guid = Guid.NewGuid(),
					Name = "name",
					Password = "pw", 
					Competnces = Account.AllCompetnce()
				}, 
				new Account
				{
					Guid = Guid.NewGuid(),
					Name = "vgameadmini",
					Password = "vgame", 
					Competnces = Account.AllCompetnce()
				}, 
				new Account
				{
					Guid = Guid.NewGuid(),
					Name = "user1",
					Password = "user", 
					Competnces = Account.AllCompetnce()
				}
			};
		}

		Value<Account> IAccountFinder.FindAccountByName(string id)
		{
			return _Accounts.Find(a => a.Name == id);
		}

		Value<Account> IAccountFinder.FindAccountById(Guid account_id)
		{
            var result = _Accounts.Find(a => a.Guid == account_id);
		    return result;
		}

		Value<IFishStage> IFishStageQueryer.Query(Guid player_id, int fish_stage)
		{
			switch(fish_stage)
			{
				case 111:
					return new QuarterStage(player_id, fish_stage);
				case 100:
					return new ZsFishStage(player_id, new FishFarmBuilder().Get(fish_stage), new FormulaPlayerRecord(), this, this);
				default:
					return new FishStage(player_id, fish_stage);
			}
		}

	    Value<ACCOUNT_REQUEST_RESULT> IAccountManager.Create(Account account)
	    {
            _Accounts.Add(account);
            return ACCOUNT_REQUEST_RESULT.OK;
	    }

	    Value<Account[]> IAccountManager.QueryAllAccount()
		{
			return _Accounts.ToArray();
		}
		

		Value<ACCOUNT_REQUEST_RESULT> IAccountManager.Delete(string account)
		{
			_Accounts.RemoveAll(a => a.Name == account);
			return ACCOUNT_REQUEST_RESULT.OK;
		}

		Value<ACCOUNT_REQUEST_RESULT> IAccountManager.Update(Account account)
		{
			if(_Accounts.RemoveAll(a => a.Id == account.Id) > 0)
			{
				_Accounts.Add(account);
				return ACCOUNT_REQUEST_RESULT.OK;
			}

			return ACCOUNT_REQUEST_RESULT.NOTFOUND;
		}

		Value<GamePlayerRecord> IGameRecorder.Load(Guid account_id)
		{
			var account = _Accounts.Find(a => a.Guid == account_id);
			if(account.IsPlayer())
			{
				var record = _Records.Find(r => r.Owner == account.Guid);
				if(record == null)
				{
					record = new GamePlayerRecord
					{
						Id = Guid.NewGuid(),
						Money = 1000, 
						Owner = account_id
					};
				}

				return record;
			}

			return null;
		}

		void IGameRecorder.Save(GamePlayerRecord game_player_record)
		{
			var account = _Accounts.Find(a => a.Guid == game_player_record.Owner);
			if(account.IsPlayer())
			{
				var old = _Records.Find(r => r.Owner == account.Guid);
				_Records.Remove(old);
				_Records.Add(game_player_record);
			}
		}

		Value<TradeNotes> ITradeNotes.Find(Guid id)
		{
			return new TradeNotes(Guid.NewGuid());
		}

		Value<TradeNotes> ITradeNotes.Load(Guid account_id)
		{
			return new TradeNotes(Guid.NewGuid());
		}

		Value<bool> ITradeNotes.Write(TradeNotes.TradeData data)
		{
			return true;
		}

		Value<int> ITradeNotes.GetTotalMoney(Guid id)
		{
			return 0;
		}

		Value<FishFarmData> IFormulaFarmRecorder.Load(int farm_id)
		{
			return new FishFarmBuilder().Get(farm_id);
		}

		Value<bool> IFormulaFarmRecorder.Save(FishFarmData data)
		{
			return true;
		}

		Value<FormulaPlayerRecord> IFormulaPlayerRecorder.Query(Guid player_id)
		{
			return new FormulaPlayerRecord();
		}

		Value<bool> IFormulaPlayerRecorder.Save(FormulaPlayerRecord record)
		{
			return true;
		}
	}
}
