using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Samebest.Utility
{
    public class Singleton<T> where T : class, new()
    {
        private static T _Instance;
        static Type _Sync = typeof(T);
        public static T GetInstance()
        {
            return Instance;
        }

		public static T Instance	
		{
			get 
			{
                lock (_Sync)
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