using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Collection
{
    using Regulus.CustomType;
	public interface IQuadObject
	{
		
		Rect Bounds { get; }
		event EventHandler BoundsChanged;
	}
}
