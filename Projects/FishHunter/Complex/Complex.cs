using System;
using System.IO;
using System.Net;
using System.Runtime.ExceptionServices;


using Regulus.Framework;
using Regulus.Remoting;
using Regulus.Utility;


using VGame.Project.FishHunter.Formula;
using VGame.Project.FishHunter.Play;
using VGame.Project.FishHunter.Storage;


using Center = VGame.Project.FishHunter.Play.Center;
using IUser = VGame.Project.FishHunter.Formula.IUser;

namespace VGame.Project.FishHunter
{
	public class Complex : ICore
	{
		private readonly LogFileRecorder _LogRecorder;

		private readonly StageMachine _Machine;

		private readonly Updater _Updater;

		private Center _Center;

		private Client _Formula;

		private IUser _FormulaUser;

		private Regulus.CustomType.Verify _FormulaVerifyData;

		private Proxy _Storage;

		private Storage.IUser _StorageUser;

		private Regulus.CustomType.Verify _StorageVerifyData;

		private ICore _Core
		{
			get { return _Center; }
		}

		public Complex()
		{           
			_LogRecorder = new LogFileRecorder("Play");

			_StorageVerifyData = new Regulus.CustomType.Verify();
			_FormulaVerifyData = new Regulus.CustomType.Verify();
			_Machine = new StageMachine();
			_Updater = new Updater();

			_BuildParams();
			_BuildUser();
		}

		void ICore.AssignBinder(ISoulBinder binder)
		{
			_Core.AssignBinder(binder);
		}

		bool IUpdatable.Update()
		{
			_Updater.Working();
			_Machine.Update();
			return true;
		}

		void IBootable.Shutdown()
		{
			_Updater.Shutdown();
			Singleton<Log>.Instance.RecordEvent -= _LogRecorder.Record;
            _LogRecorder.Save();

        }

		void IBootable.Launch()
		{
		    AppDomain.CurrentDomain.FirstChanceException += _WriteError;
            
			Singleton<Log>.Instance.RecordEvent += _LogRecorder.Record;
			_Updater.Add(_Storage);
			_Updater.Add(_Formula);
			_ToConnectStorage(_StorageUser);
		}

	    

	    private void _WriteError(object sender, FirstChanceExceptionEventArgs e)
	    {
	        
            _LogRecorder.Record($"Exception:{e.Exception.Message}\r\nStackTrace:{e.Exception.StackTrace}");
            _LogRecorder.Save();
        }

	    private void _BuildParams()
		{
			var config = new Ini(_ReadConfig());

			_StorageVerifyData.IPAddress = config.Read("Storage", "ipaddr");
			_StorageVerifyData.Port = int.Parse(config.Read("Storage", "port"));
			_StorageVerifyData.Account = config.Read("Storage", "account");
			_StorageVerifyData.Password = config.Read("Storage", "password");

			_FormulaVerifyData.IPAddress = config.Read("Formula", "ipaddr");
			_FormulaVerifyData.Port = int.Parse(config.Read("Formula", "port"));
			_FormulaVerifyData.Account = config.Read("Formula", "account");
			_FormulaVerifyData.Password = config.Read("Formula", "password");
		}

		private void _BuildUser()
		{

            
			if(_IsIpAddress(_FormulaVerifyData.IPAddress))
			{
				_Formula = new Client();
				_Formula.Selector.AddFactoty("remoting", new RemotingUserFactory());
				_FormulaUser = _Formula.Selector.CreateUserProvider("remoting").Spawn("1");
			}
			else
			{
				var center = new Formula.Center(new ExpansionFeature(new DummyFrature(), new DummyFrature(), new DummyFrature()));
				_Updater.Add(center);
				_Formula = new Client();
				_Formula.Selector.AddFactoty("remoting", new StandaloneUserFactory(center));
				_FormulaUser = _Formula.Selector.CreateUserProvider("remoting").Spawn("1");
			}

			if(_IsIpAddress(_StorageVerifyData.IPAddress))
			{
				_Storage = new Proxy(new RemotingFactory());
				_StorageUser = _Storage.SpawnUser("user");
			}
			else
			{
				var center = new Storage.Center(new DummyFrature());
				_Updater.Add(center);
				var factory = new StandaloneFactory(center);
				_Storage = new Proxy(factory);
				_StorageUser = _Storage.SpawnUser("user");
			}
		}

		private bool _IsIpAddress(string ip)
		{
			IPAddress ipaddr;
			return IPAddress.TryParse(ip, out ipaddr);
		}

		

		private void _ToConnectStorage(Storage.IUser user)
		{
			var stage = new ConnectStorageStage(user, _StorageVerifyData.IPAddress, _StorageVerifyData.Port);
			stage.OnDoneEvent += _ConnectResult;
			_Machine.Push(stage);
		}

		private void _ConnectResult(bool result)
		{
			if(result)
			{
				_ToVerifyStorage(_StorageUser);
			}
			else
			{
				throw new SystemException("stroage connect fail");
			}
		}

		private void _ToVerifyStorage(Storage.IUser user)
		{
			var stage = new VerifyStorageStage(user, _StorageVerifyData.Account, _StorageVerifyData.Password);
			stage.OnDoneEvent += _VerifyResult;
			_Machine.Push(stage);
		}

		private void _VerifyResult(bool verify_result)
		{
			if(verify_result)
			{
				_ToConnectFormula();
			}
			else
			{
				throw new SystemException("stroage verify fail");
			}
		}

		private void _ToConnectFormula()
		{
			var stage = new ConnectStage(
				_FormulaUser.Remoting.ConnectProvider, 
				_FormulaVerifyData.IPAddress, 
				_FormulaVerifyData.Port);

			stage.OnSuccessEvent += _ToFormulaVerify;

			stage.OnFailEvent += _FormulaConnectFail;

			_Machine.Push(stage);
		}

		private void _ToFormulaVerify()
		{
			var stage = new VerifyStage(
				_FormulaUser.VerifyProvider, 
				_FormulaVerifyData.Account, 
				_FormulaVerifyData.Password);

			stage.OnSuccessEvent += _ToBuildClient;

			stage.OnFailEvent += _FormulaVerifyFail;

			_Machine.Push(stage);
		}

		private void _ToBuildClient()
		{
			var stage = new BuildCenterStage(_FormulaUser, _StorageUser);

			stage.OnBuiledEvent += _Play;

			_Machine.Push(stage);
		}

		private void _Play(BuildCenterStage.ExternalFeature features)
		{            
            _Center = new Center(
				features.AccountFinder, 
				features.FishStageQueryer, 
				features.GameRecorder, 
				features.TradeAccount);

			_Updater.Add(_Center);
		}

		private void _FormulaVerifyFail()
		{
			throw new SystemException("formula verify fail");
		}

		private void _FormulaConnectFail()
		{
			throw new SystemException("formula connect fail");
		}

		private string _ReadConfig()
		{
			return File.ReadAllText("config.ini");
		}
	}
}
