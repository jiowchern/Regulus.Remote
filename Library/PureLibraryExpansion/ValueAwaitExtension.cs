// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueAwaitExtension.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the ValueAwaitExtension type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System.Threading.Tasks;

using Regulus.Remoting;

#endregion

namespace Regulus.Net45
{
	public static class ValueAwaitExtension
	{
		public static Task<T> ToTask<T>(this Value<T> value)
		{
			var t = new Task<T>(() => { return new ValueSpin<T>(value).Wait(); });
			t.Start();
			return t;
		}

		public static T WaitResult<T>(this Value<T> value)
		{
			var t = value.ToTask();
			t.Wait();
			return t.Result;
		}
	}
}