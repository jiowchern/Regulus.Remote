// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueWaiter.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the ValueWaiter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System.Threading;

#endregion

namespace Regulus.Remoting
{
	internal class ValueWaiter<T>
	{
		private readonly Value<T> value;

		private volatile bool _HasValue;

		public T Value { get; private set; }

		public ValueWaiter(Value<T> value)
		{
			// TODO: Complete member initialization
			this.value = value;
			this._HasValue = false;
		}

		internal void Run(object obj)
		{
			var are = (AutoResetEvent)obj;
			this.value.OnValue += this._Getted;

			var count = 0;
			while (this._HasValue == false)
			{
				count++;
				if (count % 10 == 0)
				{
					Thread.Sleep(0);
				}
				else if (count % 20 == 0)
				{
					Thread.Sleep(1);
				}
				else
				{
					Thread.SpinWait(count);
				}

				if (count > 20 * 1000)
				{
					count = 0;
					Thread.Sleep(1000);
				}
			}

			are.Set();
		}

		private void _Getted(T obj)
		{
			this.Value = obj;
			this._HasValue = true;
		}
	}
}