using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
    class PhysicalAbility : Regulus.Physics.IQuadObject
    {
        Regulus.Types.Rect _Bounds;        
        public PhysicalAbility(Regulus.Types.Rect bounds ,Entity owner)
        {            
            _Bounds = bounds;            
        }
        
        public Regulus.Types.Rect Bounds
        {
            get { return _Bounds; }
        }

        public event EventHandler BoundsChanged;
        public void UpdateBounds(float left , float top)
        {
            _UpdateBounds(left, top, ref _Bounds);
        }
        private void _UpdateBounds(float left, float top, ref Regulus.Types.Rect bounds)
		{
            bounds.Location = new Regulus.Types.Point(left, top);            
			if (BoundsChanged != null)
				BoundsChanged(this, new EventArgs());
		}        

    }
}
