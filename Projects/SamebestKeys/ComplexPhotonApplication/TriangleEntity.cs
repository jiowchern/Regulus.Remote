using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    class TriangleEntity : Entity, IMoverAbility2
    {
        Types.Polygon _Polygon;
        PhysicalAbility _QuadTreeObjectAbility;

        public TriangleEntity(Guid id,Types.Polygon polygon) 
            : base(id)
        {
            _Polygon = polygon;

            var points = new Queue<Types.Vector2>(_Polygon.Points);
            var firstPoint = points.Dequeue();
            float top = firstPoint.Y;
            float down = firstPoint.Y ;
            float left = firstPoint.X;
            float right = firstPoint.X;
            while (points.Count > 0)
            {
                var point = points.Dequeue();
                if (point.X > right)
                {
                    right = point.X;
                }
                if (point.X < left)
                {
                    left = point.X;
                }
                if (point.Y < top)
                {
                    top = point.Y;
                }
                if (point.Y > down)
                {
                    down = point.Y;
                }
                
            }
            _QuadTreeObjectAbility = new PhysicalAbility(new Regulus.Types.Rect(left , top , right - left , down - top ) , this);
            _Polygon.BuildEdges();
        }
        Types.Polygon IMoverAbility2.Polygon
        {
            get { return _Polygon; }
        }

        void IMoverAbility2.Act(ActionStatue action_statue, float move_speed, float direction)
        {
            
        }

        void IMoverAbility2.Update(long time, IEnumerable<Types.Polygon> obbs)
        {
            
        }

        protected override void _SetAbility(Entity.AbilitySet abilitys)
        {
            abilitys.AttechAbility<PhysicalAbility>(_QuadTreeObjectAbility);
            abilitys.AttechAbility<IMoverAbility2>(this);
        }

        protected override void _RiseAbility(Entity.AbilitySet abilitys)
        {
            abilitys.DetechAbility<PhysicalAbility>();
            abilitys.DetechAbility<IMoverAbility2>();
        }
    }
}
