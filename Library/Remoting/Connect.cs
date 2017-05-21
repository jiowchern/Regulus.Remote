using System;

namespace Regulus.Remoting
{
	public interface IConnect
	{
		Value<bool> Connect(string ipaddr, int port);
	}

	public class Connect : IGhost, IConnect
	{
		public event Action<string, int, Value<bool>> ConnectedEvent;

		public Guid Id { get; private set; }

		public Connect()
		{
			Id = Guid.NewGuid();
		}

		Value<bool> IConnect.Connect(string ipaddr, int port)
		{
			if(ConnectedEvent == null)
			{
				throw new SystemException("Invalid Connect, to regain from the provider.");
			}

			var val = new Value<bool>();
			ConnectedEvent(ipaddr, port, val);
			return val;
		}
        private event CallMethodCallback _CallMethodEvent;

        event CallMethodCallback IGhost.CallMethodEvent
        {
            add { this._CallMethodEvent += value; }
            remove { this._CallMethodEvent -= value; }
        }

        Guid IGhost.GetID()
		{
			return Id;
		}

	    public object GetInstance()
	    {
	        return this;
	    }

	    bool IGhost.IsReturnType()
		{
			return false;
		}
	}
}
