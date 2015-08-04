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

using VGame.Project.FishHunter.Common.Datas;

#endregion

namespace VGame.Project.FishHunter.Common.GPIs
{
    public interface IAccountManager : IAccountCreator
	{
		Value<Account[]> QueryAllAccount();

		
		Value<ACCOUNT_REQUEST_RESULT> Delete(string account);

		Value<ACCOUNT_REQUEST_RESULT> Update(Account account);
	}
}