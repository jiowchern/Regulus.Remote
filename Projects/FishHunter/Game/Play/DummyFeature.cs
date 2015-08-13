using System;
using System.Collections.Generic;


using Regulus.Remoting;


using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Common.GPI;
using VGame.Project.FishHunter.Formula;

namespace VGame.Project.FishHunter.Play
{
	public class DummyFrature : IAccountFinder, IFishStageQueryer, IStorage
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
					Id = Guid.NewGuid(), 
					Password = "pw", 
					Name = "name", 
					Competnces = Account.AllCompetnce()
				}, 
				new Account
				{
					Id = Guid.NewGuid(), 
					Password = "vgame", 
					Name = "vgameadmini", 
					Competnces = Account.AllCompetnce()
				}, 
				new Account
				{
					Id = Guid.NewGuid(), 
					Password = "user", 
					Name = "user1", 
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
			return _Accounts.Find(a => a.Id == account_id);
		}

		Value<IFishStage> IFishStageQueryer.Query(Guid player_id, int fish_stage)
		{
			switch(fish_stage)
			{
				case 200:
					return null;
				case 100:
					return new FishStage(player_id, fish_stage);
				default:
					return new CsFishStage(player_id, fish_stage);
			}
		}

		Value<Account[]> IAccountManager.QueryAllAccount()
		{
			return _Accounts.ToArray();
		}

		Value<ACCOUNT_REQUEST_RESULT> IAccountCreator.Create(Account account)
		{
			_Accounts.Add(account);
			return ACCOUNT_REQUEST_RESULT.OK;
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
			var account = _Accounts.Find(a => a.Id == account_id);
			if(account.IsPlayer())
			{
				var record = _Records.Find(r => r.Owner == account.Id);
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
			var account = _Accounts.Find(a => a.Id == game_player_record.Owner);
			if(account.IsPlayer())
			{
				var old = _Records.Find(r => r.Owner == account.Id);
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

		Value<StageData> IFormulaStageDataRecorder.Load(int stage_id)
		{
			throw new NotImplementedException();
		}

		Value<bool> IFormulaStageDataRecorder.Save(StageData data)
		{
			throw new NotImplementedException();
		}

		Value<FormulaPlayerRecord> IFormulaPlayerRecorder.Query(Guid player_id)
		{
			throw new NotImplementedException();
		}

		Value<bool> IFormulaPlayerRecorder.Save(FormulaPlayerRecord record)
		{
			throw new NotImplementedException();
		}
	}
}
