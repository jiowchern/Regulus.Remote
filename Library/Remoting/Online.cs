using System;

namespace Regulus.Remoting
{
	public interface IOnline
	{
		double Ping { get; }

		void Disconnect();
	}

	public class Online : IOnline, IGhost
	{
		private readonly IAgent _Agent;

		public Guid Id { get; private set; }

		public Online()
		{
			Id = Guid.NewGuid();
		}

		public Online(IAgent agent) : this()
		{
			_Agent = agent;
		}

		void IGhost.OnEvent(string name_event, byte[][] args)
		{
			throw new NotImplementedException();
		}

		Guid IGhost.GetID()
		{
			return Id;
		}

		void IGhost.OnProperty(string name, byte[] value)
		{
			throw new NotImplementedException();
		}

		bool IGhost.IsReturnType()
		{
			return false;
		}

		double IOnline.Ping
		{
			get { return TimeSpan.FromTicks(_Agent.Ping).TotalSeconds; }
		}

		void IOnline.Disconnect()
		{
			_Agent.Disconnect();
		}
	}
}
