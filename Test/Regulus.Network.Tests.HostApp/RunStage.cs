using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Regulus.Network.RUDP;
using Regulus.Utility;

namespace Regulus.Network.Tests.HostApp
{
	internal class RunStage : Regulus.Utility.IStage
	{
		private Command _Command;
		private Regulus.Network.ISocketServer _Host;		
		private Utility.Console.IViewer _Viewer;
		public event System.Action ExitEvent;
	    private long _Ticks;
	    readonly Regulus.Utility.Updater<Timestamp> _Updater;
		int _PeerId;	    

        public RunStage(Command command, Utility.Console.IViewer viewer, Regulus.Network.ISocketServer host)
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
		    _Command.RegisterLambda<RunStage,string>(this, (instance , ip) => instance.Watch(ip));
            _Host.AcceptEvent += _JoinPeer;

		    


		}

		public void Exit()
		{
			ExitEvent();
		}


		private void _JoinPeer(ISocket peer)
		{
			
			_Updater.Add(new PeerHandler(++_PeerId,_Command , _Viewer,peer));
		}

		void IStage.Leave()
		{
		    Regulus.Utility.Log.Instance.RecordEvent -= _Record;
            _Command.Unregister("Exit");
		    _Command.Unregister("Watch");
            _Host.AcceptEvent -= _JoinPeer;
			_Updater.Shutdown();
		}

	    private void _Record(string message)
	    {
	        
        }

	    void IStage.Update()
		{
		    var ticks = System.DateTime.Now.Ticks;
		    var delta = ticks - _Ticks;
		    _Ticks = ticks;

            _Updater.Working(new Timestamp(ticks , delta));
		}

	    public void Watch(string ip)
	    {
	        
	        ThreadPool.QueueUserWorkItem(_RunWatch, ip);
	        
	    }

	    private void _RunWatch(object state)
	    {
	        string ip = (string) state;
	        var form = new PeerProfile(ip);
	        Application.Run(form);
        }	    
    }
}