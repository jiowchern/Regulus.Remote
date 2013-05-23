using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Project.TurnBasedRPG
{
    class Map : Samebest.Game.IFramework
    {
        List<Field> _Fields = new List<Field>();
        List<Entity> _Entitys = new List<Entity>();

        void Add(Entity entity)
        {
            if (entity.Vision > 0)
            {
                _Fields.Add(new Field(entity));
            }
            _Entitys.Add(entity);
        }

        void Remove(Entity entity)
        {
            _Entitys.Remove(entity);
            
        }
        void Samebest.Game.IFramework.Launch()
        {
            
        }

        bool Samebest.Game.IFramework.Update()
        {            

            // 更新每位entity可見視野
            Queue<Field> fields = new Queue<Field>(_Fields);
            while (fields.Count > 0)
            {                
                var field = fields.Dequeue();
                Entity[] entitys = (from f in _Fields where f.Entity != field.Entity select field.Entity).ToArray();
                field.Update(entitys);
            }            
            // 更新entity
            return true;
        }

        void Samebest.Game.IFramework.Shutdown()
        {
            
        }
    }
}
