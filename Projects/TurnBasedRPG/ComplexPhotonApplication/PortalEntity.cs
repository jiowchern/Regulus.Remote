using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Project.TurnBasedRPG
{
    class ObserveAbility : IObserveAbility
    {
        IMap _Map;
        string _TargetMap;
        Types.Vector2 _Position;
        System.Windows.Rect _Vision;
        public ObserveAbility(System.Windows.Rect vision)
        {
            _Vision = vision;

            _Position = new Types.Vector2();
            _Position.X = (float)(_Vision.X + _Vision.Width / 2);
            _Position.Y = (float)(_Vision.Y + _Vision.Height / 2);
        }
        void IObserveAbility.Update(PhysicalAbility[] observeds, List<IObservedAbility> lefts)
        {

            
            
        }

        Types.Vector2 IObserveAbility.Position
        {
            get { return _Position; }
        }

        System.Windows.Rect IObserveAbility.Vision
        {
            get { return _Vision; }
        }

        event Action<IObservedAbility> IObserveAbility.IntoEvent
        {
            add {  }
            remove {  }
        }

        event Action<IObservedAbility> IObserveAbility.LeftEvent
        {
            add {  }
            remove {  }
        }
    }
    
    class PortalEntity : Entity
    {
        public PortalEntity(Guid id)
            : base(id)
        { 

        }

        protected override void _SetAbility(Entity.AbilitySet abilitys)
        {
                                       
        }

        protected override void _RiseAbility(Entity.AbilitySet abilitys)
        {
            
        }
    }
}
