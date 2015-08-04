// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VerifyStage.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the VerifyStage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Remoting;
using Regulus.Utility;

using VGame.Project.FishHunter.Common;
using VGame.Project.FishHunter.Common.GPIs;

#endregion

namespace VGame.Project.FishHunter
{
	internal class VerifyStage : IStage
	{
		public event DoneCallback FailEvent;

		public event DoneCallback SuccessEvent;

		private readonly string _Account;

		private readonly string _Password;

		private readonly INotifier<IVerify> _Provider;

		public VerifyStage(INotifier<IVerify> provider, string account, string password)
		{
			this._Provider = provider;
			this._Account = account;
			this._Password = password;
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

		public delegate void DoneCallback();

		private void _Provider_Supply(IVerify obj)
		{
			obj.Login(_Account, _Password).OnValue += _Result;
		}

		private void _Result(bool obj)
		{
			if (obj)
			{
				SuccessEvent();
			}
			else
			{
				FailEvent();
			}
		}
	}
}