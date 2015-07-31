// --------------------------------------------------------------------------------------------------------------------
// <copyright file="User.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the User type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;

using Regulus.Framework;
using Regulus.Game;
using Regulus.Remoting;
using Regulus.Utility;

using VGame.Project.FishHunter.Common;
using VGame.Project.FishHunter.Stage;

#endregion

namespace VGame.Project.FishHunter.Storage
{
	internal class User : IUser
	{
		private event OnQuit _QuitEvent;

		private event OnNewUser _VerifySuccessEvent;

		private readonly ISoulBinder _Binder;

		private readonly StageMachine _Machine;

		private readonly IStorage _Storage;

		private Account _Account;

		public User(ISoulBinder binder, IStorage storage)
		{
			_Storage = storage;
			this._Binder = binder;
			_Machine = new StageMachine();
		}

		void IUser.OnKick(Guid id)
		{
		}

		event OnNewUser IUser.VerifySuccessEvent
		{
			add { _VerifySuccessEvent += value; }
			remove { _VerifySuccessEvent -= value; }
		}

		event OnQuit IUser.QuitEvent
		{
			add { _QuitEvent += value; }
			remove { _QuitEvent -= value; }
		}

		bool IUpdatable.Update()
		{
			_Machine.Update();
			return true;
		}

		void IBootable.Launch()
		{
			_ToVerify();
		}

		void IBootable.Shutdown()
		{
			_Machine.Termination();
		}

		private void _ToVerify()
		{
			var verify = _CreateVerify();
			_AddVerifyToStage(verify);
		}

		private Verify _CreateVerify()
		{
			_Account = null;
			var verify = new Verify(_Storage);
			return verify;
		}

		private void _AddVerifyToStage(Verify verify)
		{
			var stage = new Stage.Verify(_Binder, verify);
			stage.DoneEvent += _VerifySuccess;
			_Machine.Push(stage);
		}

		private void _VerifySuccess(Account account)
		{
			_VerifySuccessEvent(account.Id);
			_Account = account;
			_ToRelease(account);
		}

		private void _ToRelease(Account account)
		{
			var stage = new StroageAccess(_Binder, account, _Storage);
			stage.DoneEvent += _ToVerify;
			_Machine.Push(stage);
		}
	}
}