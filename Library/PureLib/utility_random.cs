using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Utility
{
    public class Random : Regulus.Utility.Singleton<Random>
	{
        public IRandom R { get; private set; }
		public Random()
		{
            R = new SystemRandom();
		}
		
        public static float NextFloat(float min, float max)
        {
            var number = Instance.R.NextFloat();
            return (max - min) * number + min;
        }
        public static int Next(int min , int max)
        {
            return Instance.R.NextInt(min, max);
        }
	}


    public interface IRandom
    {
        float NextFloat();
        int NextInt(int min , int max);
        long NextLong(long min, long max);
    }

    public class SystemRandom : IRandom
    {
        System.Random _Random;

        public SystemRandom()
        {
            _Random = new System.Random();
        }
        float IRandom.NextFloat()
        {
            return (float)_Random.NextDouble();
        }

        int IRandom.NextInt(int min, int max)
        {
            return _Random.Next(min, max);
        }


        long IRandom.NextLong(long min, long max)
        {
            long result = _Random.Next((Int32)(min >> 32), (Int32)(max >> 32));
            result = (result << 32);
            result = result | (long)_Random.Next((Int32)min, (Int32)max);
            return result;
        }
    }
}
