using System;


using Regulus.Framework;
using Regulus.Utility;

namespace Regulus.Remote
{
	public class User : IUpdatable
	{
		private readonly IAgent _Agent;

		private readonly TProvider<IConnect> _ConnectProvider;

		private readonly StatusMachine _Machine;

		private readonly TProvider<IOnline> _OnlineProvider;

		private readonly Updater _Updater;

	    public event Action<string> ErrorMessageEvent;

        public INotifier<IConnect> ConnectProvider
		{
			get { return _ConnectProvider; }
		}

		public INotifier<IOnline> OnlineProvider
		{
			get { return _OnlineProvider; }
		}

		public User(IAgent agent)
		{
			_Agent = agent;
			_ConnectProvider = new TProvider<IConnect>();
			_OnlineProvider = new TProvider<IOnline>();
			_Machine = new StatusMachine();
			_Updater = new Updater();
		}

		bool IUpdatable.Update()
		{
			_Updater.Working();
			_Machine.Update();
			return true;
		}

		void IBootable.Launch()
		{
		    _Agent.ErrorMethodEvent += _ErrorMethod;
            _Updater.Add(_Agent);
			_ToOffline();
		}

	    private void _ErrorMethod(string arg1, string arg2)
	    {
	        if(ErrorMessageEvent != null)
	        {
	            ErrorMessageEvent(string.Format("Error Method call. mrthod[{0}] Message[{1}]" , arg1 ,arg2));
	        }
        }

	    void IBootable.Shutdown()
		{
            
            _Machine.Termination();
			_Updater.Shutdown();
            _Agent.ErrorMethodEvent -= _ErrorMethod;
        }

		private void _ToOffline()
		{
			var stage = new OfflineStage(_Agent, _ConnectProvider);

			stage.DoneEvent += _ToOnline;

			_Machine.Push(stage);
		}

		private void _ToOnline()
		{
			var stage = new OnlineStage(_Agent, _OnlineProvider);

			stage.BreakEvent += _ToOffline;

			_Machine.Push(stage);
		}
	}
}
