using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Utility
{
    public class Random : Singleton<Random>
	{
        private IRandom _R;
		public Random()
		{
            _R = new SystemRandom();
		}
		public new static IRandom Instance
        {
            get { return Singleton< Random >.Instance._R; }
        }                
    }


    public interface IRandom
    {
        float NextFloat();

        float NextFloat(float min , float max);
        int NextInt(int min , int max);
        long NextLong(long min, long max);

        double NextDouble();

        T NextEnum<T>();
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
            return _NextFloat();
        }

        private float _NextFloat()
        {
            var val = _Random.NextDouble();
            return (float)val;
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
            return _NextFloat(min, max);
        }

        private float _NextFloat(float min, float max)
        {
            return min + (float)_Random.NextDouble() * max;
        }


        T IRandom.NextEnum<T>()
        {
            return Enum
                .GetValues(typeof(T))
                .Cast<T>()
                .OrderBy(x => _NextFloat())
                .FirstOrDefault();
        }
    }
}
