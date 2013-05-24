using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
	class Actor : Entity, Common.IActor
	{
		Regulus.Project.TurnBasedRPG.Serializable.DBEntityInfomation _Infomation ;
		public Actor(Serializable.DBEntityInfomation info )
			: base(info.Property)
		{
			_Infomation = info;
		}

		Samebest.Remoting.Value<Serializable.EntityPropertyInfomation> Common.IActor.GetProperty()
		{
			return _Infomation.Property;
		}

		Samebest.Remoting.Value<Serializable.EntityLookInfomation> Common.IActor.GetLook()
		{
			return _Infomation.Look;
		}
	}
}
