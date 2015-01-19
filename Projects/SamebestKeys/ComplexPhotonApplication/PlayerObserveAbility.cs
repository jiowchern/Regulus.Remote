using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
	// 玩家觀察功能


    class PlayerObserveAbility : IObserveAbility
    {

       

        Field _Field;
        Regulus.Project.SamebestKeys.Serializable.DBEntityInfomation _Infomation;
        public PlayerObserveAbility(Regulus.Project.SamebestKeys.Serializable.DBEntityInfomation info)
        {            
            _Infomation = info;
            _Field = new Field();

        }

        

        void IObserveAbility.Update(Regulus.Project.SamebestKeys.Map.EntityInfomation[] observeds, List<IObservedAbility> lefts)
        {
            _Field.Update(this, observeds, lefts);
        }

        Regulus.CustomType.Rect IObserveAbility.Vision
        {
            get 
            {
                var position  = _Infomation.Property.Position;                    
                var vision = _Infomation.Property.Vision;
                return new Regulus.CustomType.Rect(position.X - vision / 2, position.Y - vision / 2, vision, vision); 
            }
        }


        event Action<IObservedAbility> IObserveAbility.IntoEvent
        {
            add { _Field.IntoEvent += value; }
            remove { _Field.IntoEvent -= value; }
        }

        event Action<IObservedAbility> IObserveAbility.LeftEvent
        {
            add { _Field.LeftEvent += value; }
            remove { _Field.LeftEvent -= value; }
        }

        CustomType.Vector2 IObserveAbility.Position
        {
            get { return _Infomation.Property.Position; }
        }

        public void Clear()
        {
            _Field.Clear();
        }
    }
}
