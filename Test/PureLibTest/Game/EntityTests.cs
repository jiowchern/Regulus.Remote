using NUnit.Framework;
using Regulus.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regulus.Game.Tests
{

    public class TestComponment : Regulus.Game.IComponment
    {
        public Entity Entity;
       
        void IComponment.Start(Entity entity)
        {
            Entity = entity;
        }

        void IComponment.End()
        {
            
        }

        void IComponment.Update()
        {
            
        }
    }
    [TestFixture()]
    public class EntityTests
    {
        [Test()]
        public void AddTest()
        {
            var entity = new Regulus.Game.Entity();
            entity.Add(new TestComponment());
            entity.Update();
            var test = entity.Get<TestComponment>().First();
            Assert.AreEqual( entity, test.Entity);
        }
    }
}