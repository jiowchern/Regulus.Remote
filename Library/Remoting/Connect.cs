// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Connect.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the IConnect type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;

#endregion

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
			this.Id = Guid.NewGuid();
		}

		Value<bool> IConnect.Connect(string ipaddr, int port)
		{
			if (this.ConnectedEvent == null)
			{
				throw new SystemException("Invalid Connect, to regain from the provider.");
			}

			var val = new Value<bool>();
			this.ConnectedEvent(ipaddr, port, val);
			return val;
		}

		void IGhost.OnEvent(string name_event, object[] args)
		{
			throw new NotImplementedException();
		}

		Guid IGhost.GetID()
		{
			return this.Id;
		}

		void IGhost.OnProperty(string name, byte[] value)
		{
			throw new NotImplementedException();
		}

		bool IGhost.IsReturnType()
		{
			return false;
		}
	}
}