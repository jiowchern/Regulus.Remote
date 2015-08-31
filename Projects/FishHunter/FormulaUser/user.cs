﻿using System;


using Regulus.Framework;
using Regulus.Remoting;
using Regulus.Utility;


using VGame.Project.FishHunter.Common;
using VGame.Project.FishHunter.Common.GPI;

namespace VGame.Project.FishHunter.Formula
{
	internal class User : IUser
	{
		private readonly IAgent _Agent;

		private readonly Updater _Updater;

		private readonly Regulus.Remoting.User _User;

		public User(IAgent agent)
		{
			_Agent = agent;
			_Updater = new Updater();
			_User = new Regulus.Remoting.User(_Agent);
		}

		bool IUpdatable.Update()
		{
			_Updater.Working();
			return true;
		}

		void IBootable.Launch()
		{
		    _Agent.ErrorMethodEvent += _ErrorMethodEvent;
            _Updater.Add(_User);
		}

		void IBootable.Shutdown()
		{
            _Agent.ErrorMethodEvent -= _ErrorMethodEvent;
            _Updater.Shutdown();
		}

		Regulus.Remoting.User IUser.Remoting
		{
			get { return _User; }
		}

		INotifier<IVerify> IUser.VerifyProvider
		{
			get { return _Agent.QueryNotifier<IVerify>(); }
		}

		INotifier<IFishStageQueryer> IUser.FishStageQueryerProvider
		{
			get { return _Agent.QueryNotifier<IFishStageQueryer>(); }
		}

	    private event Action<string, string> _ErrorMethodEvent;

	    event Action<string, string> IUser.ErrorMethodEvent
	    {
	        add { this._ErrorMethodEvent += value; }
	        remove { this._ErrorMethodEvent -= value; }
	    }
	}
}
