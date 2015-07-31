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

namespace VGame.Project.FishHunter.Formula
{
	internal class User : IUser
	{
		private event OnQuit _QuitEvent;

		private event OnNewUser _VerifySuccessEvent;

		private readonly ISoulBinder _Binder;

		private readonly StageMachine _Machine;

		private Account _Account;

		private StorageController _Controller;

		private User(ISoulBinder binder)
		{
			_Machine = new StageMachine();
			_Binder = binder;
		}

		public User(ISoulBinder binder, StorageController _Controller) : this(binder)
		{
			this._Controller = _Controller;
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
			_Account = null;
			var verify = new Verify(_Controller.AccountFinder);
			var stage = new Stage.Verify(_Binder, verify);
			stage.DoneEvent += _VerifySuccess;
			_Machine.Push(stage);
		}

		private void _VerifySuccess(Account account)
		{
			if (account.IsFormulaQueryer())
			{
				_Account = account;
				_VerifySuccessEvent(_Account.Id);
				_ToFishStage();
			}
			else
			{
				_ToVerify();
			}
		}

		private void _ToFishStage()
		{
			var stage = new FormulaStage(_Binder);
			stage.OnDoneEvent += () => { _QuitEvent(); };
			_Machine.Push(stage);
		}
	}
}