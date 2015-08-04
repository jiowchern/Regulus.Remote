// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitialStorageStage.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the VerifyStorageStage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Utility;

using VGame.Project.FishHunter.Common;
using VGame.Project.FishHunter.Common.GPI;

#endregion

namespace VGame.Project.FishHunter.Storage
{
	public class VerifyStorageStage : IStage
	{
		public event DoneCallback DoneEvent;

		private readonly string _Account;

		private readonly string _Password;

		private readonly IUser _User;

		public VerifyStorageStage(IUser user, string account, string password)
		{
			this._Account = account;
			this._Password = password;
			this._User = user;
		}

		void IStage.Update()
		{
		}

		void IStage.Leave()
		{
			this._User.VerifyProvider.Supply -= this._ToVerify;
		}

		void IStage.Enter()
		{
			this._User.VerifyProvider.Supply += this._ToVerify;
		}

		public delegate void DoneCallback(bool result);

		private void _ToVerify(IVerify obj)
		{
			var result = obj.Login(this._Account, this._Password);
			result.OnValue += val => { this.DoneEvent(val); };
		}
	}
}