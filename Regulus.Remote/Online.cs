using System;

namespace Regulus.Remote
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

		

		Guid IGhost.GetID()
		{
			return Id;
		}

	    object IGhost.GetInstance()
	    {
	        return this;
	    }

	    

		bool IGhost.IsReturnType()
		{
			return false;
		}

	    private event CallMethodCallback _CallMethodEvent;

	    event CallMethodCallback IGhost.CallMethodEvent
	    {
	        add { this._CallMethodEvent += value; }
	        remove { this._CallMethodEvent -= value; }
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
