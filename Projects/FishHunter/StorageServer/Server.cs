using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;

using NLog;
using NLog.Fluent;

using Regulus.CustomType;

//using Regulus.Database.DB_NoSQL;
using Regulus.Database.DB_Redis;
using Regulus.Framework;
using Regulus.Remoting;
using Regulus.Utility;

using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Common.GPI;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;

using Log = Regulus.Utility.Log;

namespace VGame.Project.FishHunter.Storage
{
	public class Server : ICore, IStorage
	{
		private static readonly Logger _Logger = LogManager.GetLogger("DBServer");

		private readonly Center _Center;

		private readonly Database _Database;

		private readonly string _DefaultAdministratorName;

		private readonly LogFileRecorder _LogRecorder;

		private readonly string _Name;

		private readonly Updater _Updater;

		private ICore _Core => _Center;

		public Server()
		{
			//_LogRecorder = new LogFileRecorder("Storage");
			_DefaultAdministratorName = "vgameadmini";

			//const string mongodbUrls = "mongodb://127.0.0.1:27017";
			_Name = "VGame";
			_Updater = new Updater();
			_Database = new Database();
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
			_UnhandleCrash();
			//Singleton<Log>.Instance.RecordEvent += _LogRecorder.Run;

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

			//Singleton<Log>.Instance.RecordEvent -= _LogRecorder.Run;
			//_LogRecorder.Save();
		}

		Value<Account> IAccountFinder.FindAccountByName(string name)
		{
			var account = _Find(name);

			if(account != null)
			{
				return account;
			}

			return new Value<Account>(null);
		}

		Value<Account> IAccountFinder.FindAccountById(Guid account_id)
		{
			return _Find(account_id);
		}

		Value<ACCOUNT_REQUEST_RESULT> IAccountManager.Delete(string account)
		{
			//_Find(account_id)
			var result = _Find(account);
			if(result != null && _Database.Remove(result, obj => obj.Id))
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
			if(_Database.Update(account, a => a.Id))
			{
				return ACCOUNT_REQUEST_RESULT.OK;
			}

			return ACCOUNT_REQUEST_RESULT.NOTFOUND;
		}

		

		Value<GamePlayerRecord> IGameRecorder.Load(Guid account_id)
		{
			var val = new Value<GamePlayerRecord>();
			var account = _Find(account_id);
			if(account.IsPlayer())
			{
				var record = _Database.FindAll<GamePlayerRecord>().SingleOrDefault(r => r.Owner == account_id);

				if(record != null)
				{
					val.SetValue(record);
				}
				else
				{
					var newRecord = new GamePlayerRecord
					{
						Id = Guid.NewGuid(),
						Owner = account_id,
						Money = 100
					};
					_Database.Add(newRecord, obj => obj.Id);
					val.SetValue(newRecord);
				}
			}
			else
			{
				val.SetValue(null);
			}

			return val;
		}

		void IGameRecorder.Save(GamePlayerRecord record)
		{
			var data = _Database.FindAll<GamePlayerRecord>().SingleOrDefault(r => r.Id == record.Id);

			_Database.Update(data, r => r.Id == record.Id);
		}

		Value<TradeNotes> ITradeNotes.Find(Guid id)
		{
			var val = new Value<TradeNotes>();
			var list = _LoadTradeNotes(id);
			var notes = list.SingleOrDefault();
			val.SetValue(notes);
			return val;
		}

		Value<TradeNotes> ITradeNotes.Load(Guid account_id)
		{
			var val = new Value<TradeNotes>();
			var list = _LoadTradeNotes(account_id);

			var notes = list.SingleOrDefault();
			if(notes == null)
			{
				var newPlayerNotes = new TradeNotes(account_id);
				_Database.Add(newPlayerNotes, obj => obj.Id);
				val.SetValue(newPlayerNotes);
			}
			else
			{
				val.SetValue(notes);
			}

			return val;
		}

