using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    //穿梭 跳副本
    class ShuttleEntity : Entity, IObserveAbility
    {
        string _Target;        
        Regulus.Types.Rect _Vision;
        Types.Vector2 _Position;

        public ShuttleEntity(Guid id, Regulus.Types.Rect vision,string target ) : base(id)
        { 
            _Target = target;
            _Vision = vision;

            _Position = new Types.Vector2();
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

        void IObserveAbility.Update(Map.EntityInfomation[] observeds, List<IObservedAbility> lefts)
        {
            foreach (var observed in observeds)
            {
                if (observed.Cross != null)
                {
                    observed.Cross.Jump(_Target);
                }
            }
        }

        Types.Vector2 IObserveAbility.Position
        {
            get { return _Position; }
        }

        Types.Rect IObserveAbility.Vision
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
}
