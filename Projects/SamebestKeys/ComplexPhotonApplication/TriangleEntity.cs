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
            Regulus.Types.Rect rect = new Types.Rect(firstPoint.X, firstPoint.Y, firstPoint.X, firstPoint.Y);
            while (points.Count > 0)
            {
                var point = points.Dequeue();
                if (rect.Left > point.X)
                {
                    rect.Left = point.X;
                }
                if (rect.Top > point.Y)
                {
                    rect.Top = point.Y;
                }
                if (rect.Right < point.X)
                {
                    rect.Right = point.X;
                }
                if (rect.Bottom < point.Y)
                {
                    rect.Bottom = point.Y;
                }
            }
            _QuadTreeObjectAbility = new PhysicalAbility(rect, this);
            
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
