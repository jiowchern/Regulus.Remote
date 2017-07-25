using System;
using Regulus.Framework;
using Regulus.Utility;

namespace Regulus.Network.Tests.HostApp
{
	internal class InitialStage : Regulus.Utility.IStage
	{
		private readonly Command _Command;

		public event Action<Regulus.Network.ISocketLintenable> CreatedEvent;

		public InitialStage(Command command)
		{
			this._Command = command;
		}

		

		public void Bind(int port)
		{
		    Regulus.Network.ISocketLintenable listener = new RudpListener(new Regulus.Network.Win32.Time());
		    listener.Bind(port);
            CreatedEvent(listener);
		}

		void IStage.Enter()
		{
			_Command.RegisterLambda<InitialStage, int>(this, (obj, port) => obj.Bind(port));
		    _Command.RegisterLambda<InitialStage>(this, (obj) => obj.Run());
        }

	    private void Run()
	    {
	        Bind(12345);
	    }

	    void IStage.Leave()
		{
			_Command.Unregister("Bind");
		    _Command.Unregister("Run");
        }

		void IStage.Update()
		{

		}
	}
}