using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
    class PlayerObserveAbility : IObserveAbility
    {
        IObservedAbility _Observed;
        Field _Field;
        Regulus.Project.TurnBasedRPG.Serializable.DBEntityInfomation _Infomation;
        public PlayerObserveAbility(IObservedAbility observed , Regulus.Project.TurnBasedRPG.Serializable.DBEntityInfomation info)
        {
            _Observed = observed;
            _Infomation = info;
            _Field = new Field();
        }

        void IObserveAbility.Update(IObservedAbility[] observeds, List<IObservedAbility> lefts)
        {
            _Field.Update(this, observeds, lefts);
        }

        IObservedAbility IObserveAbility.Observed
        {
            get { return _Observed; }
        }

        float IObserveAbility.Vision
        {
            get { return _Infomation.Property.Vision ; }
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


		void IObserveAbility.Update(Physics.QuadTree<Physics.IQuadObject> observeds, List<IObservedAbility> lefts)
		{
			_Field.Update(this, observeds, lefts);
		}
	}
}
