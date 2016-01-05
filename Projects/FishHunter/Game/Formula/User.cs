﻿using System;

using Regulus.Framework;
using Regulus.Game;
using Regulus.Remoting;
using Regulus.Utility;

using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Common.GPI;
using VGame.Project.FishHunter.Stage;

namespace VGame.Project.FishHunter.Formula
{
	internal class User : IUser, IVersion
	{
		private event OnQuit _OnQuitEvent;

		private event OnNewUser _OnVerifySuccessEvent;

		private readonly ISoulBinder _Binder;

		private readonly StageMachine _Machine;

		private Account _Account;

		private ExpansionFeature _ExpansionFeature;

		private readonly string _Version;

		private User(ISoulBinder binder)
		{
			_Machine = new StageMachine();
			_Binder = binder;

			_Version = typeof(IVerify).Assembly.GetName()
									.Version.ToString();
		}

		public User(ISoulBinder binder, ExpansionFeature expansion_feature) : this(binder)
		{
			_ExpansionFeature = expansion_feature;
		}

		void IUser.OnKick(Guid id)
		{
		}

		event OnNewUser IUser.VerifySuccessEvent
		{
			add
			{
				_OnVerifySuccessEvent += value;
			}

			remove
			{
				_OnVerifySuccessEvent -= value;
			}
		}

		event OnQuit IUser.QuitEvent
		{
			add
			{
				_OnQuitEvent += value;
			}

			remove
			{
				_OnQuitEvent -= value;
			}
		}

		bool IUpdatable.Update()
		{
			_Machine.Update();
			return true;
		}

		void IBootable.Launch()
		{
			_Binder.Bind<IVersion>(this);
			_ToVerify();
		}

		void IBootable.Shutdown()
		{
			_Binder.Unbind<IVersion>(this);
			_Machine.Termination();
		}

		string IVersion.Number
		{
			get
			{
				return _Version;
			}
		}

		private void _ToVerify()
		{
			_Account = null;
			var verify = new Verify(_ExpansionFeature.AccountFinder);
			var stage = new Storage.Verify(_Binder, verify);
			stage.DoneEvent += _VerifySuccess;
			_Machine.Push(stage);
		}

		private void _VerifySuccess(Account account)
		{
			if(account.IsFormulaQueryer())
			{
				_Account = account;
				_OnVerifySuccessEvent(_Account.Guid);
				_ToFishStage();
			}
			else
			{
				_ToVerify();
			}
		}

		private void _ToFishStage()
		{
			var stage = new FormulaStage(_Binder, _ExpansionFeature);

			stage.OnDoneEvent += () => { _OnQuitEvent(); };
			_Machine.Push(stage);
		}
	}
}
