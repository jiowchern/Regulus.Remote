using System;


using Regulus.Remote;

namespace RemotingTest
{
	internal class Ghost : IGhost
	{
		private readonly long _Id;

		public Ghost()
		{
			_Id = 1;
		}

	    

	    long IGhost.GetID()
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
