using System;


using Regulus.CustomType;
using Regulus.Net45;
using Regulus.Utility;


using VGame.Project.FishHunter.Common;
using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Common.GPI;
using VGame.Project.FishHunter.Storage;


using VGameWebApplication.Models;


using Task = System.Threading.Tasks.Task;

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

		public IGameRecorder GameRecorder { get; private set; }

		public ITradeNotes TradeNotes { get; private set; }

		public Guid ConnecterId { get; private set; }

		public Service(VerifyData data)
		{
			_Key = new object();
			_Data = data;
			_Soin = new SpinWait();

			_Proxy = new Proxy();
			(_Proxy as IUpdatable).Launch();
			_ProxyUpdate = new Task(Service._UpdateProxy, new WeakReference<Proxy>(_Proxy));
			_ProxyUpdate.Start();

			_User = _Proxy.SpawnUser("1");
		}

		~Service()
		{
			(_Proxy as IUpdatable).Shutdown();
		}

		private bool _Initial()
		{
			if(_Connect())
			{
				if(_Verify())
				{
					_GetStorageCompetnces();

					if(_Competnces[Account.COMPETENCE.ACCOUNT_MANAGER])
					{
						_GetAccountManager();
					}

					if(_Competnces[Account.COMPETENCE.ACCOUNT_FINDER])
					{
						_GetAccountFinder();
					}

					_GetAllAccountRecord();
					return true;
				}

				return false;
			}

			throw new SystemException("storage verify fail.");
		}

		private bool _Connect()
		{
			while(_User.Remoting.ConnectProvider.Ghosts.Length <= 0)
			{
				_Wait();
			}

			return _User.Remoting.ConnectProvider.Ghosts[0].Connect("127.0.0.1", 38973).WaitResult();
		}

		private bool _Verify()
		{
			while(_User.VerifyProvider.Ghosts.Length <= 0)
			{
				_Wait();
			}

			return _User.VerifyProvider.Ghosts[0].Login(_Data.Account, _Data.Password).WaitResult();
		}

		private void _GetStorageCompetnces()
		{
			var provider = _User.QueryProvider<IStorageCompetences>();
			while(provider.Ghosts.Length <= 0)
			{
				_Wait();
			}

			_Competnces = new Flag<Account.COMPETENCE>(provider.Ghosts[0].Query().WaitResult());

			ConnecterId = provider.Ghosts[0].QueryForId().WaitResult();
		}

		private void _GetAccountManager()
		{
			var provider = _User.QueryProvider<IAccountManager>();
			while(provider.Ghosts.Length <= 0)
			{
				_Wait();
			}

			AccountManager = provider.Ghosts[0];
		}

		private void _GetAccountFinder()
		{
			var provider = _User.QueryProvider<IAccountFinder>();
			while(provider.Ghosts.Length <= 0)
			{
				_Wait();
			}

			AccountFinder = provider.Ghosts[0];
		}

		private void _GetAllAccountRecord()
		{
			var provider = _User.QueryProvider<IGameRecorder>();
			while(provider.Ghosts.Length <= 0)
			{
				_Wait();
			}

			GameRecorder = provider.Ghosts[0];

			var p = _User.QueryProvider<ITradeNotes>();
			while(p.Ghosts.Length <= 0)
			{
				_Wait();
			}

			TradeNotes = p.Ghosts[0];
		}

		public void Release()
		{
		}

		private void _Wait()
		{
			_Soin.SpinOnce();
		}

		private static void _UpdateProxy(object obj)
		{
			var weak = (WeakReference<Proxy>)obj;

			var spin = new SpinWait();

			var counter = new TimeCounter();
			while(true)
			{
				Proxy proxy;

				if(weak.TryGetTarget(out proxy) == false)
				{
					break;
				}

				if(proxy.Enable == false)
				{
					break;
				}

				IUpdatable updater = proxy;
				updater.Update();
				updater = null;
				proxy = null;
				if(counter.Second >= 1.0f)
				{
					GC.Collect();
					counter.Reset();
				}
			}
		}

		internal static Guid Verify(string user, string password)
		{
			var service = new Service(
				new VerifyData
				{
					Account = user, 
					Password = password
				});
			if(service._Initial())
			{
				service.Release();
				return Singleton<KeyPool>.Instance.Query(user, password);
			}

			return Guid.Empty;
		}

		internal static Service Create(Guid id)
		{
			var data = Singleton<KeyPool>.Instance.Find(id);
			if(data != null)
			{
				var service = new Service(data);
				if(service._Initial())
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
