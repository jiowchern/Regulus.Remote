using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Utility
{
	public class Random : Samebest.Utility.Singleton<Random>
	{
		public System.Random R {get;private set;}
		public Random()
		{
			R = new System.Random();
		}

		public void SetSeed(int seed)
		{
			R = new System.Random(seed);
		}

        public static int Next(int min , int max)
        {
            return Instance.R.Next(min, max);
        }
	}
}
