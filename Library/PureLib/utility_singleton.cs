using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Samebest.Utility
{
    public class Singleton<T> where T : class, new()
    {
        private static T _Instance;

        public static T GetInstance()
        {
            return Instance;
        }

		public static T Instance	
		{
			get 
			{
				lock (typeof(T))
				{
					if (_Instance == null)
					{
						_Instance = new T();
					}
					return _Instance;
				}
			}
		}
    }
}