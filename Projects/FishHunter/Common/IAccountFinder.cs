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

#endregion

namespace VGame.Project.FishHunter.Common
{
	public interface IAccountFinder
	{
		Value<Account> FindAccountByName(string id);

		Value<Account> FindAccountById(Guid accountId);
	}
}