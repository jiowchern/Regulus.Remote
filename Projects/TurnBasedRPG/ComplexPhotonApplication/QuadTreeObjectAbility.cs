using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
    class PhysicalAbility : Regulus.Physics.IQuadObject
    {
        System.Windows.Rect _Bounds;
        Entity _Owner;
        public PhysicalAbility(System.Windows.Rect bounds ,Entity owner)
        {            
            _Bounds = bounds;
            _Owner = owner;
        }

        public IMoverAbility MoverAbility { get { return _Owner.FindAbility<IMoverAbility>(); } }
        public IObservedAbility ObservedAbility { get { return _Owner.FindAbility<IObservedAbility>(); } }
        public System.Windows.Rect Bounds
        {
            get { return _Bounds; }
        }

        public event EventHandler BoundsChanged;
        public void UpdateBounds(float left , float top)
        {
            _UpdateBounds(left, top, ref _Bounds);
        }
        private void _UpdateBounds(float left, float top, ref System.Windows.Rect bounds)
		{
            bounds.Location = new System.Windows.Point(left, top);            
			if (BoundsChanged != null)
				BoundsChanged(this, new EventArgs());
		}        

    }
}
