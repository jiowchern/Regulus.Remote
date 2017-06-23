using System;
using Regulus.Framework;
using Regulus.Utility;

namespace Regulus.Network.Tests.HostApp
{
	internal class InitialStage : Regulus.Utility.IStage
	{
		private Command _Command;

		public event Action<Regulus.Network.RUDP.Host> CreatedEvent;

		public InitialStage(Command command)
		{
			this._Command = command;
		}

		

		public void Bind(int port)
		{			
			CreatedEvent(Regulus.Network.RUDP.Host.CreateStandard(port));
		}

		void IStage.Enter()
		{
			_Command.RegisterLambda<InitialStage, int>(this, (obj, port) => obj.Bind(port));
		}

		void IStage.Leave()
		{
			_Command.Unregister("Bind");
		}

		void IStage.Update()
		{

		}
	}
}