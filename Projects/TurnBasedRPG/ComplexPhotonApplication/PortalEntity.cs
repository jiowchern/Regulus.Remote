using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Project.TurnBasedRPG
{
   

    class PortalEntity : Entity, IObserveAbility
    {
        string _TargetMap;
        CustomType.Vector2 _TargetPosition;
        CustomType.Vector2 _Position;
        Regulus.CustomType.Rect _Vision;

        public PortalEntity(Guid id, Regulus.CustomType.Rect vision,string target_map, CustomType.Vector2 target_position)
            : base(id)
        {
            _TargetPosition = new CustomType.Vector2();
            _TargetPosition.X = target_position.X;
            _TargetPosition.Y = target_position.Y;
            _TargetMap = target_map;
            _Vision = vision;

            _Position = new CustomType.Vector2();
            _Position.X = (float)(_Vision.X + _Vision.Width /2);
            _Position.X = (float)(_Vision.Y + _Vision.Height / 2);
        }
        
        protected override void _SetAbility(Entity.AbilitySet abilitys)
        {
            abilitys.AttechAbility<IObserveAbility>(this);
        }

        protected override void _RiseAbility(Entity.AbilitySet abilitys)
        {
            abilitys.DetechAbility<IObserveAbility>();
        }

        void IObserveAbility.Update(Regulus.Project.TurnBasedRPG.Map.EntityInfomation[] observeds, List<IObservedAbility> lefts)
        {
            foreach (var observed in observeds)
            {
                if (observed.Cross != null)
                {
                    observed.Cross.Move(_TargetMap, Regulus.Utility.ValueHelper.DeepCopy(_TargetPosition) );
                }
            }
        }

        CustomType.Vector2 IObserveAbility.Position
        {
            get { return _Position; }
        }

        Regulus.CustomType.Rect IObserveAbility.Vision
        {
            get { return _Vision; }
        }

        event Action<IObservedAbility> IObserveAbility.IntoEvent
        {
            add { }
            remove { }
        }

        event Action<IObservedAbility> IObserveAbility.LeftEvent
        {
            add { }
            remove { }
        }
    }
}
