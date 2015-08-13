using Regulus.Utility;


using VGame.Project.FishHunter.Common.GPI;

namespace VGame.Project.FishHunter.Storage
{
	public class VerifyStorageStage : IStage
	{
		public delegate void DoneCallback(bool result);

		public event DoneCallback OnDoneEvent;

		private readonly string _Account;

		private readonly string _Password;

		private readonly IUser _User;

		public VerifyStorageStage(IUser user, string account, string password)
		{
			_Account = account;
			_Password = password;
			_User = user;
		}

		void IStage.Update()
		{
		}

		void IStage.Leave()
		{
			_User.VerifyProvider.Supply -= _ToVerify;
		}

		void IStage.Enter()
		{
			_User.VerifyProvider.Supply += _ToVerify;
		}

		private void _ToVerify(IVerify obj)
		{
			var result = obj.Login(_Account, _Password);
			result.OnValue += val => { OnDoneEvent(val); };
		}
	}
}