		Value<bool> ITradeNotes.Write(TradeNotes.TradeData trade_data)
		{
			var val = new Value<bool>();
			var tradeNotes = _LoadTradeNotes(trade_data.BuyerId);

			if(tradeNotes == null)
			{
				throw new Exception("請檢查資料庫");
			}

			var notes = tradeNotes.SingleOrDefault();
			notes.TradeDatas.Add(trade_data);

			var result = _Database.Update(notes, a => a.Id);
			val.SetValue(result);

			return val;
		}

		Value<int> ITradeNotes.GetTotalMoney(Guid id)
		{
			var val = new Value<int>();

			var tradeNotes = _LoadTradeNotes(id);

			var notes = tradeNotes.SingleOrDefault();

			if(notes == null)
			{
				throw new Exception("請檢查資料庫");
			}

			val.SetValue(notes.GetTotalMoney());
			notes.SetTradeIsUsed();
			_Database.Update(notes, a => a.Id);

			return val;
		}

		Value<FishFarmData> IFormulaFarmRecorder.Load(int farm_id)
		{
			_Logger.Info().Message("Load Farm Data To DB Start.").Property("FarmId", farm_id).Write();

			var val = new Value<FishFarmData>();
			var data = _LoadFarmData(farm_id);

			if(data == null)
			{
				var fishFarmData = new FishFarmBuilder().Get(farm_id);

				_Database.Add(fishFarmData, obj => obj.Id);

				val.SetValue(fishFarmData);

				_Logger.Info().Message("Create New Farm Data To DB Finish.").Property("FarmId", farm_id).Write();
			}
			else
			{
				val.SetValue(data);

				_Logger.Info().Message("Load Farm Data To DB Finish.").Property("FarmId", farm_id).Write();
				
			}

			return val;
		}

		Value<bool> IFormulaFarmRecorder.Save(FishFarmData data)
		{
			Server._Logger.Info().Message("Save Farm Data To DB Start.").Property("FarmId", data.FarmId).Write();

			var val = new Value<bool>();
			var farmData = _LoadFarmData(data);

			if(farmData == null)
			{
				val.SetValue(false);
			}
			else
			{
				val.SetValue(true);

				_Database.Update(data, a => a.Id);

				Server._Logger.Info().Message("Save Farm Data To DB Finish.").Property("FarmId", data.FarmId).Write();
			}

			return val;
		}

		Value<FormulaPlayerRecord> IFormulaPlayerRecorder.Query(Guid account_id)
		{
			Server._Logger.Info().Message("Player Query Run Start.").Property("PlayerId", account_id).Write();

			var val = new Value<FormulaPlayerRecord>();
			var data = _LoadFormulaPlayerRecord(account_id);

			if(data == null)
			{
				var record = new FormulaPlayerRecord
				{
					Guid = Guid.NewGuid(),
					Owner = account_id
				};

				_Database.Add(record, obj => obj.Id);
				val.SetValue(record);

				Server._Logger.Info().Message("Create New Player Run.").Property("PlayerId", account_id).Write();
			}
			else
			{
				val.SetValue(data);
			}

			Server._Logger.Info().Message("Player Query Run Finish.").Property("PlayerId", account_id).Write();

			return val;
		}

		Value<bool> IFormulaPlayerRecorder.Save(FormulaPlayerRecord record)
		{
			Server._Logger.Info().Message("Save Player Run Start.").Property("PlayerId", record.Guid).Write();
			var val = new Value<bool>();
			var recordData = _LoadFormulaPlayerRecord(record);

			if(recordData == null)
			{
				val.SetValue(false);
				Server._Logger.Fatal().Message("Save player data fail.").Write();
			}
			else
			{
				val.SetValue(true);
				_Database.Update(record, a => a.Id);

				Server._Logger.Info().Message("Save Player Run Finish.").Property("PlayerId", record.Guid).Write();
			}

			return val;
		}

