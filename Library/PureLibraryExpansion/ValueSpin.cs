// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueSpin.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the ValueSpin type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Remoting;
using Regulus.Utility;

#endregion

namespace Regulus.Net45
{
	internal class ValueSpin<T>
	{
		private readonly Value<T> value;

		private volatile bool _HasValue;

		public T Value { get; private set; }

		public ValueSpin(Value<T> value)
		{
			this.value = value;
			this._HasValue = false;
		}

		internal T Wait()
		{
			this.value.OnValue += this._Getted;

			var sw = new SpinWait();
			while (this._HasValue == false)
			{
				sw.SpinOnce();
			}

			return this.Value;
		}

		internal void Run(object obj)
		{
			this.value.OnValue += this._Getted;

			var sw = new SpinWait();
			while (this._HasValue == false)
			{
				sw.SpinOnce();
			}
		}

		private void _Getted(T obj)
		{
			this.Value = obj;
			this._HasValue = true;
		}
	}
}