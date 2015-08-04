// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StorageController.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the StorageController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System.CodeDom;
using System.ComponentModel.Design.Serialization;
using System.Data;


using Regulus.Remoting;


using VGame.Project.FishHunter.Common;
using VGame.Project.FishHunter.Common.GPIs;

#endregion

namespace VGame.Project.FishHunter.Formula
{
	public struct StorageController
	{
		private readonly IAccountFinder _AccountFinder;

		//private IStageFinder IStageFinder;

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