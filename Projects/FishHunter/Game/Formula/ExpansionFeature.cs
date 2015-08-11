// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpansionFeature.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the ExpansionFeature type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System.CodeDom;
using System.ComponentModel.Design.Serialization;



using Regulus.Remoting;


using VGame.Project.FishHunter.Common;
using VGame.Project.FishHunter.Common.GPI;

#endregion

namespace VGame.Project.FishHunter.Formula
{
	public struct ExpansionFeature
	{
		public IFishStageDataHandler FishStageDataHandler { get; private set; }

		public IAccountFinder AccountFinder { get; private set; }

		public ExpansionFeature(IAccountFinder account_finder, IFishStageDataHandler fish_stage_data_data_loader) : this()
		{
			AccountFinder = account_finder;
			FishStageDataHandler = fish_stage_data_data_loader;
		}
	}
}