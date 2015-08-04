// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReturnValueQueue.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the ReturnValueQueue type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.Collections.Generic;

#endregion

namespace Regulus.Remoting
{
	public class ReturnValueQueue
	{
		private readonly Dictionary<Guid, IValue> _ReturnValues = new Dictionary<Guid, IValue>();

		internal IValue PopReturnValue(Guid returnTarget)
		{
			return this._PopReturnValue(returnTarget);
		}

		public Guid PushReturnValue(IValue value)
		{
			var id = Guid.NewGuid();
			this._ReturnValues.Add(id, value);
			return id;
		}

		private IValue _PopReturnValue(Guid returnTarget)
		{
			IValue val = null;
			if (this._ReturnValues.TryGetValue(returnTarget, out val))
			{
				this._ReturnValues.Remove(returnTarget);
			}

			return val;
		}
	}
}