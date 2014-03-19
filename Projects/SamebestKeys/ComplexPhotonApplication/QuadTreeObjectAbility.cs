using Regulus.Types;
using System;

namespace Regulus.Project.SamebestKeys
{
	/// <summary>
	/// 物理功能
	/// </summary>
    class PhysicalAbility : Regulus.Physics.IQuadObject
    {
        Rect _Bounds;        
        public PhysicalAbility(Rect bounds ,Entity owner)
        {            
            _Bounds = bounds;            
        }
        
        public Rect Bounds
        {
            get { return _Bounds; }
        }

        public event EventHandler BoundsChanged;
        public void UpdateBounds(float left , float top)
        {
            _UpdateBounds(left, top, ref _Bounds);
        }
        private void _UpdateBounds(float left, float top, ref Rect bounds)
		{
            bounds.Location = new Point(left, top);            
			if (BoundsChanged != null)
				BoundsChanged(this, new EventArgs());
		}
    }
}
