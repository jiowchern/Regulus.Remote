// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAccountManager.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the IAccountManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Remoting;

#endregion

namespace VGame.Project.FishHunter.Common
{
	public interface IAccountManager
	{
		Value<Account[]> QueryAllAccount();

		Value<ACCOUNT_REQUEST_RESULT> Create(Account account);

		Value<ACCOUNT_REQUEST_RESULT> Delete(string account);

		Value<ACCOUNT_REQUEST_RESULT> Update(Account account);
	}
}