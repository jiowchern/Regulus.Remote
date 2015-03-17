using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Utility
{
    public class Random : Singleton<Random>
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

        public static T NextEnum<T>()
        {            
            return Enum
                .GetValues(typeof(T))
                .Cast<T>()
                .OrderBy(x => Instance.R.NextFloat())
                .FirstOrDefault();
        }
    }


    public interface IRandom
    {
        float NextFloat();

        float NextFloat(float min , float max);
        int NextInt(int min , int max);
        long NextLong(long min, long max);

        double NextDouble();
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
            byte[] buf = new byte[8];
            _Random.NextBytes(buf);
            long longRand = BitConverter.ToInt64(buf, 0);

            return (Math.Abs(longRand % (max - min)) + min);
        }


        double IRandom.NextDouble()
        {
            return _Random.NextDouble();
        }


        float IRandom.NextFloat(float min, float max)
        {
            return min + (float)_Random.NextDouble() * max;
        }
    }
}
