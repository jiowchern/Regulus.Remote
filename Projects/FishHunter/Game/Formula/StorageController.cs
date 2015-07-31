// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StorageController.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the StorageController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using VGame.Project.FishHunter.Common;

#endregion

namespace VGame.Project.FishHunter.Formula
{
	public struct StorageController
	{
		private readonly IAccountFinder _AccountFinder;

		public IAccountFinder AccountFinder
		{
			get { return _AccountFinder; }
		}

		public StorageController(IAccountFinder account_finder)
		{
			_AccountFinder = account_finder;
		}
	}
}