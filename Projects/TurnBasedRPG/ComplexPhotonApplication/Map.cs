﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Project.TurnBasedRPG
{
    class Map : Samebest.Game.IFramework
    {
        class EntityInfomation
        {
            public Entity Entity {get;set;}
            public Action Exit; 
        }
        Regulus.Utility.Poller<EntityInfomation> _EntityInfomations = new Utility.Poller<EntityInfomation>();
        public void Into(Entity entity, Action exit_map)
        {
            _EntityInfomations.Add(new EntityInfomation() { Entity = entity, Exit = exit_map });
        }

        public void Left(Entity entity)
        {
            _EntityInfomations.Remove(info => info.Entity == entity);
        }
        void Samebest.Game.IFramework.Launch()
        {
            
        }

        bool Samebest.Game.IFramework.Update()
        {
            var infos = _EntityInfomations.Update();
            var entitys = (from info in infos select info.Entity).ToArray();
            foreach(var inf in infos)
            {
                var ent = inf.Entity;
                ent.Field.Update(entitys);                
            }
            return true;
        }

        

        void Samebest.Game.IFramework.Shutdown()
        {
            
        }
    }
}