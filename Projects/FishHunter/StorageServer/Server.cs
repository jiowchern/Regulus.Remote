// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Server.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Server type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Regulus.CustomType;
using Regulus.Database.NoSQL;
using Regulus.Framework;
using Regulus.Remoting;
using Regulus.Utility;

using VGame.Project.FishHunter.Common;

#endregion

namespace VGame.Project.FishHunter.Storage
{
	public class Server : ICore, IStorage
	{
		private readonly Center _Center;

		private readonly Database _Database;

		private readonly string _DefaultAdministratorName;

		private readonly string _Ip;

		private readonly LogFileRecorder _LogRecorder;

		private readonly string _Name;

		private readonly Updater _Updater;

		private ICore _Core
		{
			get { return _Center; }
		}

		public Server()
		{
			_LogRecorder = new LogFileRecorder("Storage");
			_DefaultAdministratorName = "vgameadmini";
			_Ip = "mongodb://127.0.0.1:27017";
			_Name = "VGame";
			_Updater = new Updater();
			_Database = new Database(_Ip);
			_Center = new Center(this);
		}

		void ICore.AssignBinder(ISoulBinder binder)
		{
			_Core.AssignBinder(binder);
		}

		bool IUpdatable.Update()
		{
			_Updater.Working();
			return true;
		}

		void IBootable.Launch()
		{
			Singleton<Log>.Instance.RecordEvent += _LogRecorder.Record;
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

			_Updater.Add(_Center);
			_Database.Launch(_Name);

			_HandleAdministrator();

			_HandleGuest();

			// _HandleTradeRecord();
		}

		void IBootable.Shutdown()
		{
			_Database.Shutdown();
			_Updater.Shutdown();

			AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
			Singleton<Log>.Instance.RecordEvent -= _LogRecorder.Record;
		}

		Value<Account> IAccountFinder.FindAccountByName(string name)
		{
			var account = _Find(name);

			if (account != null)
			{
				return account;
			}

			return new Value<Account>(null);
		}

		Value<ACCOUNT_REQUEST_RESULT> IAccountManager.Create(Account account)
		{
			var result = _Find(account.Name);
			if (result != null)
			{
				return ACCOUNT_REQUEST_RESULT.REPEAT;
			}

			_Database.Add(account);
			return ACCOUNT_REQUEST_RESULT.OK;
		}

		Value<ACCOUNT_REQUEST_RESULT> IAccountManager.Delete(string account)
		{
			var result = _Find(account);
			if (result != null && _Database.Remove<Account>(a => a.Id == result.Id))
			{
				return ACCOUNT_REQUEST_RESULT.OK;
			}

			return ACCOUNT_REQUEST_RESULT.NOTFOUND;
		}

		Value<Account[]> IAccountManager.QueryAllAccount()
		{
			return _QueryAllAccount();
		}

		Value<ACCOUNT_REQUEST_RESULT> IAccountManager.Update(Account account)
		{
			if (_Database.Update(account, a => a.Id == account.Id))
			{
				return ACCOUNT_REQUEST_RESULT.OK;
			}

			return ACCOUNT_REQUEST_RESULT.NOTFOUND;
		}

		Value<Account> IAccountFinder.FindAccountById(Guid accountId)
		{
			return _Find(accountId);
		}

		Value<Record> IRecordQueriers.Load(Guid id)
		{
			var val = new Value<Record>();
			var account = _Find(id);
			if (account.IsPlayer())
			{
				var recordTask = _Database.Find<Record>(r => r.Owner == id);
				recordTask.ContinueWith(task =>
				{
					if (task.Result.Count > 0)
					{
						val.SetValue(task.Result.FirstOrDefault());
					}
					else
					{
						var newRecord = new Record
						{
							Owner = id, 
							Money = 100
						};
						_Database.Add(newRecord).Wait();
						val.SetValue(newRecord);
					}
				});
			}
			else
			{
				val.SetValue(null);
			}

			return val;
		}

		void IRecordQueriers.Save(Record record)
		{
			_Database.Update(record, r => r.Id == record.Id);
		}

