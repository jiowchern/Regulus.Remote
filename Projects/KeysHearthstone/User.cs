using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.KeysHearthstone
{
    class User : Regulus.Game.IUser
	{
		private Remoting.ISoulBinder _Binder;
		Regulus.Utility.StageMachine _Machine;
		IStorage _Storage;
		public User(Remoting.ISoulBinder binder , IStorage storage)
		{
			_Machine = new Utility.StageMachine();
			this._Binder = binder;
			_Account = new Data.Account();
			_Storage = storage;
		}
		Data.Account _Account;
		void Game.IUser.OnKick(Guid id)
		{
			if (_Account.Id == id)
			{
				_QuitEvent();
			}
		}

        event Game.OnNewUser _VerifySuccessEvent;
        event Game.OnNewUser Game.IUser.VerifySuccessEvent
		{
			add { _VerifySuccessEvent += value; }
			remove { _VerifySuccessEvent -= value; }
		}
        event Game.OnQuit _QuitEvent;
        event Game.OnQuit Game.IUser.QuitEvent
		{
			add { _QuitEvent += value; }
			remove { _QuitEvent -= value; }
		}

		bool Utility.IUpdatable.Update()
		{
			_Machine.Update();
			return true;
		}

		void Framework.ILaunched.Launch()
		{
			_ToVerify();
		}

		private void _ToVerify()
		{
			var stage = new Verify(_Binder, _Storage);
			stage.DoneEvent += _ToMaintenance;            
            
			_Machine.Push(stage);
		}

		private void _ToMaintenance(Data.Account account)
		{
			_Account = account;


		}

		void Framework.ILaunched.Shutdown()
		{
			_Machine.Termination();
		}
	}
}
