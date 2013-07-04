using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Remoting.Ghost
{
	public class ReturnValueQueue
	{	
		
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

		Dictionary<Guid, IValue> _ReturnValues = new Dictionary<Guid, IValue>();
		private IValue _PopReturnValue(Guid returnTarget)
		{
			IValue val = null;
			if (_ReturnValues.TryGetValue(returnTarget, out val))
			{
				_ReturnValues.Remove(returnTarget);
			}
			return val;
		}
	}
}
