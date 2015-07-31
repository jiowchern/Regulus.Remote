// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Complex.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Complex type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.IO;
using System.Net;

using Regulus.Framework;
using Regulus.Remoting;
using Regulus.Utility;

using VGame.Project.FishHunter.Formula;
using VGame.Project.FishHunter.Storage;

using Center = VGame.Project.FishHunter.Play.Center;
using IUser = VGame.Project.FishHunter.Formula.IUser;

#endregion

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
			get { return this._Center; }
		}

		public Complex()
		{
			this._LogRecorder = new LogFileRecorder("Play");

			this._StorageVerifyData = new Regulus.CustomType.Verify();
			this._FormulaVerifyData = new Regulus.CustomType.Verify();
			this._Machine = new StageMachine();
			this._Updater = new Updater();

			this._BuildParams();
			this._BuildUser();
		}

		void ICore.AssignBinder(ISoulBinder binder)
		{
			this._Core.AssignBinder(binder);
		}

		bool IUpdatable.Update()
		{
			this._Updater.Working();
			this._Machine.Update();
			return true;
		}

		void IBootable.Shutdown()
		{
			this._Updater.Shutdown();
			Singleton<Log>.Instance.RecordEvent -= this._LogRecorder.Record;
			AppDomain.CurrentDomain.UnhandledException -= this.CurrentDomain_UnhandledException;
		}

		void IBootable.Launch()
		{
			AppDomain.CurrentDomain.UnhandledException += this.CurrentDomain_UnhandledException;
			Singleton<Log>.Instance.RecordEvent += this._LogRecorder.Record;
			this._Updater.Add(this._Storage);
			this._Updater.Add(this._Formula);
			this._ToConnectStorage(this._StorageUser);
		}

		private void _BuildParams()
		{
			var config = new Ini(this._ReadConfig());

			this._StorageVerifyData.IPAddress = config.Read("Storage", "ipaddr");
			this._StorageVerifyData.Port = int.Parse(config.Read("Storage", "port"));
			this._StorageVerifyData.Account = config.Read("Storage", "account");
			this._StorageVerifyData.Password = config.Read("Storage", "password");

			this._FormulaVerifyData.IPAddress = config.Read("Formula", "ipaddr");
			this._FormulaVerifyData.Port = int.Parse(config.Read("Formula", "port"));
			this._FormulaVerifyData.Account = config.Read("Formula", "account");
			this._FormulaVerifyData.Password = config.Read("Formula", "password");
		}

		private void _BuildUser()
		{
			if (this._IsIpAddress(this._FormulaVerifyData.IPAddress))
			{
				this._Formula = new Client();
				this._Formula.Selector.AddFactoty("remoting", new RemotingUserFactory());
				this._FormulaUser = this._Formula.Selector.CreateUserProvider("remoting").Spawn("1");
			}
			else
			{
				var center = new Formula.Center(new StorageController(new DummyFrature()));
				this._Updater.Add(center);
				this._Formula = new Client();
				this._Formula.Selector.AddFactoty("remoting", new StandalongUserFactory(center));
				this._FormulaUser = this._Formula.Selector.CreateUserProvider("remoting").Spawn("1");
			}

			if (this._IsIpAddress(this._StorageVerifyData.IPAddress))
			{
				this._Storage = new Proxy(new RemotingFactory());
				this._StorageUser = this._Storage.SpawnUser("user");
			}
			else
			{
				var center = new Storage.Center(new DummyFrature());
				this._Updater.Add(center);
				var factory = new StandalongFactory(center);
				this._Storage = new Proxy(factory);
				this._StorageUser = this._Storage.SpawnUser("user");
			}
		}

		private bool _IsIpAddress(string ip)
		{
			IPAddress ipaddr;
			return IPAddress.TryParse(ip, out ipaddr);
		}

		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			var ex = (Exception)e.ExceptionObject;
			this._LogRecorder.Record(ex.ToString());
			this._LogRecorder.Save();
		}

		private void _ToConnectStorage(Storage.IUser user)
		{
			var stage = new ConnectStorageStage(user, this._StorageVerifyData.IPAddress, this._StorageVerifyData.Port);
			stage.DoneEvent += this._ConnectResult;
			this._Machine.Push(stage);
		}

		private void _ConnectResult(bool result)
		{
			if (result)
			{
				this._ToVerifyStorage(this._StorageUser);
			}
			else
			{
				throw new SystemException("stroage connect fail");
			}
		}

		private void _ToVerifyStorage(Storage.IUser user)
		{
			var stage = new VerifyStorageStage(user, this._StorageVerifyData.Account, this._StorageVerifyData.Password);
			stage.DoneEvent += this._VerifyResult;
			this._Machine.Push(stage);
		}

		private void _VerifyResult(bool verify_result)
		{
			if (verify_result)
			{
				this._ToConnectFormula();
			}
			else
			{
				throw new SystemException("stroage verify fail");
			}
		}

		private void _ToConnectFormula()
		{
			var stage = new ConnectStage(this._FormulaUser.Remoting.ConnectProvider, this._FormulaVerifyData.IPAddress, 
				this._FormulaVerifyData.Port);

			stage.SuccessEvent += this._ToFormulaVerify;
			stage.FailEvent += this._FormulaConnectFail;
			this._Machine.Push(stage);
		}

		private void _ToFormulaVerify()
		{
			var stage = new VerifyStage(this._FormulaUser.VerifyProvider, this._FormulaVerifyData.Account, 
				this._FormulaVerifyData.Password);

			stage.SuccessEvent += this._ToBuildClient;
			stage.FailEvent += this._FormulaVerifyFail;
			this._Machine.Push(stage);
		}

		private void _ToBuildClient()
		{
			var stage = new BuildCenterStage(this._FormulaUser, this._StorageUser);

			stage.BuiledEvent += this._Play;

			this._Machine.Push(stage);
		}

		private void _Play(BuildCenterStage.ExternalFeature features)
		{
			this._Center = new Center(features.AccountFinder, features.FishStageQueryer, features.RecordQueriers, 
				features.TradeAccount);

			this._Updater.Add(this._Center);
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