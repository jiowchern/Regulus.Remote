using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;


using NLog;
using NLog.Fluent;

using Regulus.CustomType;
using Regulus.Database.DB_NoSQL;
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
        private readonly Center _Center;

        private readonly Database _Database;

        private readonly string _DefaultAdministratorName;

        private readonly string _Ip;

        private readonly LogFileRecorder _LogRecorder;

        private readonly string _Name;

        private readonly Updater _Updater;

		private static readonly Logger _Logger = LogManager.GetCurrentClassLogger();

		private ICore _Core => _Center;

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
            _UnhandleCrash();
            Singleton<Regulus.Utility.Log>.Instance.RecordEvent += _LogRecorder.Record;
            

            _Updater.Add(_Center);
            _Database.Launch(_Name);

            _HandleAdministrator();

            _HandleGuest();

            // _HandleTradeRecord();
        }

        private void _UnhandleCrash()
        {            
            AppDomain.CurrentDomain.FirstChanceException += _WriteError;

        }

        private void _WriteError(object sender, FirstChanceExceptionEventArgs e)
        {
			_Logger.Error("error{0}", e.Exception.Message);
			_Logger.Error("error{0}", e.Exception.StackTrace);
		}
        

        void IBootable.Shutdown()
        {
            _Database.Shutdown();
            _Updater.Shutdown();
            
            Singleton<Log>.Instance.RecordEvent -= _LogRecorder.Record;
            _LogRecorder.Save();
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

        Value<ACCOUNT_REQUEST_RESULT> IAccountManager.Delete(string account)
        {
            var result = _Find(account);
            if(result != null && _Database.Remove<Account>(a => a.Id == result.Id))
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
            if(_Database.Update(account, a => a.Id == account.Id))
            {
                return ACCOUNT_REQUEST_RESULT.OK;
            }

            return ACCOUNT_REQUEST_RESULT.NOTFOUND;
        }

        Value<Account> IAccountFinder.FindAccountById(Guid accountId)
        {
            return _Find(accountId);
        }

        Value<GamePlayerRecord> IGameRecorder.Load(Guid account_id)
        {
            var val = new Value<GamePlayerRecord>();
            var account = _Find(account_id);
            if(account.IsPlayer())
            {
                var recordTask = _Database.Find<GamePlayerRecord>(r => r.Owner == account_id);
                recordTask.ContinueWith(
                    task =>
                    {
                        if(task.Result.Count > 0)
                        {
                            val.SetValue(task.Result.FirstOrDefault());
                        }
                        else
                        {
                            var newRecord = new GamePlayerRecord
                            {
                                Id = Guid.NewGuid(), 
                                Owner = account_id, 
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

        void IGameRecorder.Save(GamePlayerRecord record)
        {
            _Database.Update(record, r => r.Id == record.Id);
        }

        Value<TradeNotes> ITradeNotes.Find(Guid id)
        {
            var val = new Value<TradeNotes>();
            var t = _LoadTradeNotesTask(id);
            t.ContinueWith(
                task =>
                {
                    var notes = task.Result.SingleOrDefault();
                    val.SetValue(notes);
                });

            return val;
        }

        Value<TradeNotes> ITradeNotes.Load(Guid account_id)
        {
            var val = new Value<TradeNotes>();
            var t = _LoadTradeNotesTask(account_id);
            t.ContinueWith(
                task =>
                {
                    var notes = task.Result.SingleOrDefault();
                    if(notes == null)
                    {
                        var newPlayerNotes = new TradeNotes(account_id);
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
            t.ContinueWith(
                task =>
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
            t.ContinueWith(
                task =>
                {
                    var notes = task.Result.SingleOrDefault();

                    if(notes == null)
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

        Value<FishFarmData> IFormulaFarmRecorder.Load(int farm_id)
        {
            var val = new Value<FishFarmData>();
            var t = _LoadFarmData(farm_id);

            t.ContinueWith(
                task =>
                {
                    var data = task.Result;
                    if(data == null)
                    {
                        var fishFarmData = new FishFarmBuilder().Get(farm_id);

						_Database.Add(fishFarmData).Wait();

						val.SetValue(fishFarmData);

						_Logger.Info().Message("Builder new farm data.").Write();
					}
                    else
                    {
                        val.SetValue(data);

						_Logger.Info().Message("Load db farm data.").Write();
					}
                });
            return val;
        }

        Value<bool> IFormulaFarmRecorder.Save(FishFarmData data)
        {
            var val = new Value<bool>();
            var t = _LoadFarmData(data.FarmId);

            t.ContinueWith(
                task =>
                {
                    var farmData = task.Result;

                    if(farmData == null)
                    {
                        val.SetValue(false);

                    }
                    else
                    {
                        val.SetValue(true);

                        _Database.Update(data, a => a.FarmId == data.FarmId);

						_Logger.Info().Message("Load db farm data.").Write();
					}
                });
            return val;
        }

        Value<FormulaPlayerRecord> IFormulaPlayerRecorder.Query(Guid account_id)
        {
            var val = new Value<FormulaPlayerRecord>();
            var t = _LoadFormulaPlayerRecord(account_id);

            t.ContinueWith(
                task =>
                {
                    var data = task.Result;
                    if(data == null)
                    {
                        var record = new FormulaPlayerRecord
                        {
                            Id = Guid.NewGuid(), 
                            Owner = account_id
                        };

                        _Database.Add(record).Wait();
                        val.SetValue(record);

                        _Logger.Info().Message("new player").Property("PlayerId", account_id).Write();
                    }
                    else
                    {
                        val.SetValue(data);
                    }
                });
            return val;
        }

        Value<bool> IFormulaPlayerRecorder.Save(FormulaPlayerRecord record)
        {
            var val = new Value<bool>();
            var t = _LoadFormulaPlayerRecord(record.Owner);

            t.ContinueWith(
                task =>
                {
                    var recordData = task.Result;

                    if(recordData == null)
                    {
                        val.SetValue(false);
						_Logger.Fatal().Message("update player data fail").Write();
					}
                    else
                    {
                        val.SetValue(true);
                        _Database.Update(record, a => a.Id == record.Id);
	                  

	                    _Logger.Info().Message("update player data to db").Write();
                    }
                });
            return val;
        }

        Value<ACCOUNT_REQUEST_RESULT> IAccountManager.Create(Account account)
        {
            var result = _Find(account.Name);
            if(result != null)
            {
                return ACCOUNT_REQUEST_RESULT.REPEAT;
            }

            _Database.Add(account);
            return ACCOUNT_REQUEST_RESULT.OK;
        }

        private async void _HandleAdministrator()
        {
            var accounts = await _Database.Find<Account>(a => a.Name == _DefaultAdministratorName);

            if(accounts.Count == 0)
            {
                var account = new Account
                {
                    Id = Guid.NewGuid(), 
                    Name = _DefaultAdministratorName, 
                    Password = "vgame", 
                    Competnces = Account.AllCompetnce()
                };

                await _Database.Add(account);

                await _Database.Add(
                    new TradeNotes
                    {
                        Owner = account.Id
                    });
            }
        }

        private async void _HandleGuest()
        {
            var accounts = await _Database.Find<Account>(a => a.Name == "Guest");

            if(accounts.Count == 0)
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

        private Task<List<TradeNotes>> _LoadTradeNotesTask(Guid account_id)
        {
            var tradeTask = _Database.Find<TradeNotes>(t => t.Owner == account_id);

            var returnTask = tradeTask.ContinueWith(
                task =>
                {
                    Singleton<Log>.Instance.WriteDebug("TradeNotes Find Done.");

					_Logger.Info().Message("TradeNotes Find Done.").Write();

					if (task.Exception != null)
                    {
						_Logger.Fatal().Message("TradeNotes Exception").Exception(task.Exception).Write();
                    }

                    return task.Result;
                });
            return returnTask;
        }

        private Task<FishFarmData> _LoadFarmData(int farm_id)
        {
            var tradeTask = _Database.Find<FishFarmData>(t => t.FarmId == farm_id);

            var returnTask = tradeTask.ContinueWith(
                task =>
                {
                    if(task.Exception != null)
                    {
                        foreach(var exception in task.Exception.InnerExceptions)
                        {
							_Logger.Fatal().Message("FishFarmData Exception").Exception(exception).Write();
                        }                        
                    }

					_Logger.Info().Message("FishFarmData Find Done.").Write();

					return task.Result.FirstOrDefault();
                });

            return returnTask;
        }

        private Task<FormulaPlayerRecord> _LoadFormulaPlayerRecord(Guid account_id)
        {
            var tradeTask = _Database.Find<FormulaPlayerRecord>(t => t.Owner == account_id);

            var returnTask = tradeTask.ContinueWith(
                task =>
                {                    
                    if(task.Exception != null)
                    {
						_Logger.Fatal().Message("FormulaPlayerRecord Exception").Exception(task.Exception).Write();
                    }

                    return task.Result.FirstOrDefault();
                });

            return returnTask;
        }
    }
}