		Value<TradeNotes> ITradeNotes.Find(Guid id)
		{
			var val = new Value<TradeNotes>();
			var t = _LoadTradeNotesTask(id);
			t.ContinueWith(task =>
			{
				var notes = task.Result.SingleOrDefault();
				val.SetValue(notes);
			});

			return val;
		}

		Value<TradeNotes> ITradeNotes.Load(Guid id)
		{
			var val = new Value<TradeNotes>();
			var t = _LoadTradeNotesTask(id);
			t.ContinueWith(task =>
			{
				var notes = task.Result.SingleOrDefault();
				if (notes == null)
				{
					var newPlayerNotes = new TradeNotes(id);
					_Database.Add(newPlayerNotes).Wait();
					val.SetValue(newPlayerNotes);
				}
				else
				{
					val.SetValue(notes);
				}
			});
			return val;
		}

		Value<bool> ITradeNotes.Write(TradeNotes.TradeData data)
		{
			var val = new Value<bool>();
			var t = _LoadTradeNotesTask(data.BuyerId);
			t.ContinueWith(task =>
			{
				var notes = task.Result.SingleOrDefault();
				notes.TradeDatas.Add(data);
				val.SetValue(_Database.Update(notes, a => a.Owner == notes.Owner));
			});

			return val;
		}

		Value<int> ITradeNotes.GetTotalMoney(Guid id)
		{
			var val = new Value<int>();

			var t = _LoadTradeNotesTask(id);
			t.ContinueWith(task =>
			{
				var notes = task.Result.SingleOrDefault();

				if (notes == null)
				{
					val.SetValue(0);
				}
				else
				{
					val.SetValue(notes.GetTotalMoney());
					notes.SetTradeIsUsed();
					_Database.Update(notes, a => a.Owner == notes.Owner);
				}
			});
			return val;
		}

		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			var ex = (Exception)e.ExceptionObject;
			_LogRecorder.Record(ex.ToString());
			_LogRecorder.Save();
		}

		private async void _HandleAdministrator()
		{
			var accounts = await _Database.Find<Account>(a => a.Name == _DefaultAdministratorName);

			if (accounts.Count == 0)
			{
				var account = new Account
				{
					Id = Guid.NewGuid(), 
					Name = _DefaultAdministratorName, 
					Password = "vgame", 
					Competnces = Account.AllCompetnce()
				};

				await _Database.Add(account);

				await _Database.Add(new TradeNotes
				{
					Owner = account.Id
				});
			}
		}

		private async void _HandleGuest()
		{
			var accounts = await _Database.Find<Account>(a => a.Name == "Guest");

			if (accounts.Count == 0)
			{
				var account = new Account
				{
					Id = Guid.NewGuid(), 
					Name = "Guest", 
					Password = "vgame", 
					Competnces = new Flag<Account.COMPETENCE>(Account.COMPETENCE.FORMULA_QUERYER)
				};
				await _Database.Add(account);
			}
		}

		private void _HandleTradeRecord()
		{
			// _QueryAllAccount().OnValue += ((accounts) =>
			// {
			// foreach (var acc in accounts)
			// {
			// var trades = _TradeCorder.FindHistory(acc.Key);

			// }
			// );
		}

		private Account _Find(string name)
		{
			var task = _Database.Find<Account>(a => a.Name == name);
			task.Wait();
			return task.Result.FirstOrDefault();
		}

		private Account _Find(Guid id)
		{
			var task = _Database.Find<Account>(a => a.Id == id);
			task.Wait();
			return task.Result.FirstOrDefault();
		}

		private Value<Account[]> _QueryAllAccount()
		{
			var val = new Value<Account[]>();
			var t = _Database.Find<Account>(a => true);
			t.ContinueWith(list => { val.SetValue(list.Result.ToArray()); });
			return val;
		}

		private Task<List<TradeNotes>> _LoadTradeNotesTask(Guid id)
		{
			var tradeTask = _Database.Find<TradeNotes>(t => t.Owner == id);

			var returnTask = tradeTask.ContinueWith(task =>
			{
				Singleton<Log>.Instance.WriteDebug("TradeNotes Find Done.");

				if (task.Exception != null)
				{
					Singleton<Log>.Instance.WriteDebug(string.Format("TradeNotes Exception {0}.", task.Exception.ToString()));
				}

				return task.Result;
			});
			return returnTask;
		}
	}
}