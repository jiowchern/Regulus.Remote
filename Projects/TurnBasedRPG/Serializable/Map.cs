using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
	namespace Serialize
	{
		[Serializable]
		public class Map
		{
			public string Name { get; set; }
			public StaticEntity[] StaticEntitys{ get; set; }
			public TransmissionEntity[] TransmissionEntitys { get; set; }
		}

		[Serializable]
		public class StaticEntity
		{			
			public Regulus.Utility.OBB Obb { get; set; }
		}
		[Serializable]
		public class TransmissionEntity
		{			
			public Regulus.Utility.OBB Obb { get; set; }
			public string TargetMapName { get; set; }
			public Regulus.Types.Vector2 TargetPosition { get; set; }
		}
	}
	

}
