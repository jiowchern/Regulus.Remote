using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Project.TurnBasedRPG
{
    class Map : Samebest.Game.IFramework
    {
        List<Field> _Fields = new List<Field>();
        
        void Add(Entity entity)
        {
            if (entity.Vision > 0)
            {
                _Fields.Add(new Field(entity));
            }        
        }

        void Remove(Entity entity)
        {
            _Fields.RemoveAll(field => field.Entity == entity);
        }
        void Samebest.Game.IFramework.Launch()
        {
            
        }

        bool Samebest.Game.IFramework.Update()
        {            

            // 更新每位entity可見視野
            _UpdateField();            

            return true;
        }

        private void _UpdateField()
        {
            Queue<Field> fields = new Queue<Field>(_Fields);
            while (fields.Count > 0)
            {
                var field = fields.Dequeue();
                Entity[] entitys = (from f in _Fields where f.Entity != field.Entity select field.Entity).ToArray();
                field.Update(entitys);
            }
        }

        void Samebest.Game.IFramework.Shutdown()
        {
            
        }
    }
}
