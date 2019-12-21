using System;

namespace Regulus.Utility
{
	public class Singleton<T> where T : new()
	{
		private static T _Instance;

		private static readonly Type _Sync = typeof(T);

	    public static System.Func<T> InstanceCreateHandler = _New;

	    private static T _New()
	    {
	        return new T();
	    }


	    public static T Instance
		{
			get
			{
				lock(Singleton<T>._Sync)
				{
					if(Singleton<T>._Instance == null)
					{
						Singleton<T>._Instance = InstanceCreateHandler();
					}

					return Singleton<T>._Instance;
				}
			}
		}
	}
}
