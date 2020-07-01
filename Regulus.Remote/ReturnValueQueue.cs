using System;
using System.Collections.Generic;

namespace Regulus.Remote
{
	public class ReturnValueQueue
	{
        private readonly IdLandlord _IdLandlord;
        private readonly Dictionary<long, IValue> _ReturnValues ;

		public ReturnValueQueue()
        {
			_IdLandlord = new IdLandlord();
			_ReturnValues = new Dictionary<long, IValue>();
		}

		internal IValue PopReturnValue(long returnTarget)
		{
			return _PopReturnValue(returnTarget);
		}

		public long PushReturnValue(IValue value)
		{
			var id = _IdLandlord.Rent();
			_ReturnValues.Add(id, value);
			return id;
		}

		private IValue _PopReturnValue(long return_target)
		{
			IValue val = null;
			if(_ReturnValues.TryGetValue(return_target, out val))
			{
				_ReturnValues.Remove(return_target);
				_IdLandlord.Return(return_target);
			}

			return val;
		}
	}
}
