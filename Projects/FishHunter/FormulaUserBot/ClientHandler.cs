using Regulus.Framework;
using Regulus.Utility;


using VGame.Project.FishHunter.Formula;

namespace FormulaUserBot
{
	internal class ClientHandler : IUpdatable
	{
		private readonly int _BotAmount;

		private readonly Updater _Bots;

		private readonly string _IPAddress;

		private readonly int _Port;

		private int _BotCount;

		public ClientHandler(string IPAddress, int Port)
		{
			// TODO: Complete member initialization
			_IPAddress = IPAddress;
			_Port = Port;
			_Bots = new Updater();
		}

		public ClientHandler(string IPAddress, int Port, int bot_amount)
			: this(IPAddress, Port)
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
		}

		void IBootable.Shutdown()
		{
		}

		internal void Begin(GameModeSelector<IUser> selector)
		{
			selector.AddFactoty("remoting", new RemotingUserFactory());
			_OnProvider(selector.CreateUserProvider("remoting"));
		}

		private void _OnProvider(UserProvider<IUser> userProvider)
		{
			while(_BotCount < _BotAmount)
			{
				_OnUser(userProvider.Spawn("bot" + _BotCount));
				_BotCount++;
			}
		}

		private void _OnUser(IUser user)
		{
			var bot = new Bot(_IPAddress, _Port, user);
			_Bots.Add(bot);
		}

		internal void End()
		{
			_Bots.Shutdown();
		}
	}
}
