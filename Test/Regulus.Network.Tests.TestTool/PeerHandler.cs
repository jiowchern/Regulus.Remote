using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using Regulus.Framework;

using Regulus.Utility;

namespace Regulus.Network.Tests.TestTool
{
	internal class PeerHandler : IUpdatable
	{
	    private static int _PeerId;
		private IPeer _Peer;
		readonly int _Id;
	    readonly Command _Command;
		private readonly string _CommandSendString;
		private readonly Utility.Console.IViewer _Viewer;
	    private readonly Regulus.Utility.StageMachine _SendMachine;
	    private readonly Regulus.Utility.StageMachine _ReceiveMachine;
        private readonly string _CommandSendContinousNumber;
	    private readonly string _CommandSendMessage;
        private readonly string _CommandReceiveContinousNumber;
        private readonly string _CommandReceiveString;
	    private string _CommandView;

	    public  PeerHandler(Command command , Utility.Console.IViewer viwer , IPeer peer)
		{		    
            _SendMachine = new StageMachine();
		    _ReceiveMachine = new StageMachine();
            _Viewer = viwer;
			_Command = command;
			_Id = ++_PeerId;
			_Peer = peer;
			_CommandSendString = string.Format("ss{0}", _Id);
		    _CommandSendContinousNumber = string.Format("scn{0}", _Id);
		    _CommandReceiveString = string.Format("rs{0}", _Id);
		    _CommandReceiveContinousNumber = string.Format("rcn{0}", _Id);
		    _CommandView = string.Format("view{0}", _Id);
        }

       

        void IBootable.Launch()
		{
			_Viewer.WriteLine(string.Format("Accept Transmitter {0} {1}", _Peer.RemoteEndPoint ,_Id));
		    		    
		    _Command.Register(_CommandSendString, ToSendString);
            _Command.Register<int,int>(_CommandSendContinousNumber, ToSendContinousNumber);
		    _Command.Register(_CommandReceiveString, ToRecevieString);
		    _Command.Register(_CommandReceiveContinousNumber, ToReceoveContinuousNumber);
		    _Command.Register(_CommandView, View);

            ToSendString();
            ToRecevieString();
		}

	    private void View()
	    {
            
	        ThreadPool.QueueUserWorkItem(_RunProfile, _Peer.RemoteEndPoint.ToString());
        }

	    private void _RunProfile(object state)
	    {
	        var profile = new PeerProfile((string)state);
	        Application.Run(profile);
        }


	    void IBootable.Shutdown()
		{
			_Viewer.WriteLine(string.Format("Leave Transmitter {0} {1}", _Peer.RemoteEndPoint,_Id));
			
			_Command.Unregister(_CommandSendString);
		    _Command.Unregister(_CommandSendContinousNumber);
		    _Command.Unregister(_CommandReceiveString);
		    _Command.Unregister(_CommandReceiveContinousNumber);
		    _Command.Unregister(_CommandView);
            _SendMachine.Termination();
		    _ReceiveMachine.Termination();
        }

		

		bool IUpdatable.Update()
		{
		    _SendMachine.Update();
		    _ReceiveMachine.Update();
            return _Peer.Connected;
		}

	    public void ToSendString()
	    {
	        _SendMachine.Push(new SendStringStage(_Id, _Peer, _Viewer, _Command));
        }


        public void ToSendContinousNumber(int fps, int size)
	    {
	        var stage = new SendContinuousNumberStage(fps , size,_Peer);
            _SendMachine.Push(stage);
	    }


	    public void ToReceoveContinuousNumber()
	    {
	        var stage = new ReceiveContinuousNumberStage(_Id, _Peer , _Viewer);
	        _ReceiveMachine.Push(stage);
        }


        public void ToRecevieString()
	    {
	        var stage = new ReceiveStringStage(_Id , _Peer, _Viewer);
	        _ReceiveMachine.Push(stage);

        }
	    
	    
	    
	}
}