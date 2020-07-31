using System;


using Regulus.Utility;

namespace Regulus.Collection
{
	public interface IQuadObject
	{
		event EventHandler BoundsChanged;

		Rect Bounds { get; }
	}
}
