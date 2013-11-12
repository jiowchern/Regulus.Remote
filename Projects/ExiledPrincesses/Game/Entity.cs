using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses
{
    class Entity : IEntity
    {
        Guid _Id;

        public Entity(Guid id)
        {
            _Id = id;
        }

        T IEntity.QueryAttrib<T>()
        {
            return default(T);
        }
        
        public Guid Id
        {
            get { return _Id; }
        }
    }
}
