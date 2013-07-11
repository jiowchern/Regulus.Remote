using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Physics
{
	
	public interface IQuadObject
	{
		
		System.Windows.Rect Bounds { get; }
		event EventHandler BoundsChanged;
	}
}
