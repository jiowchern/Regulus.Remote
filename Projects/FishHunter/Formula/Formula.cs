using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Configuration;
using System.Reflection;
using System.Runtime.ExceptionServices;


using Regulus.Collection;
using Regulus.Framework;
using Regulus.Remoting;
using Regulus.Utility;


using VGame.Project.FishHunter.Storage;

namespace VGame.Project.FishHunter.Formula
{
	public class Server : ICore
	{
		private string _Account;

		private Queue<ISoulBinder> _Binders;

		private bool _Enable;

		private string _IpAddress;

		private LogFileRecorder _LogRecorder;

		private StageMachine _Machine;

		private string _Password;

		private int _Port;

		private Proxy _Storage;

		private IUser _StorageUser;

		private Updater _Updater;

	    

	    public Server()
		{
			_Setup();
		}

		void ICore.AssignBinder(ISoulBinder binder)
		{
			_Binders.Enqueue(binder);
		}

		bool IUpdatable.Update()
		{
			_Updater.Working();
			_Machine.Update();
			return _Enable;
		}

		void IBootable.Launch()
		{
			Server._PreloadAssembly();

			_Updater.Add(_Storage);
			_ToConnectStorage(_Storage.SpawnUser("user"));
		}

		void IBootable.Shutdown()
		{
			_ReleaseLog();

			_Updater.Shutdown();
			_Machine.Termination();
		}

		private void _Setup()
		{
		    _UnhandleCrash();
			_InitialLog();

			var config = new Ini(_ReadConfig());

			_IpAddress = config.Read("Storage", "ipaddr");
			_Port = int.Parse(config.Read("Storage", "port"));
			_Account = config.Read("Storage", "account");
			_Password = config.Read("Storage", "password");

			_Storage = new Proxy();
			_Machine = new StageMachine();
			_Updater = new Updater();
			_Binders = new Queue<ISoulBinder>();
			_Enable = true;
		}

	    private void _UnhandleCrash()
	    {
	        
	        AppDomain.CurrentDomain.FirstChanceException += _WriteDump;
	    }

	    private void _WriteDump(object sender, FirstChanceExceptionEventArgs e)
	    {
            _LogRecorder.Record($"Exception:{e.Exception.Message}\r\nStackTrace:{e.Exception.StackTrace}");
            _LogRecorder.Save();            
	    }

	    private string _ReadConfig()
		{
			return File.ReadAllText("config.ini");
		}

		private static void _PreloadAssembly()
		{
			
		}

		private void _InitialLog()
		{
			_LogRecorder = new LogFileRecorder("Formula");
			Singleton<Log>.Instance.RecordEvent += _LogRecorder.Record;
		}

		

		private void _ReleaseLog()
		{
			
			_LogRecorder.Save();
		}

	    private void _ToConnectStorage()
	    {            
	        _ToConnectStorage(_StorageUser);
	    }
        private void _ToConnectStorage(IUser user)
		{
            if(_StorageUser != null)
            {
                _StorageUser.Remoting.OnlineProvider.Unsupply -= _Restart;
        
            }
			_StorageUser = user;
            _StorageUser.Remoting.ErrorMessageEvent += (msg) => { Log.Instance.WriteInfo(string.Format("StorageErrorLog:{0}", msg)); };
            _StorageUser.Remoting.OnlineProvider.Unsupply += _Restart;
            var stage = new ConnectStorageStage(user, _IpAddress, _Port);
			stage.OnDoneEvent += _ConnectResult;
			_Machine.Push(stage);
		}

	    

	    private void _Restart(IOnline obj)
	    {
	        Log.Instance.WriteInfo("Storage disconnect , need restart.");
            _ToConnectStorage();
	    }

	    private void _ConnectResult(bool result)
		{
			if(result)
			{
			    
                _ToVerifyStorage(_StorageUser);
			}
			else
			{
			    _ToWaitReconnect();
			}
		}

	    private void _ToWaitReconnect()
	    {
	        float second = 3.0f;	        
	        Log.Instance.WriteInfo(string.Format("Can't connect storage server , waittig {0} second to reconnect ... ", second));

	        var stage = new Regulus.Game.TimerStage(second);
	        stage.DoneEvent += _ToConnectStorage;
            _Machine.Push(stage);
        }

	    private void _ToVerifyStorage(IUser user)
		{
			var stage = new VerifyStorageStage(user, _Account, _Password);
			stage.OnDoneEvent += _VerifyResult;
			_Machine.Push(stage);
		}

		private void _VerifyResult(bool verify_result)
		{
			if(verify_result)
			{
				_ToBuildStorageController();
			}
			else
			{
				throw new SystemException("stroage verify fail");
			}
		}

		private void _ToBuildStorageController()
		{
			var stage = new BuildStorageControllerStage(_StorageUser);
			stage.OnDoneEvent += _ToRunFormula;
			_Machine.Push(stage);
		}

		private void _ToRunFormula(ExpansionFeature expansion_feature)
		{
			var stage = new RunFormulaStage( expansion_feature, _Binders);
			stage.DoneEvent += _ToShutdown;
			_Machine.Push(stage);
		}

		private void _ToShutdown()
		{
			_Enable = false;
		}
	}
}


