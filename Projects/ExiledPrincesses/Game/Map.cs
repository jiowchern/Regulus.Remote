using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Game
{
    
    class Map : IMap
    {
        List<IEntity> _Entitys;
        
        public Map()
        {
            
            _Entitys = new List<IEntity>();
        }
        void IMap.Enter(IEntity entity)
        {
            _Entitys.Add(entity);
        }

        void IMap.Leave(IEntity entity)
        {
            _Entitys.Remove(entity);
        }




        
    }
}
