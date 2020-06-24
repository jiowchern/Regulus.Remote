using System;
using System.Reflection;

namespace Regulus.Remote
{
    public partial class SoulProvider
    {
        public partial class Soul
		{
			internal Action<Type, object> LoadPropertyHandler;
			public readonly System.Collections.Generic.List<PropertyUpdater> PropertyUpdaters;

			public class EventHandler
			{
				public Delegate DelegateObject;

				public EventInfo EventInfo;
			}


			
			
		}
	}
}
