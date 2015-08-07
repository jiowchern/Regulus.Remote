// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStorage.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the IStorage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace VGame.Project.FishHunter.Common.GPI
{
	public interface IStorage : IAccountFinder, IAccountManager, IRecordHandler, ITradeNotes, IFishStageDataHandler
	{
	}
}