using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.KeysHearthstone
{
	class Verify : Regulus.Utility.IStage , IVerify
	{
		private Remoting.ISoulBinder _Binder;
		private IStorage _Storage;
		public delegate void OnDone(Data.Account account);
		public event OnDone DoneEvent;

		public Verify(Remoting.ISoulBinder binder, IStorage storage)
		{			
			this._Binder = binder;
			this._Storage = storage;
		}

		void Utility.IStage.Enter()
		{
			_Binder.Bind<IVerify>(this);
		}

		void Utility.IStage.Leave()
		{
			_Binder.Unbind<IVerify>(this);
		}

		void Utility.IStage.Update()
		{
				
		}

		Remoting.Value<bool> IVerify.Login(string name, string password)
		{
			Remoting.Value<bool> returnValue = new Remoting.Value<bool>(); ;
			Remoting.Value<Data.Account> val = _Storage.FindAccount(name);
			val.OnValue += (account) => 
			{
				returnValue.SetValue(_Verify(account, password));				
			};
			return returnValue;
		}

		private bool _Verify(Data.Account account, string password)
		{
			if (account.Password == password)
			{
				DoneEvent(account);
				return true;
			}
			return false;;
		}

		Remoting.Value<bool> IVerify.CreateAccount(string name, string password)
		{
			return _Storage.AddAccount(name, password);
		}
	}
}
