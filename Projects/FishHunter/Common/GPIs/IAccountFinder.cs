// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAccountFinder.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the IAccountFinder type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;

using Regulus.Remoting;

using VGame.Project.FishHunter.Common.Datas;

#endregion

namespace VGame.Project.FishHunter.Common.GPIs
{
	public interface IAccountFinder
	{
		Value<Account> FindAccountByName(string id);

		Value<Account> FindAccountById(Guid accountId);
	}
}