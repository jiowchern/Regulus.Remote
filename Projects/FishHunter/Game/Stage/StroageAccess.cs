// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StroageAccess.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the StroageAccess type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.Linq;

using Regulus.Remoting;
using Regulus.Utility;

using VGame.Project.FishHunter.Common;

#endregion

namespace VGame.Project.FishHunter.Stage
{
	public class StroageAccess : IStage, IQuitable, IStorageCompetences
	{
		public event DoneCallback DoneEvent;

		private readonly Account _Account;

		private readonly ISoulBinder _Binder;

		private readonly IStorage _Storage;

		public StroageAccess(ISoulBinder binder, Account account, IStorage storage)
		{
			this._Binder = binder;
			this._Account = account;
			this._Storage = storage;
		}

		void IQuitable.Quit()
		{
			DoneEvent();
		}

		void IStage.Enter()
		{
			_Attach(_Account);
		}

		void IStage.Leave()
		{
			_Detach(_Account);
		}

		void IStage.Update()
		{
		}

		Value<Account.COMPETENCE[]> IStorageCompetences.Query()
		{
			return _Account.Competnces.ToArray();
		}

		Value<Guid> IStorageCompetences.QueryForId()
		{
			return _Account.Id;
		}

		public delegate void DoneCallback();

		private void _Attach(Account account)
		{
			_Binder.Bind<ITradeNotes>(_Storage);
			_Binder.Bind<IStorageCompetences>(this);

			if (account.HasCompetnce(Account.COMPETENCE.ACCOUNT_FINDER))
			{
				_Binder.Bind<IAccountFinder>(_Storage);
				_Binder.Bind<IRecordQueriers>(_Storage);
			}

			if (account.HasCompetnce(Account.COMPETENCE.ACCOUNT_MANAGER))
			{
				_Binder.Bind<IAccountManager>(_Storage);
			}

			_Binder.Bind<ITradeNotes>(_Storage);
		}

		private void _Detach(Account account)
		{
			if (account.HasCompetnce(Account.COMPETENCE.ACCOUNT_FINDER))
			{
				_Binder.Unbind<IAccountFinder>(_Storage);
				_Binder.Unbind<IRecordQueriers>(_Storage);
			}

			if (account.HasCompetnce(Account.COMPETENCE.ACCOUNT_MANAGER))
			{
				_Binder.Unbind<IAccountManager>(_Storage);
			}

			_Binder.Unbind<ITradeNotes>(_Storage);

			_Binder.Unbind<IStorageCompetences>(this);

			_Binder.Unbind<ITradeNotes>(_Storage);
		}
	}
}