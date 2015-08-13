using Regulus.Remoting;
using Regulus.Utility;


using VGame.Project.FishHunter.Common;
using VGame.Project.FishHunter.Common.GPI;

namespace VGame.Project.FishHunter
{
	internal class VerifyStage : IStage
	{
		public delegate void DoneCallback();

		public event DoneCallback OnFailEvent;

		public event DoneCallback OnSuccessEvent;

		private readonly string _Account;

		private readonly string _Password;

		private readonly INotifier<IVerify> _Provider;

		public VerifyStage(INotifier<IVerify> provider, string account, string password)
		{
			_Provider = provider;
			_Account = account;
			_Password = password;
		}

		void IStage.Enter()
		{
			_Provider.Supply += _Provider_Supply;
		}

		void IStage.Leave()
		{
		}

		void IStage.Update()
		{
		}

		private void _Provider_Supply(IVerify obj)
		{
			obj.Login(_Account, _Password).OnValue += _Result;
		}

		private void _Result(bool obj)
		{
			if(obj)
			{
				OnSuccessEvent();
			}
			else
			{
				OnFailEvent();
			}
		}
	}
}
