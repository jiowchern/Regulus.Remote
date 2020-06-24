using System;

namespace Regulus.Remote
{
	public interface IConnect
	{
		Value<bool> Connect(System.Net.IPEndPoint ip);
	}

	public class ConnectGhost : IGhost, IConnect
	{
		public event Action<System.Net.IPEndPoint, Value<bool>> ConnectedEvent;

		public Guid Id { get; private set; }

		public ConnectGhost()
		{
			Id = Guid.NewGuid();
		}

		Value<bool> IConnect.Connect(System.Net.IPEndPoint  ip)
		{
			if(ConnectedEvent == null)
			{
				throw new SystemException("Invalid Connect, to regain from the provider.");
			}

			var val = new Value<bool>();
			ConnectedEvent(ip, val);
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
