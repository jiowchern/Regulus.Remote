using Regulus.Framework;
using Regulus.Utility;


using VGame.Project.FishHunter.Formula;

namespace FormulaUserBot
{
	internal class ClientHandler : IUpdatable
	{
		private readonly int _BotAmount;

		private readonly Updater _Bots;

		private readonly string _IpAddress;

		private readonly int _Port;

		private int _BotCount;

		private const bool _IsStandalone = false;

		private readonly DummyStandalone _DummyStandalone;

		public ClientHandler(string ip_address, int port)
		{
			// TODO: Complete member initialization
			_IpAddress = ip_address;
			_Port = port;
			_Bots = new Updater();
			_DummyStandalone = new DummyStandalone();
		}

		public ClientHandler(string ip_address, int port, int bot_amount)
			: this(ip_address, port)
		{
			_BotAmount = bot_amount;
		}

		bool IUpdatable.Update()
		{
			_Bots.Working();
			return true;
		}

		void IBootable.Launch()
		{
			_Bots.Add(_DummyStandalone);
		}

		void IBootable.Shutdown()
		{
		}

		internal void Begin(GameModeSelector<IUser> selector)
		{
			if(ClientHandler._IsStandalone)
			{
				selector.AddFactoty("standalone", new StandaloneUserFactory(_DummyStandalone));
				_OnProvider(selector.CreateUserProvider("standalone"));
			}
			else
			{
				selector.AddFactoty("remoting", new RemotingUserFactory());
				_OnProvider(selector.CreateUserProvider("remoting"));	
			}
		}

		private void _OnProvider(UserProvider<IUser> user_provider)
		{
			while(_BotCount < _BotAmount)
			{
				_OnUser(user_provider.Spawn("bot" + _BotCount));
				_BotCount++;
			}
		}

		private void _OnUser(IUser user)
		{
			var bot = new Bot(_IpAddress, _Port, user);
			_Bots.Add(bot);
		}

		internal void End()
		{
			_Bots.Shutdown();
		}
	}
}
