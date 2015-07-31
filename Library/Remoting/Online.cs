// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Online.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the IOnline type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;

#endregion

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
			this.Id = Guid.NewGuid();
		}

		public Online(IAgent agent) : this()
		{
			this._Agent = agent;
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

		double IOnline.Ping
		{
			get { return TimeSpan.FromTicks(this._Agent.Ping).TotalSeconds; }
		}

		void IOnline.Disconnect()
		{
			this._Agent.Disconnect();
		}
	}
}