using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
	namespace Data
	{
		[Serializable]
		public class Map
		{
			public string Name { get; set; }
			public Entity[] Entitys{ get; set; }			
		}
        [Serializable]
        public abstract class Entity
        {
            public Guid Id { get; set; }
        }

		[Serializable]
        public class StaticEntity : Entity
		{			
			public Regulus.Utility.OBB Obb { get; set; }
		}
		
	}
	

}
