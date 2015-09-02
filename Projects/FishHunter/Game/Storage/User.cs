using System;


using Regulus.Framework;

using Regulus.Remoting;
using Regulus.Utility;


using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Common.GPI;
using VGame.Project.FishHunter.Stage;

namespace VGame.Project.FishHunter.Storage
{
	internal class User : Regulus.Game.IUser
    {
		private event Regulus.Game.OnQuit _QuitEvent;

		private event Regulus.Game.OnNewUser _VerifySuccessEvent;

		private readonly ISoulBinder _Binder;

		private readonly StageMachine _Machine;

		private readonly IStorage _Storage;

		private Account _Account;

		public User(ISoulBinder binder, IStorage storage)
		{
			_Storage = storage;
			_Binder = binder;
			_Machine = new StageMachine();
		}

		void Regulus.Game.IUser.OnKick(Guid id)
		{
		}

		event Regulus.Game.OnNewUser Regulus.Game.IUser.VerifySuccessEvent
		{
			add { _VerifySuccessEvent += value; }
			remove { _VerifySuccessEvent -= value; }
		}

		event Regulus.Game.OnQuit Regulus.Game.IUser.QuitEvent
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

		private FishHunter.Verify _CreateVerify()
		{
			_Account = null;
			var verify = new FishHunter.Verify(_Storage);
			return verify;
		}

		private void _AddVerifyToStage(FishHunter.Verify verify)
		{
			var stage = new Verify(_Binder, verify);
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
