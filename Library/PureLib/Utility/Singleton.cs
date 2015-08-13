using System;

namespace Regulus.Utility
{
	public class Singleton<T> where T : class, new()
	{
		private static T _Instance;

		private static readonly Type _Sync = typeof(T);

		public static T Instance
		{
			get
			{
				lock(Singleton<T>._Sync)
				{
					if(Singleton<T>._Instance == null)
					{
						Singleton<T>._Instance = new T();
					}

					return Singleton<T>._Instance;
				}
			}
		}
	}
}
