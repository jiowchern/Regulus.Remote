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

using VGame.Project.FishHunter.Common;
using VGame.Project.FishHunter.Common.Datas;
using VGame.Project.FishHunter.Common.GPIs;
using VGame.Project.FishHunter.Formula;

namespace VGame.Project.FishHunter.Play
{
	public class DummyFrature : IAccountFinder, IFishStageQueryer, IStorage
	{
		private readonly List<Account> _Accounts;

		private readonly List<Record> _Records;

		public DummyFrature()
		{
			this._Records = new List<Record>();

			this._Accounts = new List<Account>
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
			return this._Accounts.Find(a => a.Name == id);
		}

		Value<Account> IAccountFinder.FindAccountById(Guid account_id)
		{
			return this._Accounts.Find(a => a.Id == account_id);
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

        Value<Account[]> Common.GPIs.IAccountManager.QueryAllAccount()
		{
			return this._Accounts.ToArray();
		}

        Value<ACCOUNT_REQUEST_RESULT> Common.GPIs.IAccountCreator.Create(Account account)
		{
			this._Accounts.Add(account);
			return ACCOUNT_REQUEST_RESULT.OK;
		}

        Value<ACCOUNT_REQUEST_RESULT> Common.GPIs.IAccountManager.Delete(string account)
		{
			this._Accounts.RemoveAll(a => a.Name == account);
			return ACCOUNT_REQUEST_RESULT.OK;
		}

        Value<ACCOUNT_REQUEST_RESULT> Common.GPIs.IAccountManager.Update(Account account)
		{
			if (this._Accounts.RemoveAll(a => a.Id == account.Id) > 0)
			{
				this._Accounts.Add(account);
				return ACCOUNT_REQUEST_RESULT.OK;
			}

			return ACCOUNT_REQUEST_RESULT.NOTFOUND;
		}

		Value<Record> IRecordQueriers.Load(Guid id)
		{
			var account = this._Accounts.Find(a => a.Id == id);
			if (account.IsPlayer())
			{
				var record = this._Records.Find(r => r.Owner == account.Id);
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
			var account = this._Accounts.Find(a => a.Id == record.Owner);
			if (account.IsPlayer())
			{
				var old = this._Records.Find(r => r.Owner == account.Id);
				this._Records.Remove(old);
				this._Records.Add(record);
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