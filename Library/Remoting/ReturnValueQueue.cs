using System;
using System.Collections.Generic;

namespace Regulus.Remote
{
	public class ReturnValueQueue
	{
		private readonly Dictionary<Guid, IValue> _ReturnValues = new Dictionary<Guid, IValue>();

		internal IValue PopReturnValue(Guid returnTarget)
		{
			return _PopReturnValue(returnTarget);
		}

		public Guid PushReturnValue(IValue value)
		{
			var id = Guid.NewGuid();
			_ReturnValues.Add(id, value);
			return id;
		}

		private IValue _PopReturnValue(Guid returnTarget)
		{
			IValue val = null;
			if(_ReturnValues.TryGetValue(returnTarget, out val))
			{
				_ReturnValues.Remove(returnTarget);
			}

			return val;
		}
	}
}