		Value<ACCOUNT_REQUEST_RESULT> IAccountManager.Create(Account account)
		{
			var result = _Find(account.Name);
			if(result != null)
			{
				return ACCOUNT_REQUEST_RESULT.REPEAT;
			}

			_Database.Add(account, obj => obj.Id);
			return ACCOUNT_REQUEST_RESULT.OK;
		}

		private void _UnhandleCrash()
		{
			AppDomain.CurrentDomain.FirstChanceException += _WriteError;
		}

		private void _WriteError(object sender, FirstChanceExceptionEventArgs e)
		{
			Server._Logger.Error("error{0}", e.Exception.Message);
			Server._Logger.Error("error{0}", e.Exception.StackTrace);
		}

		private void _HandleAdministrator()
		{
			_Logger.Info().Message("_HandleAdministrator From DB Start.").Write();

			var accounts = _Find(_DefaultAdministratorName);

			_Logger.Info().Message("_HandleAdministrator From DB Finish.").Write();

			if (accounts == null)
			{
				var account = new Account
				{
					Guid = Guid.NewGuid(),
					Name = _DefaultAdministratorName,
					Password = "vgame",
					Competnces = Account.AllCompetnce()
				};

				_Database.Add(account, obj => obj.Id);

				_Database.Add(new TradeNotes
				{
					Owner = account.Guid
				}, obj => obj.Id);
			}
		}

		private void _HandleGuest()
		{
			_Logger.Info().Message("_HandleGuest From DB Start.").Write();

			var accounts = _Find("Guest");
			
			_Logger.Info().Message("_HandleGuest From DB Finish.").Write();

			if (accounts == null)
			{
				var account = new Account
				{
					Guid = Guid.NewGuid(),
					Name = "Guest",
					Password = "vgame",
					Competnces = new Flag<Account.COMPETENCE>(Account.COMPETENCE.FORMULA_QUERYER)
				};

				_Database.Add(account, obj => obj.Id);
			}
		}

		private Account _Find(string name)
		{
			return _Database.FindAll<Account>().SingleOrDefault(account => account.Name == name);
		}

		private Account _Find(Guid guid)
		{
			return _Database.FindAll<Account>().SingleOrDefault(account => account.Guid == guid);
		}

		private Value<Account[]> _QueryAllAccount()
		{
			_Logger.Info().Message("QueryAllAccount From DB Start.").Write();

			var val = new Value<Account[]>();

			var result = _Database.FindAll<Account>();

			val.SetValue(result.ToArray());

			_Logger.Info().Message("QueryAllAccount From DB Finish.").Write();

			return val;
		}

		private List<TradeNotes> _LoadTradeNotes(Guid account_id)
		{
			_Logger.Info().Message("Load TradeNotes From DB Start.").Write();
			var tradeList = _Database.FindAll<TradeNotes>().Where(obj => obj.Guid == account_id);

			_Logger.Info().Message("Load TradeNotes From DB Finish.").Write();
			return tradeList.ToList();
		}

		private FishFarmData _LoadFarmData(int farm_id)
		{
			var farmData = _Database.FindAll<FishFarmData>().Where(t => t.FarmId == farm_id);

			return farmData.FirstOrDefault();
		}

		private FishFarmData _LoadFarmData(FishFarmData data)
		{
			var farmData = _Database.Find(data, obj => obj.Id);

			return farmData;
		}

		private FormulaPlayerRecord _LoadFormulaPlayerRecord(Guid account_id)
		{
			var playerRecord = _Database.FindAll<FormulaPlayerRecord>().Where(t => t.Owner == account_id);
			return playerRecord.FirstOrDefault();
		}

		private FormulaPlayerRecord _LoadFormulaPlayerRecord(FormulaPlayerRecord record)
		{
			var data = _Database.Find(record, x => x.Id);
			return data;
		}
	}
}
