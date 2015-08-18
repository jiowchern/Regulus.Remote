using System.Runtime.CompilerServices;


using Regulus.Framework;
using Regulus.Utility;

namespace VGame.Project.FishHunter.Storage
{
	public class Proxy :
		IUpdatable, Console.IViewer, Console.IInput
	{
		private readonly Client<IUser> _Client;

		private readonly Updater _Updater;

		private readonly IUserFactoty<IUser> _UserFactory;

		private UserProvider<IUser> _UserProvider;

	    private Center _Standalone;

	    public bool Enable
		{
			get { return _Client.Enable; }
		}

		public Proxy(IUserFactoty<IUser> custom)
		{
			_UserFactory = custom;
			_Client = new Client<IUser>(this, this);
			_Updater = new Updater();

			Client_ModeSelectorEvent(_Client.Selector);
		}

	    public Proxy(IUserFactoty<IUser> custom, Center standalone):this(custom)
	    {
	        _Standalone = standalone;
	    }

        public Proxy()
		{
			_UserFactory = new RemotingFactory();
			_Client = new Client<IUser>(this, this);
			_Updater = new Updater();

			Client_ModeSelectorEvent(_Client.Selector);
		}

		event Console.OnOutput Console.IInput.OutputEvent
		{
			add { }
			remove { }
		}

		bool IUpdatable.Update()
		{
			_Updater.Working();
			return _Client.Enable;
		}

		void IBootable.Launch()
		{
			_Updater.Add(_Client);

		    if(_Standalone != null)
		    {
                _Updater.Add(_Standalone);
            }
		}

		void IBootable.Shutdown()
		{
			_Updater.Shutdown();
		}

		void Console.IViewer.WriteLine(string message)
		{
		}

		void Console.IViewer.Write(string message)
		{
		}

		private void Client_ModeSelectorEvent(GameModeSelector<IUser> selector)
		{
			selector.AddFactoty("fac", _UserFactory);
			_UserProvider = selector.CreateUserProvider("fac");
		}

		public IUser SpawnUser(string name)
		{
			return _UserProvider.Spawn(name);
		}

		public void UnspawnUser(string name)
		{
			_UserProvider.Unspawn(name);
		}
	}
}
