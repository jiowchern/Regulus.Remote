using System.Reflection;
using Regulus.Utility;

namespace Regulus.Remote
{
    public partial class SoulProvider
    {
        private partial class Soul
		{
            public class PropertyHandler
			{
				public readonly PropertyInfo PropertyInfo;

				public object Value;

			    public readonly int Id;

			    public PropertyHandler(PropertyInfo info, int id)
			    {
			        PropertyInfo = info;
			        Id = id;
			    }

                internal bool UpdateProperty(object val)
				{
					if(!ValueHelper.DeepEqual(Value, val))
					{
						Value = ValueHelper.DeepCopy(val);
						return true;
					}

					return false;
				}
			}
		}
	}
}
