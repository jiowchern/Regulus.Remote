using System;


using Regulus.CustomType;

namespace Regulus.Collection
{
	public interface IQuadObject
	{
		event EventHandler BoundsChanged;

		Rect Bounds { get; }
	}
}
