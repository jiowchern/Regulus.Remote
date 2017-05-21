using System;


using Regulus.Remoting;

namespace RemotingTest
{
	internal class Ghost : IGhost
	{
		private readonly Guid _Id;

		public Ghost()
		{
			_Id = Guid.NewGuid();
		}

	    

	    Guid IGhost.GetID()
		{
			return _Id;
		}

	    public object GetInstance()
	    {
	        return this;
	    }

        private event CallMethodCallback _CallMethodEvent;

        event CallMethodCallback IGhost.CallMethodEvent
        {
            add { this._CallMethodEvent += value; }
            remove { this._CallMethodEvent -= value; }
        }

        bool IGhost.IsReturnType()
		{
			throw new NotImplementedException();
		}
	}
}
