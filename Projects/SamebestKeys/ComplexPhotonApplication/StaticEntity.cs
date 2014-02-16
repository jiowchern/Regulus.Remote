using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    class StaticEntity : Entity, IMoverAbility
    {
        Regulus.Utility.OBB _Obb;
        PhysicalAbility _QuadTreeObjectAbility;

        public StaticEntity(Regulus.Utility.OBB obb, Guid id)
            : base(id)
        {
            _Obb = obb;


            var a = (float)((_Obb.getRotation() - 180) * Math.PI / 180);
            var s = (float)Math.Sin(a);
            var c = (float)Math.Cos(a);
            
            s = s < 0 ? -s : s;
            c = c < 0 ? -c : c;

            var w = _Obb.getHeight() * s + _Obb.getWidth() * c;
            var h = _Obb.getHeight() * c + _Obb.getWidth() * s;
            var x = _Obb.getX() - w / 2;
            var y = _Obb.getY() - h / 2;

            _QuadTreeObjectAbility = new PhysicalAbility(new Regulus.Types.Rect(x,y,w,h) , this);
            
        }
        
        protected override void _SetAbility(Entity.AbilitySet abilitys)
        {
            abilitys.AttechAbility<PhysicalAbility>(_QuadTreeObjectAbility);
            abilitys.AttechAbility<IMoverAbility>(this);
        }

        protected override void _RiseAbility(Entity.AbilitySet abilitys)
        {
            abilitys.DetechAbility<PhysicalAbility>();
            abilitys.DetechAbility<IMoverAbility>();
        }

        Utility.OBB IMoverAbility.Obb
        {
            get { return _Obb; }
        }

        void IMoverAbility.Act(ActionStatue action_statue, float move_speed, float direction)
        {
            
        }

        void IMoverAbility.Update(long p, IEnumerable<Utility.OBB> obbs)
        {
            
        }
    }
}
