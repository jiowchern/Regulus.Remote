using System.Reflection;
using Regulus.Utility;

namespace Regulus.Remote
{
    public partial class SoulProvider
    {
        public partial class Soul
		{
            public class PropertyHandler
			{
				private readonly PropertyInfo _PropertyInfo;

				private object _Value;

			    public readonly int Id;

			    public PropertyHandler(PropertyInfo info, int id)
			    {
			        _PropertyInfo = info;
			        Id = id;
			    }

                private bool _UpdateProperty(object val)
				{
					if(!ValueHelper.DeepEqual(_Value, val))
					{
						_Value = ValueHelper.DeepCopy(val);
						return true;
					}

					return false;
				}

				public bool TryUpdate(object instance , out object value)
				{
					value = _PropertyInfo.GetValue(instance, null);
					return _UpdateProperty(value);
					
				}
			}
		}
	}
}
