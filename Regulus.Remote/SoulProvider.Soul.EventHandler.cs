using System;
using System.Reflection;

namespace Regulus.Remote
{
    public partial class SoulProvider
    {
        public partial class Soul
		{
			internal Action<Type, object> LoadPropertyHandler;

			public class EventHandler
			{
				public Delegate DelegateObject;

				public EventInfo EventInfo;
			}


			
			
		}
	}
}
