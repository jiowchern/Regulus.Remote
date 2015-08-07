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
using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Common.GPI;


using StageLock = VGame.Project.FishHunter.Common.Data.StageLock;

#endregion

namespace VGame.Project.FishHunter.Play
{
    internal class User : Regulus.Game.IUser, IAccountStatus
	{
		private event Action _KickEvent;

		private event OnQuit _QuitEvent;

		private event OnNewUser _VerifySuccessEvent;

		private readonly IAccountFinder _AccountFinder;

		private readonly ISoulBinder _Binder;

		private readonly IFishStageQueryer _FishStageQueryer;

		private readonly StageMachine _Machine;

		private readonly IRecordHandler _RecordHandler;

		private readonly StageTicketInspector _StageTicketInspector;

		private readonly ITradeNotes _TradeAccount;

		private Account _Account;

		private PlayerRecord _PlayerRecord;

		public User(ISoulBinder binder, 
			IAccountFinder account_finder, 
			IFishStageQueryer queryer, 
			IRecordHandler record_handler, 
			ITradeNotes trade_account)
		{
			_Machine = new StageMachine();

			_Binder = binder;
			_AccountFinder = account_finder;
			_FishStageQueryer = queryer;
			var locks = new[]
			{
				new StageLock
				{
					KillCount = 200, 
					Stage = 3
				}
			};
			_StageTicketInspector = new StageTicketInspector(new StageGate(locks));

			_RecordHandler = record_handler;

			_TradeAccount = trade_account;
		}

		event Action IAccountStatus.KickEvent
		{
			add { _KickEvent += value; }
			remove { _KickEvent -= value; }
		}

		event OnNewUser Regulus.Game.IUser.VerifySuccessEvent
		{
			add { _VerifySuccessEvent += value; }
			remove { _VerifySuccessEvent -= value; }
		}

		event OnQuit Regulus.Game.IUser.QuitEvent
		{
			add { _QuitEvent += value; }
			remove { _QuitEvent -= value; }
		}

		void Regulus.Game.IUser.OnKick(Guid id)
		{
			if (_Account != null && _Account.Id == id)
			{
				if (_KickEvent != null)
				{
					_KickEvent();
				}

				_ToVerify();
			}
		}

		bool IUpdatable.Update()
		{
			_Machine.Update();
			return true;
		}

		void IBootable.Launch()
		{
			_Binder.BreakEvent += _Quit;
			_Binder.Bind<IAccountStatus>(this);
			_ToVerify();
		}

		void IBootable.Shutdown()
		{
			_SaveRecord();
			_Binder.Unbind<IAccountStatus>(this);
			_Machine.Termination();
			_Binder.BreakEvent -= _Quit;
		}

		private void _Quit()
		{
			_QuitEvent();
		}

		private void _SaveRecord()
		{
			if (_PlayerRecord != null)
			{
				_RecordHandler.Save(_PlayerRecord);
			}
		}

		private void _ToVerify()
		{
			var verify = _CreateVerify();
			_AddVerifyToStage(verify);
		}

		private Verify _CreateVerify()
		{
			_Account = null;
			var verify = new Verify(_AccountFinder);
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
			_ToQueryRecord();
		}

		private void _ToQueryRecord()
		{
			_RecordHandler.Load(_Account.Id).OnValue += obj =>
			{
				_PlayerRecord = obj;
				_StageTicketInspector.Initial(new[]
				{
					new Common.Data.Stage
					{
						Id = 1, 
						Pass = true
					}, 
					new Common.Data.Stage
					{
						Id = 2, 
						Pass = false
					}, 
					new Common.Data.Stage
					{
						Id = 4, 
						Pass = false
					}, 
					new Common.Data.Stage
					{
						Id = 5, 
						Pass = false
					}, 
					new Common.Data.Stage
					{
						Id = 6, 
						Pass = false
					}, 
					new Common.Data.Stage
					{
						Id = 7, 
						Pass = false
					}, 
					new Common.Data.Stage
					{
						Id = 8, 
						Pass = false
					}, 
					new Common.Data.Stage
					{
						Id = 9, 
						Pass = false
					}, 
					new Common.Data.Stage
					{
						Id = 10, 
						Pass = false
					}, 
					new Common.Data.Stage
					{
						Id = 11, 
						Pass = false
					}, 
					new Common.Data.Stage
					{
						Id = 12, 
						Pass = false
					}, 
					new Common.Data.Stage
					{
						Id = 13, 
						Pass = false
					}, 
					new Common.Data.Stage
					{
						Id = 14, 
						Pass = false
					}
				});
				_ToLoadTradeNotes();
			};
		}

		private void _ToLoadTradeNotes()
		{
			_TradeAccount.GetTotalMoney(_Account.Id).OnValue += money =>
			{
				_PlayerRecord.Money += money;
				_ToSelectStage();
			};
		}

		private void _ToSelectStage()
		{
			var stage = new SelectStage(_StageTicketInspector.PlayableStages, _Binder, _FishStageQueryer);
			stage.DoneEvent += _ToPlayStage;
			_Machine.Push(stage);
		}

		private void _ToPlayStage(IFishStage fish_stage)
		{
			var stage = new PlayStage(_Binder, fish_stage, _PlayerRecord);
			stage.PassEvent += _Pass;
			stage.KillEvent += _Kill;
			_Machine.Push(stage);
		}

		private void _Kill(int kill_count)
		{
			_StageTicketInspector.Kill(kill_count);
			_ToSelectStage();
		}

		private void _Pass(int pass_stage)
		{
			_StageTicketInspector.Pass(pass_stage);
			_ToSelectStage();
		}
	}
}