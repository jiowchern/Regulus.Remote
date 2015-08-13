using Regulus.Framework;
using Regulus.Remoting;


using VGame.Project.FishHunter;

namespace Console
{
	public class ModeCreator
	{
		private readonly ICore core;

		public ModeCreator(ICore core)
		{
			// TODO: Complete member initialization
			this.core = core;
		}

		internal void OnSelect(GameModeSelector<IUser> selector)
		{
			selector.AddFactoty("Standalone", new StandaloneUserFactory(core));
			selector.AddFactoty("remoting", new RemotingUserFactory());

			// var provider = selector.CreateUserProvider("Standalone");
			var provider = selector.CreateUserProvider("remoting");

			provider.Spawn("1");
			provider.Select("1");
		}
	}
}
