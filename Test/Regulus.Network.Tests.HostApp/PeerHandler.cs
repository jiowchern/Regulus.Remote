using System;
using System.Net.Sockets;
using Regulus.Framework;
using Regulus.Network.RUDP;
using Regulus.Utility;

namespace Regulus.Network.Tests.HostApp
{
	internal class PeerHandler : IUpdatable<Timestamp>
	{
		private ISocket _Peer;
		readonly int _Id;
		Command _Command;
		private readonly string _CommandSendString;
		private readonly Utility.Console.IViewer _Viewer;
	    private byte[] _Buffer;
	    private readonly Regulus.Utility.StageMachine _Machine;
	    private TimeCounter _Counter;
	    private float _TimeToSend;
	    private int _PackageSize;
	    private readonly string _CommandStartString;
	    private readonly string _CommandStopString;

	    public  PeerHandler(int id ,Command command , Utility.Console.IViewer viwer , ISocket peer)
		{
		    _Counter = new TimeCounter();
            _Machine = new StageMachine();
            _Viewer = viwer;
			_Command = command;
			_Id = id;
			_Peer = peer;
			_CommandSendString = string.Format("send{0}", _Id);
		    _CommandStartString = string.Format("start{0}", _Id);
		    _CommandStopString = string.Format("stop{0}", _Id);
            _Buffer = new byte[Config.PackageSize];



        }

		void IBootable.Launch()
		{
			_Viewer.WriteLine(string.Format("Accept Transmitter {0} {1}", _Peer.RemoteEndPoint ,_Id));
		    
		    _Peer.Receive(_Buffer, 0, _Buffer.Length, _Readed);
            _Command.Register<string>(_CommandSendString, Send);
		    _Command.Register<int,int>(_CommandStartString, StartSimulation);
		    _Command.Register(_CommandStopString, StopSimulation);
        }

	    private void _Readed(int read_count, SocketError error)
	    {
	        var message = Convert.ToBase64String(_Buffer , 0 , read_count);
	        _Viewer.WriteLine(String.Format("transmitter{0} : {1}", _Id, message));


	        for (int i = 0; i < _Buffer.Length; i++)
	        {
	            _Buffer[i] = 0;
	        }

            _Peer.Receive(_Buffer, 0, _Buffer.Length, _Readed);
        }


	    public void Send(string message)
	    {
	        var buffer = _ToBytes(message);

            _Peer.Send(buffer , 0 , buffer.Length , (send_count , error) => { });
		}
		public byte[] _ToBytes(string message)
		{
			return System.Text.Encoding.Default.GetBytes(message);
		}

		void IBootable.Shutdown()
		{
			_Viewer.WriteLine(string.Format("Leave Transmitter {0} {1}", _Peer.RemoteEndPoint,_Id));
			
			_Command.Unregister(_CommandSendString);
		    _Command.Unregister(_CommandStartString);
		    _Command.Unregister(_CommandStopString);
            _Machine.Termination();

        }

		

		bool IUpdatable<Timestamp>.Update(Timestamp time)
		{
		    _Machine.Update();

            return _Peer.Connected;
		}


	    public void StartSimulation(int fps, int size)
	    {
	        _Machine.Push(new SimpleStage(()=>{} , ()=>{} , _UpdateSendPackage));
	        _TimeToSend = 1f / fps ;
	        _PackageSize = size;
	    }

	    public void StopSimulation()
	    {
	        _Machine.Empty();

        }

	    void _UpdateSendPackage()
	    {
	        if (_Counter.Second > _TimeToSend)
	        {
	            _Counter.Reset();
	            _Peer.Send(_CreateData(_PackageSize) , 0 , _PackageSize, _SendResult);
            }

        }

	    private void _SendResult(int arg1, SocketError arg2)
	    {
	        _Viewer.WriteLine("send size:" + arg1 + " error:" + arg2);
	    }

	    private byte[] _CreateData(int package_size)
	    {
	        var buffer = new byte[package_size];
	        for (int i = 0; i < package_size; i++)
	        {
	            buffer[i] = (byte)i;
	        }

	        return buffer;
	    }
	}
}