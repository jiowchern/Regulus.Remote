using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
    class Entity
    {
        public Regulus.Types.Vector2    Position    {get;private set;}
        
        internal static void LeftField(Entity entity)
        {
            throw new NotImplementedException();
        }

        internal static void IntoField(Entity entity)
        {
            throw new NotImplementedException();
        }

        public int Vision { get; private set; }
    }
}
