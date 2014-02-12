using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
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

        [Serializable]
        public class PortalEntity : Entity
		{
            public PortalEntity()
            {
                Vision = new Types.Rect();
                TargetPosition = new Types.Vector2();
            }
            public Types.Rect Vision { get; set; }

            public string TargetMap { get; set; }

            public Types.Vector2 TargetPosition { get; set; }
        }

        
		
	}
	

}
