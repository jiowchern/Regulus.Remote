// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DummyFeature.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the DummyFrature type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Collections.Generic;

using Regulus.Remoting;

using VGame.Project.FishHunter.Data;
using VGame.Project.FishHunter.Formula;

namespace VGame.Project.FishHunter
{
	public class DummyFrature : IAccountFinder, IFishStageQueryer, IStorage
	{
		private readonly List<Account> _Accounts;

		private readonly List<Record> _Records;

		public DummyFrature()
		{
			_Records = new List<Record>();

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

		Value<Account> IAccountFinder.FindAccountById(Guid accountId)
		{
			return _Accounts.Find(a => a.Id == accountId);
		}

		Value<IFishStage> IFishStageQueryer.Query(long player_id, byte fish_stage)
		{
			switch (fish_stage)
			{
				case 200:
					return null;
				case 100:
					return new FishStage(player_id, fish_stage);
				default:
					return new CsFishStage(player_id, fish_stage);
			}

			// return new FishStage(player_id, fish_stage);
		}

		Value<Account[]> IAccountManager.QueryAllAccount()
		{
			return _Accounts.ToArray();
		}

		Value<ACCOUNT_REQUEST_RESULT> IAccountManager.Create(Account account)
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
			if (_Accounts.RemoveAll(a => a.Id == account.Id) > 0)
			{
				_Accounts.Add(account);
				return ACCOUNT_REQUEST_RESULT.OK;
			}

			return ACCOUNT_REQUEST_RESULT.NOTFOUND;
		}

		Value<Record> IRecordQueriers.Load(Guid id)
		{
			var account = _Accounts.Find(a => a.Id == id);
			if (account.IsPlayer())
			{
				var record = _Records.Find(r => r.Owner == account.Id);
				if (record == null)
				{
					record = new Record
					{
						Money = 1000, 
						Owner = id
					};
				}

				return record;
			}

			return null;
		}

		void IRecordQueriers.Save(Record record)
		{
			var account = _Accounts.Find(a => a.Id == record.Owner);
			if (account.IsPlayer())
			{
				var old = _Records.Find(r => r.Owner == account.Id);
				_Records.Remove(old);
				_Records.Add(record);
			}
		}

		Value<TradeNotes> ITradeNotes.Find(Guid id)
		{
			return new TradeNotes(Guid.NewGuid());
		}

		Value<TradeNotes> ITradeNotes.Load(Guid id)
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
	}
}