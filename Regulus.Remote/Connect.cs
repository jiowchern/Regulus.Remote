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

		public long Id { get; private set; }

		public ConnectGhost()
		{			
			Id = LongProvider.ConnectId;
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

        event EventNotifyCallback IGhost.AddEventEvent
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event EventNotifyCallback IGhost.RemoveEventEvent
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        long IGhost.GetID()
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
