using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
    class PhysicalAbility : Regulus.Collection.IQuadObject
    {
        Regulus.CustomType.Rect _Bounds;        
        public PhysicalAbility(Regulus.CustomType.Rect bounds ,Entity owner)
        {            
            _Bounds = bounds;            
        }
        
        public Regulus.CustomType.Rect Bounds
        {
            get { return _Bounds; }
        }

        public event EventHandler BoundsChanged;
        public void UpdateBounds(float left , float top)
        {
            _UpdateBounds(left, top, ref _Bounds);
        }
        private void _UpdateBounds(float left, float top, ref Regulus.CustomType.Rect bounds)
		{
            bounds.Location = new Regulus.CustomType.Point(left, top);            
			if (BoundsChanged != null)
				BoundsChanged(this, new EventArgs());
		}        

    }
}
