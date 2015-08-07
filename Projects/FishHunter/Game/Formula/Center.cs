// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Center.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the Center type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Framework;
using Regulus.Remoting;
using Regulus.Utility;

using VGame.Project.FishHunter.Play;

#endregion

namespace VGame.Project.FishHunter.Formula
{
	public class Center : ICore
	{
		private readonly ExpansionFeature _ExpansionFeature;

		private readonly Hall _Hall;

		private readonly Updater _Updater;

		public Center(ExpansionFeature expansion_feature) 
		{
			_ExpansionFeature = expansion_feature;

			_Hall = new Hall();
			_Updater = new Updater();
		}

		void ICore.AssignBinder(ISoulBinder binder)
		{
			_Hall.PushUser(new User(binder, _ExpansionFeature));
		}

		bool IUpdatable.Update()
		{
			_Updater.Working();
			return true;
		}

		void IBootable.Launch()
		{
			_Updater.Add(_Hall);
		}

		void IBootable.Shutdown()
		{
			_Updater.Shutdown();
		}
	}
}