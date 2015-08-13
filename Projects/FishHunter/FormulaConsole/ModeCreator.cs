using Regulus.Framework;
using Regulus.Remoting;

namespace VGame.Project.FishHunter.Formula
{
	public class ModeCreator
	{
		private ICore core;

		public ModeCreator(ICore core)
		{
			// TODO: Complete member initialization
			this.core = core;
		}

		internal void OnSelect(GameModeSelector<IUser> selector)
		{
			// selector.AddFactoty("Standalone", new VGame.Project.FishHunter.Formula.StandaloneUserFactory(core));
			selector.AddFactoty("remoting", new RemotingUserFactory());

			// var provider = selector.CreateUserProvider("Standalone");
			// var provider = selector.CreateUserProvider("remoting");

			// provider.Spawn("1");
			// provider.Select("1");
		}
	}
}
