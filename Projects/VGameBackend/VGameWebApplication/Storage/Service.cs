// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Service.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Service type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;

using Regulus.CustomType;
using Regulus.Net45;
using Regulus.Utility;

using VGame.Project.FishHunter.Common;
using VGame.Project.FishHunter.Storage;

using VGameWebApplication.Models;

using Task = System.Threading.Tasks.Task;

#endregion

namespace VGameWebApplication.Storage
{
	internal class Service
	{
		private readonly VerifyData _Data;

		private readonly Proxy _Proxy;

		private readonly Task _ProxyUpdate;

		private readonly SpinWait _Soin;

		private readonly IUser _User;

		private Flag<Account.COMPETENCE> _Competnces;

		private object _Key;

		public IAccountManager AccountManager { get; private set; }

		public IAccountFinder AccountFinder { get; private set; }

		public IRecordQueriers RecodeQueriers { get; private set; }

		public ITradeNotes TradeNotes { get; private set; }

		public Guid ConnecterId { get; private set; }

		public Service(VerifyData data)
		{
			this._Key = new object();
			this._Data = data;
			this._Soin = new SpinWait();

			this._Proxy = new Proxy();
			(this._Proxy as IUpdatable).Launch();
			this._ProxyUpdate = new Task(Service._UpdateProxy, new WeakReference<Proxy>(this._Proxy));
			this._ProxyUpdate.Start();

			this._User = this._Proxy.SpawnUser("1");
		}

		~Service()
		{
			(this._Proxy as IUpdatable).Shutdown();
		}

		private bool _Initial()
		{
			if (this._Connect())
			{
				if (this._Verify())
				{
					this._GetStorageCompetnces();

					if (this._Competnces[Account.COMPETENCE.ACCOUNT_MANAGER])
					{
						this._GetAccountManager();
					}

					if (this._Competnces[Account.COMPETENCE.ACCOUNT_FINDER])
					{
						this._GetAccountFinder();
					}

					this._GetAllAccountRecode();
					return true;
				}

				return false;
			}

			throw new SystemException("storage verify fail.");
		}

		private bool _Connect()
		{
			while (this._User.Remoting.ConnectProvider.Ghosts.Length <= 0)
			{
				this._Wait();
			}

			return this._User.Remoting.ConnectProvider.Ghosts[0].Connect("127.0.0.1", 38973).WaitResult();
		}

		private bool _Verify()
		{
			while (this._User.VerifyProvider.Ghosts.Length <= 0)
			{
				this._Wait();
			}

			return this._User.VerifyProvider.Ghosts[0].Login(this._Data.Account, this._Data.Password).WaitResult();
		}

		private void _GetStorageCompetnces()
		{
			var provider = this._User.QueryProvider<IStorageCompetences>();
			while (provider.Ghosts.Length <= 0)
			{
				this._Wait();
			}

			this._Competnces = new Flag<Account.COMPETENCE>(provider.Ghosts[0].Query().WaitResult());

			this.ConnecterId = provider.Ghosts[0].QueryForId().WaitResult();
		}

		private void _GetAccountManager()
		{
			var provider = this._User.QueryProvider<IAccountManager>();
			while (provider.Ghosts.Length <= 0)
			{
				this._Wait();
			}

			this.AccountManager = provider.Ghosts[0];
		}

		private void _GetAccountFinder()
		{
			var provider = this._User.QueryProvider<IAccountFinder>();
			while (provider.Ghosts.Length <= 0)
			{
				this._Wait();
			}

			this.AccountFinder = provider.Ghosts[0];
		}

		private void _GetAllAccountRecode()
		{
			// var accounts = AccountManager.QueryAllAccount().WaitResult();
			var provider = this._User.QueryProvider<IRecordQueriers>();
			while (provider.Ghosts.Length <= 0)
			{
				this._Wait();
			}

			this.RecodeQueriers = provider.Ghosts[0];

			var p = this._User.QueryProvider<ITradeNotes>();
			while (p.Ghosts.Length <= 0)
			{
				this._Wait();
			}

			this.TradeNotes = p.Ghosts[0];

			// int money = provider.Ghosts[0].Load(accounts[0].Key).WaitResult().Money;
		}

		public void Release()
		{
		}

		private void _Wait()
		{
			this._Soin.SpinOnce();
		}

		private static void _UpdateProxy(object obj)
		{
			var weak = (WeakReference<Proxy>)obj;

			var spin = new SpinWait();

			var counter = new TimeCounter();
			while (true)
			{
				Proxy proxy;

				if (weak.TryGetTarget(out proxy) == false)
				{
					break;
				}

				if (proxy.Enable == false)
				{
					break;
				}

				IUpdatable updater = proxy;
				updater.Update();
				updater = null;
				proxy = null;
				if (counter.Second >= 1.0f)
				{
					GC.Collect();
					counter.Reset();
				}
			}
		}

		internal static Guid Verify(string user, string password)
		{
			var service = new Service(new VerifyData
			{
				Account = user, 
				Password = password
			});
			if (service._Initial())
			{
				service.Release();
				return Singleton<KeyPool>.Instance.Query(user, password);
			}

			return Guid.Empty;
		}

		internal static Service Create(Guid id)
		{
			var data = Singleton<KeyPool>.Instance.Find(id);
			if (data != null)
			{
				var service = new Service(data);
				if (service._Initial())
				{
					return service;
				}
			}

			return null;
		}

		internal static Service Create(object p)
		{
			return Service.Create((Guid)p);
		}

		internal static void Destroy(Guid guid)
		{
			Singleton<KeyPool>.Instance.Destroy(guid);
		}
	}
}