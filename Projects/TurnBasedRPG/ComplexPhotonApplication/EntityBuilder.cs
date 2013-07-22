using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
	
	static class EntityBuilder
	{
        static StaticEntity Build(Data.StaticEntity data)
		{
            return new StaticEntity(data.Obb, data.Id);
		}
	}
	
	
	
}
