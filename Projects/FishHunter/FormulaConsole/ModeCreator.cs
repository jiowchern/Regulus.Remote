// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModeCreator.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the ModeCreator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Framework;
using Regulus.Remoting;

#endregion

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
			// selector.AddFactoty("standalong", new VGame.Project.FishHunter.Formula.StandalongUserFactory(core));
			selector.AddFactoty("remoting", new RemotingUserFactory());

			// var provider = selector.CreateUserProvider("standalong");
			// var provider = selector.CreateUserProvider("remoting");

			// provider.Spawn("1");
			// provider.Select("1");
		}
	}
}