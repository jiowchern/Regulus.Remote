using System;
using Regulus.Network.RUDP;
using Regulus.Utility;

namespace Regulus.Network.Tests.HostApp
{
	internal class RunStage : Regulus.Utility.IStage
	{
		private Command _Command;
		private Host _Host;		
		private Utility.Console.IViewer _Viewer;
		public event System.Action ExitEvent;
	    private long _Ticks;
	    readonly Regulus.Utility.Updater<Timestamp> _Updater;
		int _PeerId;

		public RunStage(Command command, Utility.Console.IViewer viewer, Host host)
		{
		
			_Updater = new Updater<Timestamp>();
			this._Command = command;
			this._Viewer = viewer;
			this._Host = host;
		}

		void IStage.Enter()
		{
		    _Ticks = System.DateTime.Now.Ticks;

            _Command.RegisterLambda(this , instance=>instance.Exit() );
			_Host.AcceptEvent += _JoinPeer;
		    _Updater.Add(_Host);

        }

		public void Exit()
		{
			ExitEvent();
		}


		private void _JoinPeer(IPeer peer)
		{
			
			_Updater.Add(new PeerHandler(++_PeerId,_Command , _Viewer,peer));
		}

		void IStage.Leave()
		{
			_Command.Unregister("Exit");
			_Host.AcceptEvent -= _JoinPeer;
			_Updater.Shutdown();
		}

		void IStage.Update()
		{
		    var ticks = System.DateTime.Now.Ticks;
		    var delta = ticks - _Ticks;
		    _Ticks = ticks;

            _Updater.Working(new Timestamp(ticks , delta));
		}
	}
}