using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
	class Actor : Entity
	{
		Regulus.Project.TurnBasedRPG.Serializable.DBEntityInfomation _Infomation ;
		public Actor(Serializable.DBEntityInfomation info )
			: base(info.Property)
		{
			_Infomation = info;
		}

		
	}
}
