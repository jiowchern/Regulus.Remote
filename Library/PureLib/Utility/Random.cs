// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Random.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Random type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.Linq;

#endregion

namespace Regulus.Utility
{
	public class Random : Singleton<Random>
	{
		private readonly IRandom _R;

		public new static IRandom Instance
		{
			get { return Singleton<Random>.Instance._R; }
		}

		public Random()
		{
			this._R = new SystemRandom();
		}
	}


	public interface IRandom
	{
		float NextFloat();

		float NextFloat(float min, float max);

		int NextInt(int min, int max);

		long NextLong(long min, long max);

		double NextDouble();

		T NextEnum<T>();
	}

	public class SystemRandom : IRandom
	{
		private readonly System.Random _Random;

		public SystemRandom()
		{
			this._Random = new System.Random();
		}

		float IRandom.NextFloat()
		{
			return this._NextFloat();
		}

		int IRandom.NextInt(int min, int max)
		{
			return this._Random.Next(min, max);
		}

		long IRandom.NextLong(long min, long max)
		{
			var buf = new byte[8];
			this._Random.NextBytes(buf);
			var longRand = BitConverter.ToInt64(buf, 0);

			return Math.Abs(longRand % (max - min)) + min;
		}

		double IRandom.NextDouble()
		{
			return this._Random.NextDouble();
		}

		float IRandom.NextFloat(float min, float max)
		{
			return this._NextFloat(min, max);
		}

		T IRandom.NextEnum<T>()
		{
			return Enum
				.GetValues(typeof (T))
				.Cast<T>()
				.OrderBy(x => this._NextFloat())
				.FirstOrDefault();
		}

		private float _NextFloat()
		{
			var val = this._Random.NextDouble();
			return (float)val;
		}

		private float _NextFloat(float min, float max)
		{
			return min + (float)this._Random.NextDouble() * max;
		}
	}
}