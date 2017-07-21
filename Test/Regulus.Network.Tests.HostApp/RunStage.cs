using System;
using System.Text.RegularExpressions;
using Regulus.Network.RUDP;
using Regulus.Utility;

namespace Regulus.Network.Tests.HostApp
{
	internal class RunStage : Regulus.Utility.IStage
	{
		private Command _Command;
		private Regulus.Network.ISocketLintenable _Host;		
		private Utility.Console.IViewer _Viewer;
		public event System.Action ExitEvent;
	    private long _Ticks;
	    readonly Regulus.Utility.Updater<Timestamp> _Updater;
		int _PeerId;
	    private Regex _Regex
            ;

	    public RunStage(Command command, Utility.Console.IViewer viewer, Regulus.Network.ISocketLintenable host)
		{
		    _Regex = new Regex(
		        @"\[RUDP\]\sEndPoint:([\d]+.[\d]+.[\d]+.[\d]+:[\d]+)\sSendBytes:([\d]+)\sReceiveBytes:([\d]+)\sRTT:([\d]+)\sRTO:([\d]+)\sSendPackages:([\d]+)\sSendLost:([\d]+)\sReceivePackages:([\d]+)\sReceiveInvalidPackages:([\d]+)");
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

		    Regulus.Utility.Log.Instance.RecordEvent += _Record;


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
			_Host.AcceptEvent -= _JoinPeer;
			_Updater.Shutdown();
		}

	    private void _Record(string message)
	    {
	        var match = _Regex.Match(message);
	        if (match.Success)
	        {
	            var endPoint = match.Groups[0];
	            var sendBytes = match.Groups[1];
	            var receiveBytes = match.Groups[2];
	            var rtt = match.Groups[3];
	            var rto = match.Groups[4];
	            var sendPackages = match.Groups[5];
	            var sendLosts = match.Groups[6];
	            var receivePackages = match.Groups[7];
	            var receiveInvalids = match.Groups[8];
            }
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